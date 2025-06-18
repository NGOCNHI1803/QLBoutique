using Microsoft.AspNetCore.SignalR;

namespace QLBoutique.Model
{
    public class HoaDonHub : Hub
    {
        public async Task SendUpdate(string maHoaDon)
        {
            await Clients.All.SendAsync("ReceiveUpdate", maHoaDon);
        }
    }
}
