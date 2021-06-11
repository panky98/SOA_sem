using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DataMicroservice.Services
{
    public class EventHub : Hub
    {

        public async Task SendMessage(string groupName,string message)
        {
            Console.WriteLine("HUB: MessageSent - " + message);
            var identity = (ClaimsIdentity)Context.User.Identity;
            await Clients.OthersInGroup(groupName).SendAsync("ReceiveMessage", message);
        }
        public async Task AddToGroup(string groupName)
        {
            var identity = (ClaimsIdentity)Context.User.Identity;
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine(Context.ConnectionId + " entered group " + groupName);
        }

        public async Task RemoveFromGroup(string groupName)
        {
            var identity = (ClaimsIdentity)Context.User.Identity;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            Console.WriteLine(Context.ConnectionId + " left group " + groupName);

        }
    }
}
