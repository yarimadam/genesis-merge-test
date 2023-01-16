using System.Threading.Tasks;
using CoreType.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CoreSvc.Common
{
    [Authorize]
    public class SignalRHub : Hub //Hub<ISignalRHub>
    {
        //public override async Task OnConnectedAsync()
        //{
        //    await base.OnConnectedAsync();
        //}

        public async Task ReceiveMessage(SocketActionType socketActionType, object message)
        {
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }
    }

    // public interface IClientHub
    // {
    //     Task ReceiveMessage(string user, string message);
    // }
}