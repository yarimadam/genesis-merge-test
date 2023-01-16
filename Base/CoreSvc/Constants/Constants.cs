using System;
using CoreSvc.Controllers;

namespace CoreSvc
{
    public abstract class Constants : CoreData.Constants
    {
        public static readonly string PROJECT_NAME = typeof(Constants).Assembly.FullName;
        public const int MAX_LOG_CHAR_LENGTH = 10000;

        public static readonly Type[] LISTING_ALLOWED_CONTROLLER_METHODS =
        {
            typeof(AuthController)
        };

        public static class ResourceCodes
        {
            public const string AdminPages = "AdminPages";
            public const string AuthActionsInfo_tab = "AuthActionsInfo_tab";
            public const string AuthTemplateDetails_tab = "AuthTemplateDetails_tab";
            public const string AuthTemplates = "AuthTemplates";
            public const string AuthTemplates_tab = "AuthTemplates_tab";
            public const string Communication = "Communication";
            public const string CommunicationDefinitions_tab = "CommunicationDefinitions_tab";
            public const string CommunicationTemplates_tab = "CommunicationTemplates_tab";
            public const string CompanyInfo_tab = "CompanyInfo_tab";
            public const string CoreCompany_Res = "CoreCompany_Res";
            public const string CoreDepartment_Res = "CoreDepartment_Res";
            public const string CorePermissions_Res = "CorePermissions_Res";
            public const string CoreSearchMonitor_Res = "CoreSearchMonitor_Res";
            public const string Definitions = "Definitions";
            public const string MainPage = "MainPage";
            public const string MainPageTab = "MainPageTab";
            public const string Notification_res = "Notification_res";
            public const string Notifications = "Notifications";
            public const string NotificationSettings_res = "NotificationSettings_res";
            public const string ResourceDefinition = "ResourceDefinition";
            public const string ResourceDefinitionInfo_tab = "ResourceDefinitionInfo_tab";
            public const string SystemParameters = "SystemParameters";
            public const string SystemParametersInfo_tab = "SystemParametersInfo_tab";
            public const string TestResource = "TestResource";
            public const string TransactionLogs_Res = "TransactionLogs_Res";
            public const string UserInfo_tab = "UserInfo_tab";
            public const string UserPerm_tab = "UserPerm_tab";
            public const string Users = "Users";
            public const string Tenant_Res = "Tenant_Res";
            public const string ChangePassword_Res = "ChangePassword_Res";
            public const string Profile_Res = "Profile_Res";
        }
    }
}