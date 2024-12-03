using Helper;
using Microsoft.AspNetCore.SignalR;
using Models;
using Puzzle.Data;
using Puzzle.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string userId, string projectId, string role, string message)
        {
            using var db = new PuzzleDbContext();

            var conversation = new Conversation
            {
                CreatedAt = DateTime.Now,
                Message = message,
                UserId = userId,
                ProjectId = projectId.ToInt(),
                Role = Enum.Parse<Enums.Enum.Role>(role)
            };

            db.Conversation.Add(conversation);

            db.SaveChanges();

            var user = db.Users.Where(x => x.Id == userId).First();

            await Clients.All.SendAsync("ReceiveMessage", user, conversation, role);
        }

        //public async Task SendVoice(iformFile userId, string projectId, string role, string message)
        //{
        //    using var db = new PuzzleDbContext();

        //    var conversation = new Conversation
        //    {
        //        CreatedAt = DateTime.Now,
        //        Message = message,
        //        UserId = userId,
        //        ProjectId = projectId.ToInt(),
        //        Role = Enum.Parse<Enums.Enum.Role>(role)
        //    };

        //    db.Conversation.Add(conversation);

        //    db.SaveChanges();

        //    var user = db.Users.Where(x => x.Id == userId).First();

        //    await Clients.All.SendAsync("ReceiveMessage", user, conversation, role);
        //}

        public async Task EditMessage(int conversationId, string message)
        {
            using var db = new PuzzleDbContext();

            var conversation = db.Conversation
                .Where(x => x.Id == conversationId)
                .First();

            conversation.Message = message;

            db.SaveChanges();

            await Clients.All.SendAsync("ReceiveEditMessage", conversationId, message);
        }

        public async Task RemoveMessage(int conversationId)
        {
            using var db = new PuzzleDbContext();

            var conversation = db.Conversation
                .Where(x => x.Id == conversationId)
                .First();

            db.Conversation.Remove(conversation);

            db.SaveChanges();

            await Clients.All.SendAsync("RemoveMessage", conversationId);
        }

        public async Task ReplyMessage(string userId, string projectId, string role, string message)
        {
            using var db = new PuzzleDbContext();

            var conversation = new Conversation
            {
                CreatedAt = DateTime.Now,
                Message = message,
                UserId = userId,
                ProjectId = projectId.ToInt(),
                Role = Enum.Parse<Enums.Enum.Role>(role)
            };

            db.Conversation.Add(conversation);

            db.SaveChanges();

            var user = db.Users.Where(x => x.Id == userId).First();

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
