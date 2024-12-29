using Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Puzzle;
using Puzzle.Data;
using Puzzle.Helper;
using Puzzle.Models;
using Repository;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewModels;

namespace Controllers
{
    [UpdateLastActivity]
    public class MyController : Controller
    {
        public IActionResult Index()
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Redirect("/my/login");
            }
            else
            {
                var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

                ViewData["MyName"] = customer.FirstName + " " + customer.LastName;

                ViewData["Accounting"] = customer.Balance;

                return Redirect("/my/project");
            }
        }

        [HttpGet("/my/conversation/{id}")]
        public IActionResult Conversation(int id)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Redirect("/my/login");
            }

            ViewData["MyRole"] = Enums.Enum.Role.Customer.ToInt();
            ViewData["MyId"] = token.CustomerId;

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            ViewData["MyName"] = customer.FirstName + " " + customer.LastName;
            ViewData["Accounting"] = token.Customer.Balance;

            ViewData["ProjectId"] = id;

            var project = db.Project
                .Where(x => x.Id == id)
                .Where(x => x.CustomerId == token.CustomerId)
                .Include(x => x.Customer)
                .Include(x => x.Conversation).ThenInclude(x => x.Parent)
                .Include(x => x.Payment)
                .Include(x => x.Designer)
                .Include(x => x.Child).ThenInclude(x => x.Conversation).ThenInclude(x => x.Parent)
                .Include(x => x.Child).ThenInclude(x => x.Payment)
                .First();

            ViewData["DesignerId"] = project.DesignerId;

            var conversation = project.Conversation
                .Where(x => x.IsDelete == false)
                //.Where(x => x.UserId == null || x.Accepted == true ||
                //    (x.Type == Enums.Enum.MediaType.Render && (x.Project.App || x.Project.Whatsapp || x.Project.Telegram || x.Project.PrintDelivery || x.Project.Eitaa || x.Project.Soroush || x.Project.Rubika || x.Project.Other || x.Project.Flash)) ||
                //    (x.Type == Enums.Enum.MediaType.Screen && (x.Project.Screenshot || x.Project.App || x.Project.Whatsapp || x.Project.Telegram || x.Project.PrintDelivery || x.Project.Eitaa || x.Project.Soroush || x.Project.Rubika || x.Project.Other || x.Project.Flash)))
                .ToList();

            var childConversation = project.Child.SelectMany(x => x.Conversation.Where(x => x.IsDelete == false)
                 //.Where(x => x.UserId == null || x.Accepted == true ||
                 //   (x.Type == Enums.Enum.MediaType.Render && (x.Project.App || x.Project.Whatsapp || x.Project.Telegram || x.Project.PrintDelivery || x.Project.Eitaa || x.Project.Soroush || x.Project.Rubika || x.Project.Other || x.Project.Flash)) ||
                 //   (x.Type == Enums.Enum.MediaType.Screen && (x.Project.Screenshot || x.Project.App || x.Project.Whatsapp || x.Project.Telegram || x.Project.PrintDelivery || x.Project.Eitaa || x.Project.Soroush || x.Project.Rubika || x.Project.Other || x.Project.Flash))))
                 )
                .ToList();

            conversation.AddRange(childConversation);

            foreach (var item in project.Child.Where(x => x.IsDelete == false))
            {
                conversation.Add(new Conversation
                {
                    Message = item.Title,
                    CreatedAt = item.CT,
                    Attachment = "ChildForConversation",
                    UserId = "UserId",
                    Accepted = true
                });
            }

            project.Conversation = conversation.OrderBy(x => x.CreatedAt).ToList();

            Read(id);

            return View(project);
        }

        public IActionResult Login()
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token != null && key != null)
            {
                return Redirect("/my/project");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            model.Mobile = model.Mobile.ToEnNumber();
            model.Mobile = "0" + model.Mobile.Substring(model.Mobile.Length - 10, 10);

            using var db = new PuzzleDbContext();

            var customer = db.Customer
                .Where(x => x.Mobile == model.Mobile)
                .FirstOrDefault();

            if (customer == null)
            {
                customer = new Customer { Mobile = model.Mobile };

                db.Customer.Add(customer);

                db.SaveChanges();
            }

            var token = db.Token
                .Where(x => x.CustomerId == customer.Id)
                .Where(x => x.CreatedAt >= DateTime.Now.AddSeconds(-30))
                .Any();

            if (token == false)
            {
                int code = new Random().Next(1000, 10000);

                if (customer.Id == 5026)
                {
                    code = 2023;
                }

                if (Setting.IsDevelopment)
                {
                    code = 1234;
                }
                else
                {
                    new SmsService().Send(model.Mobile, Enums.Enum.SmsTemplate.PuzzleAutoVerify, code.ToString(), "pOwmg5Kl", null);
                }

                db.Token.Add(new Token
                {
                    Code = code,
                    CreatedAt = DateTime.Now,
                    CustomerId = customer.Id
                });

                db.SaveChanges();
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult Verify(LoginModel model)
        {
            model.Mobile = model.Mobile.ToEnNumber();
            model.Mobile = "0" + model.Mobile.Substring(model.Mobile.Length - 10, 10);

            model.Code = model.Code.ToEnNumber();

            using var db = new PuzzleDbContext();

            var customer = db.Customer
                .Where(x => x.Mobile == model.Mobile)
                .FirstOrDefault();

            if (customer != null)
            {
                var token = db.Token
                    .Where(x => x.CustomerId == customer.Id)
                    .Where(x => x.CreatedAt >= DateTime.Now.AddSeconds(-180))
                    .Where(x => x.Code == model.Code.ToInt())
                    .FirstOrDefault();

                if (token != null)
                {
                    token.Key = Guid.NewGuid().ToString();

                    db.SaveChanges();

                    HttpContext.Response.Cookies.Append("authorisation", token.Key, new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(365)
                    });

                    return Ok(customer);
                }
            }

            return NotFound();
        }

        [HttpGet("/my/user/details")]
        public IActionResult UserDetails(UpdateInfoModel model)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Redirect("/my/login");
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId)
                .Select(x => new UpdateInfoModel
                {
                    City = x.City,
                    FirstName = x.FirstName,
                    LastName = x.LastName
                })
                .First();

            return View(customer);
        }

        [HttpPost("/my/user/update")]
        public IActionResult UserUpdate(UpdateInfoModel model)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Redirect("/my/login");
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.City = model.City;

            db.SaveChanges();

            return Ok();
        }

        public IActionResult Project()
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Redirect("/my/login");
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            if (string.IsNullOrWhiteSpace(customer.FirstName) || string.IsNullOrWhiteSpace(customer.LastName))
            {
                return Redirect("/my/user/details");
            }

            ViewData["MyName"] = customer.FirstName + " " + customer.LastName;
            ViewData["Accounting"] = customer.Balance;

            var project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null)
                .Where(x => x.CustomerId == token.CustomerId)
                .OrderByDescending(x => x.Child.Any() ? (x.Child.OrderByDescending(x => x.Id).First().Conversation.Any() ? x.Child.OrderByDescending(x => x.Id).First().Conversation.OrderByDescending(a => a.CreatedAt).First().CreatedAt : x.Child.OrderByDescending(a => a.Id).First().CT) : (x.Conversation.Any() ? x.Conversation.OrderByDescending(a => a.CreatedAt).First().CreatedAt : x.CT))
                //.OrderByDescending(x => x.Receipt).ThenByDescending(x => x.Id)
                .Take(20)
                //.Include(x => x.Media)
                .Include(x => x.Conversation)
                .Include(x => x.Child)
                .ToList();

            if (customer.PushSms == true)
            {
                HttpContext.Response.Cookies.Append("notfi-confirm-status", "true", new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(365)
                });
            }

            return View(project);
        }

        [HttpPost]
        public IActionResult Project(int page)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Redirect("/my/login");
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            var project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null)
                .Where(x => x.CustomerId == token.CustomerId)
                .OrderByDescending(x => x.Child.Any() ? (x.Child.OrderByDescending(x => x.Id).First().Conversation.Any() ? x.Child.OrderByDescending(x => x.Id).First().Conversation.OrderByDescending(a => a.CreatedAt).First().CreatedAt : x.Child.OrderByDescending(a => a.Id).First().CT) : (x.Conversation.Any() ? x.Conversation.OrderByDescending(a => a.CreatedAt).First().CreatedAt : x.CT))
                //.OrderByDescending(x => x.Receipt).ThenByDescending(x => x.Id)
                .Skip((page - 1) * 20)
                .Take(20)
                //.Include(x => x.Media)
                .Include(x => x.Conversation)
                .Include(x => x.Child)
                .ToList();

            if (project.Count == 0)
            {
                return NoContent();
            }

            return View("_MyProjectPartial", project);
        }

        [HttpGet("/my/projectdetails/{id}")]
        public IActionResult ProjectDetails(int id)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Redirect("/my/login");
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            ViewData["MyName"] = customer.FirstName + " " + customer.LastName;
            ViewData["Accounting"] = customer.Balance;

            var childIds = db.Project.Where(x => x.ProjectId == id).Select(x => x.Id).ToList();

            var conversation = db.Conversation
                .Where(x => x.ProjectId == id || childIds.Contains(x.ProjectId))
                .Where(x => x.Project.CustomerId == token.CustomerId)
                .Where(x => x.IsDelete == false)
                .Where(x => x.Type == Enums.Enum.MediaType.Render || x.Type == Enums.Enum.MediaType.Screen)
                .Include(x => x.Project)
                //.Where(x => x.Accepted == true ||
                //    (x.Type == Enums.Enum.MediaType.Render && (x.Project.App || x.Project.Whatsapp || x.Project.Telegram || x.Project.PrintDelivery || x.Project.Eitaa || x.Project.Soroush || x.Project.Rubika || x.Project.Other || x.Project.Flash)) ||
                //    (x.Type == Enums.Enum.MediaType.Screen && (x.Project.Screenshot || x.Project.App || x.Project.Whatsapp || x.Project.Telegram || x.Project.PrintDelivery || x.Project.Eitaa || x.Project.Soroush || x.Project.Rubika || x.Project.Other || x.Project.Flash)))
                .ToList();

            //var media = db.Media
            //    .Where(x => x.ProjectId == id)
            //    .Where(x => x.Project.CustomerId == token.CustomerId)
            //    .ToList();

            var media = new List<Media>();

            foreach (var item in conversation)
            {
                var totalPrice = item.Project.TotalPrice > 0 ? item.Project.TotalPrice : item.Project.Approximate;
                var orginal = item.Project.TotalPayment >= totalPrice;
                orginal = orginal || item.Project.App || item.Project.Whatsapp || item.Project.Telegram || item.Project.PrintDelivery || item.Project.Eitaa || item.Project.Soroush || item.Project.Rubika || item.Project.Other || item.Project.Flash;

                Enums.Enum.MediaStatus status = Enums.Enum.MediaStatus.Read;

                if (item.Type == Enums.Enum.MediaType.Render && (orginal == false))
                {
                    status = Enums.Enum.MediaStatus.New;
                }
                else if (item.Type == Enums.Enum.MediaType.Screen && ((orginal || item.Project.Screenshot) == false))
                {
                    status = Enums.Enum.MediaStatus.New;
                }

                media.Add(new Media
                {
                    CreatedAt = item.CreatedAt,
                    Url = item.Attachment,
                    Type = item.Type.Value,
                    Description = item.Project.Title,
                    Status = status,
                    ProjectId = item.ProjectId
                });
            }

            return View(media);
        }

        private void Read(int projectId)
        {
            using var db = new PuzzleDbContext();

            Func<Conversation, bool> showOrHide = x => x.Accepted == true ||
            (x.Type == Enums.Enum.MediaType.Render && (x.Project.App || x.Project.Whatsapp || x.Project.Telegram || x.Project.PrintDelivery || x.Project.Eitaa || x.Project.Soroush || x.Project.Rubika || x.Project.Other || x.Project.Flash)) ||
            (x.Type == Enums.Enum.MediaType.Screen && x.Project.Screenshot);

            var conversation = db.Conversation
                .Where(x => x.ProjectId == projectId)
                .Where(x => x.IsDelete == false && (x.Accepted == true ||
                (x.Type == Enums.Enum.MediaType.Render && (x.Project.App || x.Project.Whatsapp || x.Project.Telegram || x.Project.PrintDelivery || x.Project.Eitaa || x.Project.Soroush || x.Project.Rubika || x.Project.Other || x.Project.Flash)) ||
                (x.Type == Enums.Enum.MediaType.Screen && x.Project.Screenshot)))
                .Where(x => x.UnreadByCustomer == true)
                .ToList();

            if (conversation.Any())
            {
                foreach (var item in conversation)
                {
                    item.UnreadByCustomer = false;
                }

                db.SaveChanges();
            }
        }

        [HttpGet("/my/project/add")]
        public IActionResult AddProject()
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Redirect("/my/login");
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            ViewData["MyName"] = customer.FirstName + " " + customer.LastName;

            return View();
        }

        [HttpPost("/my/project/add")]
        public IActionResult AddProject(AddProjectModel model)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            var project = new Project
            {
                Title = model.Title,
                CustomerId = token.CustomerId,
                Receipt = DateTime.Now,
                CT = DateTime.Now,
                Status = Enums.Enum.Status.New,
                ProjectCategory = model.Category,
                OrderType = Enums.Enum.OrderType.App,
                Type = model.Quality
            };

            db.Project.Add(project);

            db.SaveChanges();

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            SMSHelper.SendByPattern(customer.Mobile, customer.FirstName, "oq12gjy6limvfja");

            return Ok(new { project.Id });
        }

        [HttpPost("/my/conversation/add")]
        public IActionResult AddConversation(ConversationViewModel.Add model)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            if (model.ConversationId != null)
            {
                var hasAny = db.Conversation
                    .Where(x => x.Id == model.ConversationId)
                    .Where(x => (x.ProjectId == model.ProjectId) || (x.Project.ProjectId == model.ProjectId))
                    .Any();

                if (hasAny == false)
                {
                    return NotFound();
                }
            }

            var project = db.Project
                .Where(x => x.CustomerId == token.CustomerId)
                .Where(x => x.Id == model.ProjectId)
                .Include(x => x.Child)
                .First();

            project.MediaDone = false;
            project.DoneByDesigner = false;

            var projectId = model.ProjectId;

            if (project.Child.Any())
            {
                projectId = project.Child.OrderByDescending(x => x.Id).First().Id;
            }

            var conversation = new Conversation
            {
                CreatedAt = DateTime.Now,
                Message = model.Message,
                ProjectId = projectId,
                Role = Enums.Enum.Role.Customer,
                UnreadByAdmin = true,
                UnreadByDesigner = true,
                ConversationId = model.ConversationId
            };

            conversation.Accepted = true;

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

        [HttpPost("/my/conversation/update")]
        public IActionResult UpdateConversation(ConversationViewModel.Add model)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            var conversation = db.Conversation
                .Where(x => x.Project.CustomerId == token.CustomerId)
                .Where(x => x.Id == model.Id)
                .Where(x => x.IsDelete == false)
                .Include(x => x.Project)
                .First();

            conversation.Project.MediaDone = false;
            conversation.Project.DoneByDesigner = false;


            conversation.UnreadByAdmin = true;
            conversation.UnreadByDesigner = true;

            conversation.Message = model.Message;

            conversation.Accepted = true;

            db.SaveChanges();

            return Ok(conversation);
        }

        [HttpPost("/my/conversation/remove")]
        public IActionResult RemoveConversation(ConversationViewModel.Add model)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            var conversation = db.Conversation
                .Where(x => x.Project.CustomerId == token.CustomerId)
                .Where(x => x.UserId == null)
                .Where(x => x.Id == model.Id)
                .First();

            conversation.IsDelete = true;

            db.SaveChanges();

            return Ok();
        }

        [HttpPost("/my/pushsms/status")]
        public IActionResult ActivePushSms(bool status)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            customer.PushSms = status;

            db.SaveChanges();

            return Ok();
        }

        [HttpPost("/my/project/repair/add")]
        public IActionResult AddRepair(int projectId, Enums.Enum.ProjectCategory projectCategory)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            var parent = db.Project
                .Where(x => x.Id == projectId)
                .Where(x => x.ProjectId == null)
                .Include(x => x.Child)
                .Include(x => x.Customer)
                .First();

            var project = new Project
            {
                ProjectId = projectId,
                CT = DateTime.Now,

                CustomerId = parent.CustomerId,

                OrderType = Enums.Enum.OrderType.App,
                ProjectCategory = projectCategory,
                Status = Enums.Enum.Status.New,

                Receipt = DateTime.Today,

                Delivery = DateTime.Today.AddDays(1)
            };

            var count = parent.Child.Count;

            project.Title = $"متمم {count + 1}";

            db.Project.Add(project);

            db.SaveChanges();

            new ProjectRepository().UpdateChild(project.ProjectId.Value);

            new CustomerRepository().Balance(project.CustomerId);

            return Ok();
        }

        [HttpGet("/my/payment")]
        public IActionResult Payment()
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            var customerChat = db.CustomerChat
                .Where(x => x.CustomerId == token.CustomerId)
                .Where(x => x.IsDelete == false)
                .ToList();

            foreach (var item in customerChat)
            {
                item.UnreadByCustomer = false;
            }

            db.SaveChanges();

            ViewData["Accounting"] = customer.Balance;

            return View(customerChat);
        }

        [HttpPost("/my/payment")]
        public IActionResult Payment(PaymentModel model)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            customer.DoneByAdmin = false;

            var customerChat = new CustomerChat
            {
                Attachment = model.Attachment.UploadFile("customerchat/" + token.CustomerId),
                CreatedAt = DateTime.Now,
                CustomerId = token.CustomerId,
                UnreadByAdmin = true
            };

            db.CustomerChat.Add(customerChat);

            db.SaveChanges();

            return Ok();
        }

        [HttpPost("/my/payment/badge")]
        public IActionResult PaymentBadge(PaymentModel model)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            var customerChat = db.CustomerChat
                .Where(x => x.CustomerId == token.CustomerId)
                .Where(x => x.IsDelete == false)
                .Where(x => x.UnreadByCustomer == true)
                .Any();

            return Ok(new { Badge = customerChat });
        }

        [HttpPost("/my/add-fcmtoken")]
        public IActionResult AddFcmToken(string firbase)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            if (db.Firebase.Where(x => x.Token == firbase).Any() == false)
            {
                db.Firebase.Add(new Firebase
                {
                    CreatedAt = DateTime.Now,
                    CustomerId = token.CustomerId,
                    Token = firbase
                });

                db.SaveChanges();
            }

            return Ok();
        }

        [HttpGet("/my/gallery")]
        public IActionResult Gallery()
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            ViewData["MyName"] = customer.FirstName + " " + customer.LastName;
            ViewData["Accounting"] = customer.Balance;

            return View();
        }

        [HttpGet("/my/gallerysearch/{category}")]
        public IActionResult GallerySearch(string category, Enums.Enum.Quality? quality)
        {
            using var db = new PuzzleDbContext();

            var key = HttpContext.Request.Cookies["authorisation"];

            var token = db.Token.Where(x => x.Key == key).FirstOrDefault();

            if (token == null || key == null)
            {
                return Unauthorized();
            }

            var customer = db.Customer.Where(x => x.Id == token.CustomerId).First();

            ViewData["MyName"] = customer.FirstName + " " + customer.LastName;
            ViewData["Accounting"] = customer.Balance;

            var query = db.Project
                .Where(x => x.Favorite.Any())
                .Where(x => x.Conversation.Where(a => a.ConversationFavorite.Any()).Any())
                .Where(x => quality == null || x.Type == quality);

            if (Enum.TryParse(category, out Enums.Enum.ProjectCategory categoryId))
            {
                query = query.Where(x => x.ProjectCategory == categoryId);
            }
            else
            {
                if (category == "cabinet")
                {
                    query = query.Where(x => x.ProjectCategory == Enums.Enum.ProjectCategory.Cabinet_Modern || x.ProjectCategory == Enums.Enum.ProjectCategory.Cabinet_Classic ||
                    x.ProjectCategory == Enums.Enum.ProjectCategory.Cabinet_NeoClassic || x.ProjectCategory == Enums.Enum.ProjectCategory.Cabinet_Enzo ||
                    x.ProjectCategory == Enums.Enum.ProjectCategory.Cabinet_Other ||
                    x.ProjectCategory == Enums.Enum.ProjectCategory.ModernCabinet || x.ProjectCategory == Enums.Enum.ProjectCategory.ClassicCabinet ||
                    x.ProjectCategory == Enums.Enum.ProjectCategory.ModernVray || x.ProjectCategory == Enums.Enum.ProjectCategory.ClassicVray);

                    ViewData["Title"] = "کابینت";
                }
                else if (category == "decor")
                {
                    query = query.Where(x => x.ProjectCategory == Enums.Enum.ProjectCategory.Decor_Closet || x.ProjectCategory == Enums.Enum.ProjectCategory.Decor_Full ||
                    x.ProjectCategory == Enums.Enum.ProjectCategory.Decor_Other ||
                    x.ProjectCategory == Enums.Enum.ProjectCategory.Decor || x.ProjectCategory == Enums.Enum.ProjectCategory.DecorVray);

                    ViewData["Title"] = "دکوراسیون داخلی";
                }
                else if (category == "official")
                {
                    query = query.Where(x => x.ProjectCategory == Enums.Enum.ProjectCategory.Official);

                    ViewData["Title"] = "فروشگاهی و اداری";
                }
                else if (category == "area")
                {
                    query = query.Where(x => x.ProjectCategory == Enums.Enum.ProjectCategory.Facade);

                    ViewData["Title"] = "محوطه و نمای ساختمان";
                }
            }

            var project = query
                .Include(x => x.Designer)
                .Include(x => x.Conversation).ThenInclude(x => x.ConversationFavorite)
                .ToList();

            var result = project.SelectMany(x => x.Conversation.SelectMany(a => a.ConversationFavorite).ToList()).ToList();

            return View(result);
        }

        public class LoginModel
        {
            public string Mobile { get; set; }

            public string Code { get; set; }
        }

        public class AddProjectModel
        {
            public string Title { get; set; }

            public Enums.Enum.Quality? Quality { get; set; }

            public Enums.Enum.ProjectCategory Category { get; set; }
        }

        public class UpdateInfoModel
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string City { get; set; }
        }

        public class PaymentModel
        {
            public IFormFile Attachment { get; set; }

            public int CustomerId { get; set; }
        }
    }
}
