using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreData;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Operations;
using CoreData.Repositories;
using CoreSvc.Common;
using CoreType.DBModels;
using CoreType.Types;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;

namespace CoreSvc.Operations
{
    public class BulkSaveOperation<TEntity, TRepository> : OperationBaseAsync<RequestWithExcelData<TEntity>, ResponseWrapper>
        where TEntity : class, new()
        where TRepository : GenericRepositoryBase
    {
        private readonly Func<TEntity, Task<TEntity>> _saveFunc;
        private readonly SaveOperation<TEntity, TRepository> _saveOperation;
        private readonly NotificationRepository _notificationRepository;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public BulkSaveOperation(TRepository repository, Func<TEntity, Task<TEntity>> saveFunc) : base(new UnitOfWork(repository.Context))
        {
            _saveFunc = saveFunc;
            _notificationRepository = new NotificationRepository();
            _saveOperation = new SaveOperation<TEntity, TRepository>(repository);
        }

        protected override async Task<ResponseWrapper> OnExecuteAsync(RequestWithExcelData<TEntity> request, ResponseWrapper _)
        {
            BulkSaveWithNotificationAsync(request);

            // Wait a bit until service intances acquired by method above.
            await Task.Delay(500);

            return new ResponseWrapper
            {
                Message = LocalizedMessages.BATCH_UPLOAD_STARTED,
                Success = true
            };
        }

        /// <summary>
        /// Bulk saves provided list.
        /// If HubContext is alive, websocket notification will be send too.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Returns failed records</returns>
        public async Task<Notification> BulkSaveWithNotificationAsync(RequestWithExcelData<TEntity> request)
        {
            try
            {
                _stopwatch.Restart();

                var session = SessionAccessor.GetSession();
                var hubContext = ServiceLocator.Current.GetService<HubContext>();
                var servicePath = ServiceLocator.GetHttpContextAccessor()?.HttpContext?.Request?.GetDisplayUrl();

                var oldNotification = new Notification
                {
                    NotificationId = request?.NotificationId ?? default
                };

                if (oldNotification.NotificationId > 0)
                {
                    oldNotification = await _notificationRepository.GetAsync(oldNotification);
                    if (oldNotification.NotificationSettings != null)
                    {
                        oldNotification.NotificationSettings.Status = 3;
                        await _notificationRepository.SaveAsync(oldNotification);
                    }
                }

                var response = await BulkSave(request);
                var unSuccessfulRecords = response.Data;
                var isSuccessful = response.Success;
                var title = (isSuccessful ? LocalizedMessages.EXCEL_IMPORT_SUCCESS_TITLE : LocalizedMessages.EXCEL_IMPORT_ERROR_TITLE).GetKeyCode();
                var subtitle = (isSuccessful ? LocalizedMessages.EXCEL_IMPORT_SUCCESS_SUBTITLE : LocalizedMessages.EXCEL_IMPORT_ERROR_SUBTITLE).GetKeyCode();
                var message = (isSuccessful ? LocalizedMessages.EXCEL_IMPORT_SUCCESS_MESSAGE : LocalizedMessages.EXCEL_IMPORT_ERROR_MESSAGE).GetKeyCode();
                var description = (isSuccessful ? LocalizedMessages.EXCEL_IMPORT_SUCCESS_DESCRIPTION : LocalizedMessages.EXCEL_IMPORT_ERROR_DESCRIPTION).GetKeyCode();

                var notificationSettings = new NotificationSettings
                {
                    NotificationSettingsId = oldNotification.NotificationSettingsId,
                    NotificationType = (int) SocketActionType.EXCEL_IMPORT,
                    UserId = session.GetUserId(),
                    Title = title,
                    Subtitle = subtitle,
                    Message = message,
                    Data = JsonConvert.SerializeObject(
                        new NotificationExcelData<TEntity>
                        {
                            ServicePath = servicePath,
                            ClientType = request?.Type,
                            UnsuccessfulRecords = unSuccessfulRecords,
                            ElapsedTime = _stopwatch.Elapsed.TotalSeconds
                        }, CustomJsonSerializerSettings.CamelCase),
                    Description = description,
                    Status = isSuccessful ? 1 : 2
                };

                var notification = new Notification
                {
                    NotificationId = oldNotification.NotificationId,
                    NotificationSettingsId = oldNotification.NotificationSettingsId,
                    NotificationSettings = notificationSettings,
                    SendDate = DateTime.UtcNow,
                    UserId = session.GetUserId(),
                    Status = 1
                };

                await _notificationRepository.SaveAsync(notification);

                if (hubContext != null)
                    await hubContext.Send(session, SocketActionType.EXCEL_IMPORT, notification);

                return notification;
            }
            catch (Exception e)
            {
                Log.Fatal(e, "BulkSaveWithNotificationAsync");
                throw;
            }
            finally
            {
                _stopwatch.Stop();
            }
        }

        private async Task<ResponseWrapper<List<ResponseWrapper<TEntity>>>> BulkSave(RequestWithExcelData<TEntity> request)
        {
            var response = new ResponseWrapper<List<ResponseWrapper<TEntity>>>
            {
                Data = new List<ResponseWrapper<TEntity>>()
            };

            if (request == null)
            {
                response.AddError(new ArgumentNullException(nameof(request)));
                response.Message = LocalizedMessages.PROCESS_FAILED;
                return response;
            }

            try
            {
                UnitOfWork.Context.ChangeTracker.AutoDetectChangesEnabled = false;
                UnitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                foreach (var entity in request.Data)
                {
                    try
                    {
                        var result = await _saveOperation.ExecuteAsync(() => _saveFunc(entity));

                        if (result != null && result.Success != true)
                        {
                            result.Data = entity;
                            response.Data.Add(result);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    finally
                    {
                        UnitOfWork.DetachAllEntities();
                    }
                }
            }
            catch (Exception e)
            {
                response.AddError(e);
            }

            if (response.Data.Any())
            {
                response.Message = LocalizedMessages.PROCESS_FAILED;
            }
            else
            {
                response.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
                response.Success = true;
            }

            return response;
        }

        /* public static List<ResponseWrapper> BulkSave<T>(List<T> dataList, Func<T, T> SaveFunc)
        {
            var unSuccessfulResults = new ConcurrentBag<ResponseWrapper>();

            // WARN: Related exception can occur on too many parallel threads, limit it with MaxDegreeOfParallelism option -> Npgsql.PostgresException (0x80004005): 53300: sorry, too many clients already
            Parallel.ForEach(dataList /*, new ParallelOptions {MaxDegreeOfParallelism = Environment.ProcessorCount } #1#,
                data =>
                {
                    Exception exception = null;

                    try
                    {
                        SaveFunc(data);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }
                    finally
                    {
                        if (exception != null || ReflectionHelper.GetPrimaryIdVal(data) <= 0)
                        {
                            var response = new ResponseWrapper
                            {
                                Data = data,
                                Message = exception != null
                                    ? exception.Message + "\n\n" + exception.InnerException
                                    : LocalizedMessages.PROCESS_FAILED.ToString()
                            };

                            unSuccessfulResults.Add(response);
                        }
                    }
                });

            return unSuccessfulResults.ToList();
        } */
    }
}