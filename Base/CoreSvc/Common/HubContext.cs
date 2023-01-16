using System;
using System.Linq;
using System.Threading.Tasks;
using CoreData.Common;
using CoreType.Types;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace CoreSvc.Common
{
    public class HubContext
    {
        private const string clientMethodName = Constants.CLIENT_RECEIVER_METHOD_NAME;
        private readonly IHubContext<SignalRHub> _hubContext;

        public IHubClients Clients => _hubContext.Clients;
        public IGroupManager Groups => _hubContext.Groups;

        public HubContext(IHubContext<SignalRHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task Send(SocketActionType socketActionType, params object[] args)
        {
            var session = SessionAccessor.GetSession();

            return Send(session, socketActionType, args);
        }

        public Task Send(SessionContext session, SocketActionType socketActionType, params object[] args)
        {
            return Send(session.CurrentUser.UserId, socketActionType, args);
        }

        public Task Send(int userId, SocketActionType socketActionType, params object[] args)
        {
            try
            {
                if (userId > 0)
                {
                    var user = Clients.User(userId.ToString());
                    if (user != null)
                    {
                        return user.SendCoreAsync(clientMethodName, args.Prepend(socketActionType).ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "HubContext.Send");
            }

            return Task.CompletedTask;
        }

        public Task SendToAll(SocketActionType socketActionType, params object[] args)
        {
            try
            {
                return Clients.All.SendCoreAsync(clientMethodName, args.Prepend(socketActionType).ToArray());
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "HubContext.SendToAll");
            }

            return Task.CompletedTask;
        }
    }
}