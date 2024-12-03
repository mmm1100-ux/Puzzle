using Extensions;
using Helper;
using Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Puzzle.Data;
using Puzzle.Models;
using Puzzle.ViewModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ViewModels;
using static Enums.Enum;

namespace Puzzle.Controllers
{
    [Authorize]
    public class ConversationController : Controller
    {
        private readonly IHubContext<ChatHub> _chatHub;

        public ConversationController(IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/conversationlist/{page?}")]
        public IActionResult Index(int? page = 1)
        {
            using var db = new PuzzleDbContext();

            var result = new ProjectViewModel();

            result.Project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                //.Where(x => x.Cancel == false)
                .Where(x => x.Conversation.Where(x => x.Role != Role.Admin).Any())
                .Where(x => x.MediaDone == false || x.Conversation.Where(a => a.UnreadByAdmin).Any())
                .Include(x => x.Customer)
                .Include(x => x.Designer)
                .Include(x => x.Parent)
                //.Include(x => x.Child)
                //.Include(x => x.Media)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport)
                .ToList()
                .OrderByDescending(x => x.Conversation.OrderByDescending(a => a.CreatedAt).First().CreatedAt)
                .ToList();

            ViewData["Designer"] = new DesignerRepository().GetList(true);
            ViewData["FormTitle"] = "تایم‌لاین";

            return View("/Views/Project/Index.cshtml", result);
        }

        [HttpPost("/conversation/add")]
        public IActionResult Add(ConversationViewModel.Add model)
        {
            using var db = new PuzzleDbContext();

            var project = db.Project
                .Where(x => x.Id == model.ProjectId)
                .First();

            //project.MediaDone = false;
            project.DoneByDesigner = false;

            var conversation = new Conversation
            {
                CreatedAt = DateTime.Now,
                Message = model.Message,
                ProjectId = model.ProjectId
            };

            conversation.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (User.IsInRole(Enums.Enum.Role.Admin.ToString()))
            {
                conversation.Role = Enums.Enum.Role.Admin;
                conversation.Accepted = true;

                conversation.UnreadByCustomer = true;
                conversation.UnreadByDesigner = true;
            }
            else if (User.IsInRole(Enums.Enum.Role.Designer.ToString()))
            {
                conversation.Role = Enums.Enum.Role.Designer;

                conversation.Accepted = true;

                conversation.UnreadByCustomer = true;
                conversation.UnreadByAdmin = true;
            }

            if (model.Attachment != null)
            {
                if (model.Attachment.ContentType == "audio/webm;codecs=opus")
                {
                    conversation.Attachment = model.Attachment.UploadFile(model.ProjectId, ".ogg");
                }
                else
                {
                    conversation.Attachment = model.Attachment.UploadFile(model.ProjectId);
                }

                conversation.Type = model.Type;

                if (conversation.Type == Enums.Enum.MediaType.File)
                {
                    conversation.FileName = model.Attachment.FileName;
                    conversation.FileSize = model.Attachment.Length.ToInt();
                }
            }

            db.Conversation.Add(conversation);

            db.SaveChanges();

            return Ok(conversation);
        }

        [HttpPost("/conversation/add-image")]
        [DisableRequestSizeLimit]
        public IActionResult Add(ConversationViewModel.AddImage model)
        {
            using var db = new PuzzleDbContext();

            var project = db.Project.Where(x => x.Id == model.ProjectId).First();

            project.MediaDone = false;
            project.DoneByDesigner = false;

            var conversation = new Conversation
            {
                CreatedAt = DateTime.Now,
                Message = model.Message,
                ProjectId = model.ProjectId
            };

            var add = new List<Conversation>();

            conversation.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            if (User.IsInRole(Enums.Enum.Role.Admin.ToString()))
            {
                conversation.Role = Enums.Enum.Role.Admin;
                conversation.Accepted = true;

                conversation.UnreadByCustomer = true;
                conversation.UnreadByDesigner = true;
            }
            else if (User.IsInRole(Enums.Enum.Role.Designer.ToString()))
            {
                conversation.Role = Enums.Enum.Role.Designer;

                if ((model.Type != Enums.Enum.MediaType.Screen) && (model.Type != Enums.Enum.MediaType.Render))
                {
                    conversation.Accepted = true;
                }

                conversation.UnreadByCustomer = true;
                conversation.UnreadByAdmin = true;
            }

            if (string.IsNullOrWhiteSpace(model.Message) == false)
            {
                db.Conversation.Add(new Conversation
                {
                    Accepted = conversation.Accepted,
                    CreatedAt = conversation.CreatedAt,
                    UnreadByAdmin = conversation.UnreadByAdmin,
                    UnreadByCustomer = conversation.UnreadByCustomer,
                    UnreadByDesigner = conversation.UnreadByDesigner,
                    Message = model.Message,
                    UserId = conversation.UserId,
                    ProjectId = conversation.ProjectId,
                    Role = conversation.Role
                });
            }

            if (model.Attachment != null)
            {
                foreach (var item in model.Attachment)
                {
                    var attachment = item.UploadFile(model.ProjectId);

                    add.Add(new Conversation
                    {
                        Accepted = conversation.Accepted,
                        Type = model.Type,
                        CreatedAt = conversation.CreatedAt,
                        ProjectId = conversation.ProjectId,
                        Role = conversation.Role,
                        UserId = conversation.UserId,
                        Attachment = attachment,
                        UnreadByAdmin = conversation.UnreadByAdmin,
                        UnreadByCustomer = conversation.UnreadByCustomer,
                        UnreadByDesigner = conversation.UnreadByDesigner,
                        Message = conversation.Message
                    });
                }
            }

            db.AddRange(add);

            db.SaveChanges();

            foreach (var item in add)
            {
                item.Attachment = item.Attachment.Bitmap(480);
            }

            return Ok(add);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("/conversation/accepted")]
        public IActionResult Accepted(ConversationViewModel.Accepted model)
        {
            using var db = new PuzzleDbContext();

            var conversation = db.Conversation.Where(x => model.ConversationId.Contains(x.Id)).ToList();

            foreach (var item in conversation)
            {
                item.Accepted = model.Status;
            }

            db.SaveChanges();

            return Ok();
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("/conversation/remove")]
        public IActionResult Remove(ConversationViewModel.Remove model)
        {
            using var db = new PuzzleDbContext();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var isAdmin = User.IsInRole(Enums.Enum.Role.Admin.ToString());

            var conversation = db.Conversation
                .Where(x => x.Id == model.Id)
                .First();

            if (isAdmin == false)
            {
                if ((conversation.UserId != userId) || (conversation.CreatedAt.AddMinutes(90) < DateTime.Now))
                {
                    return BadRequest();
                }
            }

            conversation.IsDelete = true;

            db.SaveChanges();

            return Ok();
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost("/conversation/edit")]
        public IActionResult Edit(ConversationViewModel.Add model)
        {
            using var db = new PuzzleDbContext();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var isAdmin = User.IsInRole(Enums.Enum.Role.Admin.ToString());

            var conversation = db.Conversation
                .Where(x => x.Id == model.Id)
                .First();

            if (conversation.UserId == userId)
            {
                if (conversation.CreatedAt.AddMinutes(90) < DateTime.Now)
                {
                    return BadRequest();
                }
            }
            else if (isAdmin == false)
            {
                return BadRequest();
            }

            conversation.Message = model.Message;

            db.SaveChanges();

            return Ok();
        }

        [HttpPost("/conversation/get")]
        public IActionResult Get(int projectId)
        {
            using var db = new PuzzleDbContext();

            var project = db.Project
                .Where(x => x.Id == projectId)
                .Include(x => x.Child).ThenInclude(x => x.ProjectReport).ThenInclude(x => x.User)
                .Include(x => x.ProjectReport).ThenInclude(x => x.User)
                .First();

            if (project.ProjectId != null)
            {
                project = db.Project
                    .Where(x => x.Id == project.ProjectId)
                    .Include(x => x.Child).ThenInclude(x => x.ProjectReport).ThenInclude(x => x.User)
                    .Include(x => x.ProjectReport).ThenInclude(x => x.User)
                    .First();
            }

            var childIds = project.Child.Select(x => x.Id).ToList();

            var result = db.Conversation
                .Where(x => x.ProjectId == project.Id || childIds.Contains(x.ProjectId))
                .Include(x => x.User)
                .Include(x => x.Parent).ThenInclude(x => x.User)
                .Include(x => x.Project).ThenInclude(x => x.Customer)
                .ToList();

            foreach (var item in project.Child)
            {
                result.Add(new Conversation
                {
                    Message = item.Title,
                    CreatedAt = item.CT,
                    Attachment = "ChildForConversation",
                    UserId = "UserId"
                });
            }

            result.Add(new Conversation
            {
                Message = project.Title,
                CreatedAt = project.CT,
                Attachment = "ChildForConversation",
                UserId = "UserId"
            });

            foreach (var item in project.ProjectReport)
            {
                string message = null;

                if (item.Reason != ProjectReportReason.Other)
                {
                    message += item.Reason.GetDescription();

                    if (string.IsNullOrWhiteSpace(item.Description) == false)
                    {
                        message += "<br />";
                    }
                }

                if (string.IsNullOrWhiteSpace(item.Description) == false)
                {
                    message += item.Description;
                }

                result.Add(new Conversation
                {
                    Message = message,
                    CreatedAt = item.CreatedAt,
                    Attachment = "ProjectReport",
                    UserId = item.UserId,
                    User = item.User,
                    Role = Role.Designer
                });
            }

            foreach (var item in project.Child.SelectMany(x => x.ProjectReport))
            {
                string message = null;

                if (item.Reason != ProjectReportReason.Other)
                {
                    message += item.Reason.GetDescription();

                    if (string.IsNullOrWhiteSpace(item.Description) == false)
                    {
                        message += "<br />";
                    }
                }

                if (string.IsNullOrWhiteSpace(item.Description) == false)
                {
                    message += item.Description;
                }

                result.Add(new Conversation
                {
                    Message = message,
                    CreatedAt = item.CreatedAt,
                    Attachment = "ProjectReport",
                    UserId = item.UserId,
                    User = item.User,
                    Role = Role.Designer
                });
            }

            result = result.OrderBy(x => x.CreatedAt).ToList();

            ViewData["ProjectId"] = projectId;

            if (User.IsInRole(Enums.Enum.Role.Admin.ToString()))
            {
                ViewData["MyRole"] = Enums.Enum.Role.Admin.ToInt();
                ViewData["IsAdmin"] = true;

                ReadPrjectReport(project.Id);
            }
            else if (User.IsInRole(Enums.Enum.Role.Designer.ToString()))
            {
                ViewData["MyRole"] = Enums.Enum.Role.Designer.ToInt();
                ViewData["IsAdmin"] = false;
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = db.Users.Where(x => x.Id == userId).First();
            ViewData["MyId"] = userId;
            ViewData["MyName"] = user.Name;
            ViewData["MyImage"] = user.Image;

            return View("/views/shared/_ConversationPartial.cshtml", result);
        }

        private void ReadPrjectReport(int projectId)
        {
            using var db = new PuzzleDbContext();

            var projectReport = db.ProjectReport
                .Where(x => x.ProjectId == projectId || x.Project.ProjectId == projectId)
                .ToList();

            foreach (var item in projectReport)
            {
                item.ReadByAdmin = true;
            }

            db.SaveChanges();
        }

        [HttpPost("/conversation/favorite")]
        public IActionResult Favorite(int id)
        {
            using var db = new PuzzleDbContext();

            var userId = User.GetUserId();

            var conversationFavorite = db.ConversationFavorite
                .Where(x => x.ConversationId == id)
                .FirstOrDefault();

            if (conversationFavorite != null)
            {
                db.ConversationFavorite.Remove(conversationFavorite);
            }
            else
            {
                db.ConversationFavorite.Add(new ConversationFavorite
                {
                    ConversationId = id,
                    CreatedAt = DateTime.Now,
                    UserId = userId
                });
            }

            db.SaveChanges();

            return Ok();
        }
    }
}
