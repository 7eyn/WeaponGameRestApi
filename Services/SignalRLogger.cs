using Microsoft.AspNetCore.SignalR;
using WeaponGameRestApi.Core;
using WeaponGameRestApi.Hubs;
namespace WeaponGameRestApi.Services
{
    public class SignalRLogger : IGameLogger
    {
        private readonly IHubContext<GameHub> _hubContext;

        public SignalRLogger(IHubContext<GameHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void Log(string message)
        {
            _hubContext.Clients.All.SendAsync("LogMessage", message);
        }
    }
}
