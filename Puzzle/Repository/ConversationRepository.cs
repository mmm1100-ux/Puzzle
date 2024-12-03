using Helper;
using Microsoft.EntityFrameworkCore;
using Models;
using Puzzle.Data;
using Puzzle.Models;
using System;
using System.Linq;

namespace Puzzle.Repository
{
    public class ConversationRepository
    {
        public void AlarmBySms()
        {
            using var db = new PuzzleDbContext();

            try
            {
                var conversation = db.Conversation
                    .Where(x => x.Project.IsDelete == false)
                    .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                    .Where(x => x.IsDelete == false)
                    .Where(x => x.UnreadByCustomer)
                    .Where(x => x.CreatedAt <= DateTime.Now.AddMinutes(-60))
                    .Where(x => x.Accepted == true)
                    .Where(x => x.Project.Customer.PushSms == true)
                    .Include(x => x.Project).ThenInclude(x => x.Customer)
                    .ToList();

                foreach (var item in conversation.GroupBy(x => x.Project.Customer))
                {
                    var smsAlarm = db.SmsAlarm
                        .Where(x => x.CustomerId == item.Key.Id)
                        .OrderByDescending(x => x.CreatedAt)
                        .FirstOrDefault();

                    if (smsAlarm == null)
                    {
                        SendSms(item.Key);
                    }
                    else if ((item.Key.LastActivity != null) && (item.Key.LastActivity.Value > smsAlarm.CreatedAt))
                    {
                        if (item.Where(x => x.CreatedAt > item.Key.LastActivity.Value).Any())
                        {
                            SendSms(item.Key);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                db.Error.Add(new Error { DateTime = DateTime.Now, StackTrace = exception.StackTrace, Message = exception.Message });

                db.SaveChanges();
            }
        }

        public void AlarmBySmsToDesigner()
        {
            using var db = new PuzzleDbContext();

            try
            {
                var conversation = db.Conversation
                    .Where(x => x.Project.IsDelete == false)
                    .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                    .Where(x => x.IsDelete == false)
                    .Where(x => x.Project.DesignerId != null)
                    .Where(x => x.UnreadByDesigner)
                    .Where(x => x.CreatedAt <= DateTime.Now.AddMinutes(-30))
                    .Where(x => x.Accepted == true)
                    .Include(x => x.Project).ThenInclude(x => x.Designer)
                    .ToList();

                foreach (var item in conversation.GroupBy(x => x.Project.Designer))
                {
                    var smsAlarm = db.SmsAlarm
                        .Where(x => x.UserId == item.Key.Id)
                        .OrderByDescending(x => x.CreatedAt)
                        .FirstOrDefault();

                    if (smsAlarm == null)
                    {
                        SendSmsToDesigner(item.Key);
                    }
                    else if ((item.Key.LastActivity != null) && (item.Key.LastActivity.Value > smsAlarm.CreatedAt))
                    {
                        if (item.Where(x => x.CreatedAt > item.Key.LastActivity.Value).Any())
                        {
                            SendSmsToDesigner(item.Key);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                db.Error.Add(new Error
                {
                    DateTime = DateTime.Now,
                    StackTrace = exception.StackTrace,
                    Message = exception.Message
                });

                db.SaveChanges();
            }
        }

        public void SendSms(Customer customer)
        {
            using var db = new PuzzleDbContext();

            try
            {
                db.SmsAlarm.Add(new Models.SmsAlarm { CreatedAt = DateTime.Now, CustomerId = customer.Id });

                db.SaveChanges();

                SMSHelper.SendByPattern(customer.Mobile, customer.FirstName, "suyud178a4lqrxr");
            }
            catch (Exception exception)
            {
                db.Error.Add(new Error { DateTime = DateTime.Now, StackTrace = exception.StackTrace, Message = exception.Message });

                db.SaveChanges();
            }
        }

        public void SendSmsToDesigner(User user)
        {
            using var db = new PuzzleDbContext();

            try
            {
                db.SmsAlarm.Add(new Models.SmsAlarm { CreatedAt = DateTime.Now, UserId = user.Id });

                db.SaveChanges();

                SMSHelper.SendByPattern(user.PhoneNumber, user.Name, "suyud178a4lqrxr");
            }
            catch (Exception exception)
            {
                db.Error.Add(new Error { DateTime = DateTime.Now, StackTrace = exception.StackTrace, Message = exception.Message });

                db.SaveChanges();
            }
        }
    }
}
