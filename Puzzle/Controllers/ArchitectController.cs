using Extensions;
using Helper;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Puzzle.Data;
using Puzzle.Helper;
using Puzzle.Models;
using Puzzle.ViewModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using static Enums.Enum;

namespace Controllers
{
    [TimeFilter]
    [Authorize(Roles = "Designer")]
    public class ArchitectController : Controller
    {
        private readonly ProjectRepository projectRepository;
        private readonly DepositRepository _depositRepository;
        private readonly IHubContext<SignalRHub> _signalRHub;

        public ArchitectController(IHubContext<SignalRHub> signalRHub)
        {
            projectRepository = new ProjectRepository();
            _depositRepository = new DepositRepository();
            _signalRHub = signalRHub;
        }

        #region Project

        [HttpGet("/architect/project")]
        public IActionResult Index(int? page = 1)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var result = projectRepository.GetByDesignerId(userId, PersianDateTime.Today.AddDays(-page.Value + 1), PersianDateTime.Today.AddDays(-page.Value + 1));

            result.Page = page.Value;

            ViewData["FormTitle"] = "پروژه‌ها";
            ViewData["ShowCalender"] = true;

            return View(result);
        }

        [HttpGet("/architect/project/open")]
        public IActionResult Open()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var project = projectRepository.Open(userId);

            ViewData["FormTitle"] = "تحویل نشده‌ها";

            var result = new ProjectViewModel { Project = project };

            return View("Index", result);
        }

        [HttpGet("/architect/project/no-settlement")]
        public IActionResult NoSettlement()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var project = projectRepository.NoSettlement(userId);

            ViewData["FormTitle"] = "تسویه نشده‌ها";

            var result = new ProjectViewModel { Project = project };

            return View("Index", result);
        }

        [HttpGet("/architect/favorite")]
        public IActionResult Favorite()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            using var db = new PuzzleDbContext();

            var project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.DesignerId == userId)
                .Where(x => x.Favorite.Where(a => a.UserId == userId).Any())
                .Include(c => c.Customer)
                .Include(c => c.Designer)
                //.Include(x => x.Media)
                .Include(x => x.Parent)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite).ThenInclude(x => x.User)
                .Include(x => x.ProjectReport)
                .ToList();

            ViewData["FormTitle"] = "علاقه‌مندی‌ها";

            var result = new ProjectViewModel { Project = project };

            return View("Index", result);
        }

        #endregion




        public IActionResult Wage()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var project = projectRepository.GetByDesignerId(userId, null, null);
            var deposit = _depositRepository.GetByDesignerId(userId);

            var result = new WageViewModel
            {
                Wage = project.Project.Sum(c => c.Wage) - deposit.Sum(c => c.Price),
                Deposit = deposit
            };

            return View(result);
        }





        [HttpGet("/[controller]/customer/{id}")]
        public IActionResult Customer(int id)
        {
            using var db = new PuzzleDbContext();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var customer = db.Customer
                .Where(x => x.Id == id)
                .Where(x => x.Project.Where(a => a.DesignerId == userId).Any())
                .FirstOrDefault();

            if (customer == null)
            {
                return NotFound();
            }

            ViewData["FormTitle"] = customer.LastName + " " + customer.FirstName;

            var project = projectRepository.GetByDesignerId(userId, customerId: id);

            return View("Index", project);
        }

        [HttpGet("/[controller]/media/{id}")]
        public IActionResult Media(int id)
        {
            using var db = new PuzzleDbContext();

            ViewData["ProjectId"] = id;
            ViewData["Id"] = id;
            ViewData["MyRole"] = Enums.Enum.Role.Designer.ToInt();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var user = db.Users.Where(x => x.Id == userId).First();

            ViewData["MyId"] = userId;
            ViewData["MyName"] = user.Name;
            ViewData["MyImage"] = user.Image;

            var project = db.Project
                .Where(x => x.Id == id)
                .Where(x => x.DesignerId == userId)
                .Include(x => x.Child).ThenInclude(x => x.ProjectReport).ThenInclude(x => x.User)
                .Include(x => x.ProjectReport).ThenInclude(x => x.User)
                .First();

            var childIds = project.Child.Select(x => x.Id).ToList();

            var conversation = db.Conversation
                .Where(x => x.Accepted == true || x.UserId == userId)
                .Where(x => x.ProjectId == id || childIds.Contains(x.ProjectId))
                .Include(x => x.User)
                .Include(x => x.Project).ThenInclude(x => x.Customer)
                .ToList();

            foreach (var item in project.Child)
            {
                conversation.Add(new Conversation
                {
                    Message = item.Title,
                    CreatedAt = item.CT,
                    Attachment = "ChildForConversation",
                    UserId = "UserId"
                });
            }

            conversation.Add(new Conversation
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

                conversation.Add(new Conversation
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

                conversation.Add(new Conversation
                {
                    Message = message,
                    CreatedAt = item.CreatedAt,
                    Attachment = "ProjectReport",
                    UserId = item.UserId,
                    User = item.User,
                    Role = Role.Designer
                });
            }

            conversation = conversation.OrderBy(x => x.CreatedAt).ToList();

            return View(conversation);
        }

        //[HttpPost("/[controller]/add-media")]
        //public IActionResult AddMedia(MediaViewModel model)
        //{
        //    if (model.Media == null || model.Media.Any() == false)
        //    {
        //        return Ok();
        //    }

        //    using var db = new PuzzleDbContext();

        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    var project = db.Project
        //        .Where(x => x.Id == model.ProjectId)
        //        .Where(x => x.DesignerId == userId)
        //        .Include(x => x.Customer)
        //        .Include(x => x.Designer)
        //        .FirstOrDefault();

        //    if (project == null)
        //    {
        //        return Ok();
        //    }

        //    project.MediaDone = false;

        //    foreach (var item in model.Media)
        //    {
        //        db.Media.Add(new Media
        //        {
        //            CreatedAt = DateTime.Now,
        //            ProjectId = model.ProjectId,
        //            Url = item.UploadFile(model.ProjectId),
        //            Type = model.Type,
        //            Status = Enums.Enum.MediaStatus.New,
        //            Description = model.Description
        //        });
        //    }

        //    db.SaveChanges();

        //    _signalRHub.Clients.All.SendAsync("Notification", project.Designer.Name, "پروژه آقای " + project.Customer.FirstName + " " + project.Customer.LastName + " آپدیت شد", "http://puzzlearchitect/edit/" + model.ProjectId).GetAwaiter();

        //    return Ok();
        //}

        //[HttpPost("/[controller]/remove-media")]
        //public IActionResult RemoveMedia(int id)
        //{
        //    using var db = new PuzzleDbContext();

        //    var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

        //    var media = db.Media.Where(x => x.Id == id).Where(x => x.Project.DesignerId == userId).FirstOrDefault();

        //    if (media != null)
        //    {
        //        db.Media.Remove(media);

        //        db.SaveChanges();
        //    }

        //    return Ok();
        //}

        [HttpPost("/[controller]/get-media")]
        public IActionResult GetMedia(MediaViewModel model)
        {
            using var db = new PuzzleDbContext();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //var media = db.Media
            //    .Where(x => x.ProjectId == model.ProjectId)
            //    .Where(x => x.Project.DesignerId == userId)
            //    .ToList();

            var media = new List<Media>();

            var conversation = db.Conversation
                .Where(x => x.ProjectId == model.ProjectId)
                .Where(x => x.Type == MediaType.Render || x.Type == MediaType.Screen)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            foreach (var item in conversation)
            {
                media.Add(new Media
                {
                    CreatedAt = item.CreatedAt,
                    Url = item.Attachment,
                    Type = item.Type.Value,
                    Description = item.Message ?? "Conversation"
                });
            }

            media = media.OrderByDescending(x => x.CreatedAt).ToList();

            return PartialView("_MediaPartial", media);
        }

        public IActionResult Search(string Keyword, int Page = 1)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var X = new CustomerRepository().Search(Keyword, Page, designerId: userId);

            if (X == null)
            {
                return NotFound();
            }
            return View(X);
        }

        [HttpPost("/[controller]/Conversation/Read")]
        public IActionResult ReadConversation(int projectId)
        {
            using var db = new PuzzleDbContext();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var conversation = db.Conversation
                .Where(x => x.Project.DesignerId == userId)
                .Where(x => x.ProjectId == projectId).Where(x => x.UnreadByDesigner == true)
                .ToList();

            if (conversation.Any())
            {
                foreach (var item in conversation)
                {
                    item.UnreadByDesigner = false;
                }

                db.SaveChanges();
            }

            return Ok();
        }

        [HttpGet("/[controller]/timeline")]
        public IActionResult Timeline(int? page = 1)
        {
            using var db = new PuzzleDbContext();

            var result = new ProjectViewModel { Page = page.Value };

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var query = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.DesignerId == userId)
                .Where(x => x.Conversation.Where(x => x.Accepted || x.UserId == userId).Any())
                .Where(x => x.DoneByDesigner == false || x.Conversation.Where(x => x.UnreadByDesigner).Any());

            result.TotalPage = Math.Ceiling(query.Count() / 30.0).ToInt();

            result.Project = query.OrderByDescending(x => x.Conversation.OrderByDescending(a => a.CreatedAt).First().CreatedAt)
                .Skip((page.Value - 1) * 30)
                .Take(30)
                .Include(x => x.Customer)
                .Include(x => x.Designer)
                .Include(x => x.Parent)
                .Include(x => x.Child)
                //.Include(x => x.Media)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport)
                .ToList();

            ViewData["FormTitle"] = "تایم‌لاین";

            return View("Index", result);
        }

        [HttpPost("/[controller]/done")]
        public IActionResult Done(int projectId)
        {
            using var db = new PuzzleDbContext();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var project = db.Project
                .Where(x => x.DesignerId == userId)
                .Where(x => x.Id == projectId)
                .First();

            project.DoneByDesigner = !project.DoneByDesigner;

            db.SaveChanges();

            return Ok();
        }

        [HttpPost("/[controller]/favorite")]
        public IActionResult Favorite(int id)
        {
            var userId = User.GetUserId();

            projectRepository.Favorite(id, userId);

            return Ok();
        }

        [HttpPost("/[controller]/add-report")]
        public IActionResult AddReport(int projectId, string description, ProjectReportReason reason)
        {
            using var db = new PuzzleDbContext();

            var userId = User.GetUserId();

            var project = db.Project
                .Where(x => x.Id == projectId)
                .Where(x => x.DesignerId == userId)
                .Where(x => x.IsDelete == false)
                .FirstOrDefault();

            if (project != null)
            {
                db.ProjectReport.Add(new ProjectReport
                {
                    CreatedAt = DateTime.Now,
                    ProjectId = projectId,
                    UserId = userId,
                    Description = description,
                    Reason = reason
                });

                db.SaveChanges();
            }

            return Ok();
        }

        [HttpGet("/[controller]/accounting")]
        public IActionResult Accounting()
        {
            var userId = User.GetUserId();

            using var db = new PuzzleDbContext();

            var projectAccounting = db.ProjectAccounting.Where(x => x.DesignerId == userId).ToList();

            ViewData["Project"] = projectRepository.GetByDesignerId(userId, null, null);
            ViewData["Deposit"] = _depositRepository.GetByDesignerId(userId);

            return View(projectAccounting);
        }
    }
}