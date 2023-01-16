using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreData.Infrastructure;
using CoreData.Infrastructure.Producers;
using CoreType.DBModels;
using CoreType.Models;
using CoreType.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using MailMessage = CoreType.Types.MailMessage;
using NetMailMessage = System.Net.Mail.MailMessage;

namespace CoreData.Common
{
    public class CommunicationManager
    {
        public static CommunicationDefinitions GetCommunicationDefinition(int commDefinitionId) => GetCommunicationDefinition(commDefinitionId, null, null);

        public static CommunicationDefinitions GetCommunicationDefinition(string commDefinitionName) => GetCommunicationDefinition(0, commDefinitionName, null);

        public static CommunicationDefinitions GetCommunicationDefinition(short commDefinitionType) => GetCommunicationDefinition(0, null, commDefinitionType);

        public static CommunicationDefinitions GetCommunicationDefinition(int commDefinitionId, string commDefinitionName, short? commDefinitionType)
        {
            try
            {
                var communicationDefinitions = DistributedCache.Get<List<CommunicationDefinitions>>("CommunicationDefinitions");

                return communicationDefinitions
                    .Where(x => commDefinitionId == 0 || x.CommDefinitionId.Equals(commDefinitionId))
                    .Where(x => !commDefinitionType.HasValue || x.CommDefinitionType.Equals(commDefinitionType.Value))
                    .Single(x => string.IsNullOrEmpty(commDefinitionName) || x.CommDefinitionName.Equals(commDefinitionName));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "CommunicationManager.GetCommunicationDefinition", commDefinitionName);
            }

            return null;
        }

        public static CommunicationTemplates GetCommunicationTemplate(string commTemplateName)
        {
            try
            {
                var communicationTemplates = DistributedCache.Get<List<CommunicationTemplates>>("CommunicationTemplates");

                return communicationTemplates.Single(x => string.IsNullOrEmpty(commTemplateName) || x.CommTemplateName.Equals(commTemplateName));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "CommunicationManager.GetCommunicationTemplate", commTemplateName);
            }

            return null;
        }

        public class Mail
        {
            [Obsolete]
            public static bool SendViaSettings(MailMessage mailMessage)
            {
                try
                {
                    MailSettings MailSettings = ConfigurationManager.MailSettings;

                    SmtpClient client = new SmtpClient
                    {
                        Host = MailSettings.SMTPServer,
                        Port = MailSettings.SMTPPort,
                        EnableSsl = MailSettings.UseSSL,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(MailSettings.SenderName, MailSettings.SenderPass)
                    };

                    NetMailMessage netMailMessage = new NetMailMessage
                    {
                        From = new MailAddress(MailSettings.SenderAccount),
                        Subject = mailMessage.Subject,
                        Body = mailMessage.Body
                    };
                    netMailMessage.To.Add(mailMessage.To);

                    client.Send(netMailMessage);
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "CommunicationManager.Send", mailMessage);
                    return false;
                }

                return true;
            }

            public static Task SendAsync(string commTemplateName, object requestObject, object responseObject)
            {
                var communication = TemplateEngineManager.GetFormattedCommunication(commTemplateName, requestObject, responseObject);

                return SendAsync(communication);
            }

            public static bool Send(string commTemplateName, object requestObject, object responseObject)
            {
                var communication = TemplateEngineManager.GetFormattedCommunication(commTemplateName, requestObject, responseObject);

                return Send(communication);
            }

            public static Task SendAsync(Communication communication)
            {
                return CommMailProducer.Produce(communication);
            }

            public static bool Send(Communication communication)
            {
                try
                {
                    if (communication != null)
                    {
                        var commDefinition = communication.CommunicationDefinitions;
                        var commTemplate = communication.CommunicationTemplates;

                        SmtpClient client = new SmtpClient
                        {
                            Host = commDefinition.EmailSmtpServer,
                            Port = Convert.ToInt32(commDefinition.EmailPort),
                            EnableSsl = commDefinition.EmailSecurityType == 1,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(commDefinition.EmailUsername, EncryptionManager.Decrypt(commDefinition.EmailPassword))
                        };

                        var emailBody = commTemplate.EmailIsBodyHtml == true
                            ? $"<html><body>{commTemplate.EmailBody}</body></html>"
                            : commTemplate.EmailBody;
                        var emailSubject = Constants.NEW_LINE_REGEX.Replace(commTemplate.EmailSubject, " ");
                        var senderName = string.Empty;

                        if (!string.IsNullOrWhiteSpace(commTemplate.EmailSenderName))
                            senderName = commTemplate.EmailSenderName;
                        else if (!string.IsNullOrWhiteSpace(commDefinition.EmailSenderName))
                            senderName = commDefinition.EmailSenderName;

                        NetMailMessage netMailMessage = new NetMailMessage
                        {
                            From = new MailAddress(commDefinition.EmailSenderAccount, senderName),
                            Subject = emailSubject,
                            Body = emailBody,
                            IsBodyHtml = commTemplate.EmailIsBodyHtml ?? false
                        };

                        if (!string.IsNullOrEmpty(commTemplate.EmailRecipients))
                        {
                            commTemplate.EmailRecipients = Constants.NEW_LINE_REGEX.Replace(commTemplate.EmailRecipients, string.Empty).Trim();
                            netMailMessage.To.Add(commTemplate.EmailRecipients);
                        }

                        if (!string.IsNullOrEmpty(commTemplate.EmailCcs))
                        {
                            commTemplate.EmailCcs = Constants.NEW_LINE_REGEX.Replace(commTemplate.EmailCcs, string.Empty).Trim();
                            netMailMessage.CC.Add(commTemplate.EmailCcs);
                        }

                        if (!string.IsNullOrEmpty(commTemplate.EmailBccs))
                        {
                            commTemplate.EmailBccs = Constants.NEW_LINE_REGEX.Replace(commTemplate.EmailBccs, string.Empty).Trim();
                            netMailMessage.Bcc.Add(commTemplate.EmailBccs);
                        }

                        client.Send(netMailMessage);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "CommunicationManager.Send", communication);
                }

                return false;
            }

            //public static bool Send(MailMessage mailMessage, string commDefinitionName)
            //{
            //    try
            //    {
            //        var communicationDefinition = GetCommunicationDefinition(commDefinitionName);

            //        if (communicationDefinition != null)
            //        {
            //            SmtpClient client = new SmtpClient
            //            {
            //                Host = communicationDefinition.EmailSmtpServer,
            //                Port = Convert.ToInt32(communicationDefinition.EmailPort),
            //                EnableSsl = communicationDefinition.EmailSecurityType == 1,
            //                DeliveryMethod = SmtpDeliveryMethod.Network,
            //                UseDefaultCredentials = false,
            //                Credentials = new NetworkCredential(communicationDefinition.EmailSenderName, communicationDefinition.EmailPassword),
            //            };

            //            NetMailMessage netMailMessage = new NetMailMessage()
            //            {
            //                From = new MailAddress(communicationDefinition.EmailSenderAccount, communicationDefinition.EmailSenderName),
            //                Subject = mailMessage.Subject,
            //                Body = mailMessage.Body,
            //            };

            //            if (!string.IsNullOrEmpty(mailMessage.To))
            //                netMailMessage.To.Add(mailMessage.To);
            //            if (!string.IsNullOrEmpty(mailMessage.CC))
            //                netMailMessage.CC.Add(mailMessage.CC);
            //            if (!string.IsNullOrEmpty(mailMessage.Bcc))
            //                netMailMessage.Bcc.Add(mailMessage.Bcc);

            //            client.Send(netMailMessage);
            //            return true;
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Fatal(ex, "CommunicationManager.Send", mailMessage);
            //    }
            //    return false;
            //}
        }

        public class SMS
        {
            public static bool Send(Communication communication)
            {
                try
                {
                    if (communication != null)
                    {
                        var commDefinition = communication.CommunicationDefinitions;
                        // var commTemplate = communication.CommunicationTemplates;

                        using (HttpClient http = new HttpClient())
                        {
                            var authValue = new AuthenticationHeaderValue("Basic",
                                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{commDefinition.SmsUsername}:{EncryptionManager.Decrypt(commDefinition.SmsPassword)}")));

                            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, commDefinition.SmsEndpointUrl);
                            requestMessage.Content = new FormUrlEncodedContent(JsonConvert.DeserializeObject<Dictionary<string, string>>(commDefinition.SmsFormData));
                            requestMessage.Headers.Authorization = authValue;

                            HttpResponseMessage result = http.SendAsync(requestMessage).Result;

                            if (string.IsNullOrEmpty(commDefinition.SmsExpectedStatusCode)
                                || (int) result.StatusCode == Convert.ToInt32(commDefinition.SmsExpectedStatusCode))
                            {
                                if (!string.IsNullOrEmpty(commDefinition.SmsExpectedResponse))
                                {
                                    JObject responseJson = JObject.Parse(result.Content.ReadAsStringAsync().Result);
                                    JObject expectedJson = JObject.Parse(commDefinition.SmsExpectedResponse);

                                    foreach (var item in expectedJson)
                                    {
                                        if (!item.Value.Equals(responseJson[item.Key]))
                                            throw new GenesisException("Response does not match the expected format !");
                                    }
                                }

                                Log.Debug("CommunicationManager.Send SMS sent successfully.");

                                return true;
                            }
                            else throw new GenesisException("Http response does not match the expected status code !");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "CommunicationManager.Send", communication);
                }

                return false;
            }

            //public static bool Send(string commDefinitionName)
            //{
            //    try
            //    {
            //        var communicationDefinition = GetCommunicationDefinition(commDefinitionName);

            //        if (communicationDefinition != null)
            //        {
            //            using (HttpClient http = new HttpClient())
            //            {
            //                var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{communicationDefinition.SmsUsername}:{communicationDefinition.SmsPassword}")));

            //                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, communicationDefinition.SmsEndpointUrl);
            //                requestMessage.Content = new FormUrlEncodedContent(JsonConvert.DeserializeObject<Dictionary<string, string>>(communicationDefinition.SmsFormData));
            //                requestMessage.Headers.Authorization = authValue;

            //                HttpResponseMessage result = http.SendAsync(requestMessage).Result;

            //                if (string.IsNullOrEmpty(communicationDefinition.SmsExpectedStatusCode)
            //                    || (int)result.StatusCode == Convert.ToInt32(communicationDefinition.SmsExpectedStatusCode))
            //                {
            //                    if (!string.IsNullOrEmpty(communicationDefinition.SmsExpectedResponse))
            //                    {
            //                        JObject responseJson = JObject.Parse(result.Content.ReadAsStringAsync().Result);
            //                        JObject expectedJson = JObject.Parse(communicationDefinition.SmsExpectedResponse);

            //                        foreach (var item in expectedJson)
            //                        {
            //                            if (!item.Value.Equals(responseJson[item.Key]))
            //                                throw new GenesisException("Response does not match the expected format !");
            //                        }
            //                    }

            //                    return true;
            //                }
            //                else throw new GenesisException("Http response does not match the expected status code !");
            //            }
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Log.Fatal(ex, "CommunicationManager.Send", commDefinitionName);
            //    }
            //    return false;
            //}
        }
    }
}