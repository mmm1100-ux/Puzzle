using Extensions;
using Helper;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models;
using Puzzle;
using Puzzle.Data;
using Puzzle.Extensions;
using Puzzle.Helper;
using Puzzle.Models;
using Puzzle.ViewModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using ViewModels;
using static Enums.Enum;

namespace Controllers
{
    [TimeFilter]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly CustomerRepository _customerRepository;
        private readonly ProjectRepository _projectRepository;
        private readonly DesignerRepository _designerRepository;
        private readonly IHubContext<SignalRHub> _signalRHub;

        public HomeController(IHubContext<SignalRHub> signalRHub)
        {
            _customerRepository = new CustomerRepository();
            _projectRepository = new ProjectRepository();
            _designerRepository = new DesignerRepository();
            _signalRHub = signalRHub;
        }

        public IActionResult Index()
        {
            ViewData["Designer"] = _designerRepository.GetList(true);
            ViewData["Number"] = _projectRepository.GetTodayCount();

            return View();
        }

        [HttpPost]
        public int Index(ProjectAddViewModel C, string Delivery, string Receipt, string Birthday)
        {
            if (Receipt != null)
            {
                C.Receipt = PersianDateTime.Parse(Receipt);
            }

            if (Delivery != null)
            {
                C.Delivery = PersianDateTime.Parse(Delivery);
            }

            if (Birthday != null)
            {
                C.Birthday = PersianDateTime.Parse(Birthday);
            }

            C.Mobile = C.Mobile.ToEnNumber();

            var project = new Project()
            {
                CB = User.GetUserId(),
                CT = DateTime.Now,
                Receipt = C.Receipt.HasValue ? C.Receipt.Value.ToDateTime() : DateTime.Today,
                Delivery = C.Delivery?.ToDateTime().Date.AddHours(C.DeliveryTime),
                ProjectCategory = C.ProjectCategory,
                Description = C.Description,
                DesignerId = C.DesignerId,
                Status = C.Status,
                TotalPrice = C.TotalPrice == null ? 0 : C.TotalPrice.Replace(",", null).ToInt(),
                OrderType = C.OrderType,
                Approximate = C.Approximate == null ? 0 : C.Approximate.Replace(",", null).ToInt(),
                Print = C.Print,
                Type = C.Type,
                Telegram = false,
                Whatsapp = false,
                Other = false,
                Screenshot = false,
                Ready = false,
                Notification = false,
                Payment = new List<Payment>()
            };

            var customer = _customerRepository.GetByMobile(C.Mobile);

            for (int i = 0; i < C.PaymentType.Length; i++)
            {
                if (C.PaymentPrice[i] != null && C.PaymentPrice[i].Replace(",", null).ToInt() > 0 &&
                    (string.IsNullOrEmpty(C.PaymentDate[i]) || PersianDateTime.Parse(C.PaymentDate[i]).ToDateTime().Date == DateTime.Today))
                {
                    var payment = new Payment
                    {
                        CreatedAt = string.IsNullOrEmpty(C.PaymentDate[i]) ? DateTime.Today : PersianDateTime.Parse(C.PaymentDate[i]).ToDateTime(),
                        Price = C.PaymentPrice[i].Replace(",", null).ToInt(),
                        PaymentType = C.PaymentType[i].Value,
                        DesignerId = C.PaymentType[i].Value == PaymentType.ToDesigner ? C.PaymentDesignerId[i] : null,
                        Description = C.PaymentDescription[i]
                    };

                    //payment.CreatedAt = payment.CreatedAt.AddHours(C.PaymentTime[i].ToInt());

                    project.Payment.Add(payment);
                }
            }

            var positive = project.Payment
                .Where(c => c.PaymentType != PaymentType.Return)
                .Where(c => c.PaymentType != PaymentType.ToCredit)
                .Sum(c => c.Price);

            var negative = project.Payment
                .Where(c => c.PaymentType == PaymentType.Return || c.PaymentType == PaymentType.ToCredit)
                .Sum(c => c.Price);

            var fromCredit = project.Payment.Where(c => c.PaymentType == PaymentType.FromCredit).Sum(c => c.Price);

            if (fromCredit > 0)
            {
                var credit = customer == null ? 0 : customer.Credit;

                if (fromCredit > credit)
                {
                    return 4;
                }
            }

            project.TotalPayment = positive - negative;

            if (project.Print)
            {
                project.PrintPrice = C.PrintPrice ?? Setting.Print(DateTime.Today);
            }

            if (project.DesignerId != null)
            {
                project.DesignerPercent = _designerRepository.GetPercent(project.DesignerId, project.CT);

                int max = project.TotalPrice > 0 ? project.TotalPrice : project.Approximate;

                int print = project.PrintPrice ?? 0;

                if (project.TotalPayment < max)
                {
                    project.Wage = project.DesignerPercent * (project.TotalPayment - print) / 100.0;
                }
                else
                {
                    project.Wage = project.DesignerPercent * (max - print) / 100.0;
                }

                project.AssignedToDesignerAt = DateTime.Now;
            }
            else
            {
                project.Wage = 0;
            }

            if (project.TotalPayment < 0)
            {
                return 6;
            }

            if (customer != null)
            {
                customer.Birthday = C.Birthday?.ToDateTime();
                customer.Phone = C.Phone;
                //customer.Status = C.CustomerStatus;
                //customer.Description = C.CustomerDescription;
                project.Customer = customer;
            }
            else
            {
                project.Customer = new Customer()
                {
                    Address = C.Address,
                    Birthday = C.Birthday?.ToDateTime(),
                    Mobile = "0" + C.Mobile.Substring(C.Mobile.Length - 10, 10),
                    FirstName = C.FirstName,
                    LastName = C.LastName,
                    Phone = C.Phone,
                    Social = C.Social,
                    IsSpecial = false,
                    //Status = C.CustomerStatus,
                    //Description = C.CustomerDescription
                };

                if (!string.IsNullOrEmpty(C.Presenter))
                {
                    var presenter = _customerRepository.GetByMobile(C.Presenter);

                    if (presenter == null)
                    {
                        return 1;
                    }
                    else
                    {
                        project.Presenter = new Presenter { CustomerId = presenter.Id };
                    }
                }

                SMSHelper.SendByPattern(project.Customer.Mobile, project.Customer.FirstName + " " + project.Customer.LastName, "t171mukg0g");
            }

            if (string.IsNullOrWhiteSpace(C.Title))
            {
                var db = new PuzzleDbContext();

                var count = db.Project.Where(x => x.CustomerId == project.Customer.Id).Count();

                project.Title = $"پروژه شماره {count + 1}";

                if (project.ProjectCategory == ProjectCategory.Print)
                {
                    count = db.Project.Where(x => x.CustomerId == project.Customer.Id).Where(x => x.ProjectCategory == ProjectCategory.Print).Count();

                    project.Title = $"پرینت شماره {count + 1}";
                }
            }
            else
            {
                project.Title = C.Title;
            }

            #region ProjectAccounting

            project.ProjectAccounting = new List<ProjectAccounting>
            {
                new ProjectAccounting
                {
                    Approximate = project.Approximate,
                    CreatedAt = DateTime.Now,
                    DesignerPercent = project.DesignerPercent,
                    PrintPrice = project.PrintPrice,
                    TotalPayment = project.TotalPayment,
                    TotalPrice = project.TotalPrice,
                    DesignerId = project.DesignerId,
                    Wage = project.Wage
                }
            };

            #endregion

            _projectRepository.Add(project);

            _customerRepository.Balance(project.CustomerId);

            return 2;
        }

        public JsonResult FindByMobile(string Mobile)
        {
            var X = _customerRepository.GetByMobile(Mobile.ToEnNumber());

            if (X != null)
            {
                return Json(new
                {
                    X.Address,
                    Birthday = X.Birthday?.ToShamsi().ToShortDateString(),
                    X.Mobile,
                    X.Id,
                    X.FirstName,
                    X.LastName,
                    X.Phone,
                    X.Social,
                    X.Balance,
                    Duplicate = _customerRepository.Duplicate(X.Id),
                    X.Credit
                });
            }

            return Json(null);
        }

        [HttpPost]
        public bool Description(string description, int id)
        {
            using var db = new PuzzleDbContext();

            var project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.Id == id)
                .First();

            project.Description = description;

            db.SaveChanges();

            return true;
        }

        [HttpPost]
        public bool Delivery(int projectId, DeliveryType deliveryType)
        {
            using var db = new PuzzleDbContext();

            var project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.Id == projectId)
                .First();

            if (deliveryType == DeliveryType.Print)
            {
                project.PrintDelivery = !project.PrintDelivery;
            }
            else if (deliveryType == DeliveryType.Telegram)
            {
                project.Telegram = !project.Telegram;
            }
            else if (deliveryType == DeliveryType.Whatsapp)
            {
                project.Whatsapp = !project.Whatsapp;
            }
            else if (deliveryType == DeliveryType.Other)
            {
                project.Other = !project.Other;
            }
            else if (deliveryType == DeliveryType.Screenshot)
            {
                project.Screenshot = !project.Screenshot;
            }
            else if (deliveryType == DeliveryType.Ready)
            {
                project.Ready = !project.Ready;
            }
            else if (deliveryType == DeliveryType.Notification)
            {
                project.Notification = !project.Notification;
            }
            else if (deliveryType == DeliveryType.Follow)
            {
                project.Follow = !project.Follow;
            }
            //else if (deliveryType == DeliveryType.Soroush)
            //{
            //    project.Soroush = !project.Soroush;
            //}
            else if (deliveryType == DeliveryType.Eitaa)
            {
                project.Eitaa = !project.Eitaa;
            }
            //else if (deliveryType == DeliveryType.Rubika)
            //{
            //    project.Rubika = !project.Rubika;
            //}
            else if (deliveryType == DeliveryType.MediaDone)
            {
                project.MediaDone = !project.MediaDone;
            }
            else if (deliveryType == DeliveryType.Cancel)
            {
                project.Cancel = !project.Cancel;
            }
            else if (deliveryType == DeliveryType.Flash)
            {
                project.Flash = !project.Flash;
            }
            else if (deliveryType == DeliveryType.App)
            {
                project.App = !project.App;
            }
            else if (deliveryType == DeliveryType.Archive)
            {
                project.Archive = !project.Archive;
            }

            db.SaveChanges();

            return true;
        }

        [HttpPost]
        public IActionResult Date(string date)
        {
            int day = (DateTime.Today - PersianDateTime.Parse(date).ToDateTime()).Days;

            return RedirectToAction(nameof(Project), new { page = day + 1 });
        }

        [HttpGet("/Edit/{Id}")]
        public IActionResult Edit(int Id)
        {
            var X = _projectRepository.GetById(Id);

            if (X.ProjectId != null)
            {
                return NotFound();
            }

            var M = new CustomerViewModel()
            {
                Address = X.Customer.Address,
                Birthday = X.Customer.Birthday?.ToShamsi(),
                Description = X.Description,
                Id = X.Id,
                Mobile = X.Customer.Mobile,
                FirstName = X.Customer.FirstName,
                LastName = X.Customer.LastName,
                Phone = X.Customer.Phone,
                Social = X.Customer.Social,
                ProjectCategory = X.ProjectCategory,
                Delivery = X.Delivery?.ToShamsi(),
                Receipt = X.Receipt.ToShamsi(),
                Survey = X.Survey?.Description,
                DesignerId = X.DesignerId,
                OrderType = X.OrderType,
                TotalPrice = X.TotalPrice,
                Status = X.Status,
                Payment = X.Payment.ToList(),
                Presenter = X.Presenter?.Customer.Mobile,
                Approximate = X.Approximate,
                Print = X.Print,
                Telegram = X.Telegram,
                Whatsapp = X.Whatsapp,
                Other = X.Other,
                Screenshot = X.Screenshot,
                Ready = X.Ready,
                Notification = X.Notification,
                Credit = X.Customer.Credit,
                Balance = X.Customer.Balance,
                DesignerPercent = X.DesignerPercent,
                //Media = X.Media.ToList(),
                Title = X.Title,
                Cancel = X.Cancel,
                Type = X.Type,
                PrintDelivery = X.PrintDelivery,
                Flash = X.Flash,
                Eitaa = X.Eitaa,
                App = X.App,
                Follow = X.Follow,
                Archive = X.Archive,
                PrintPrice = X.PrintPrice
            };

            using var db = new PuzzleDbContext();

            ViewData["ProjectId"] = Id;
            ViewData["MyRole"] = Enums.Enum.Role.Admin.ToInt();
            ViewData["IsAdmin"] = true;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = db.Users.Where(x => x.Id == userId).First();
            ViewData["MyId"] = userId;
            ViewData["MyName"] = user.Name;
            ViewData["MyImage"] = user.Image;

            var childIds = X.Child.Select(x => x.Id).ToList();

            M.Conversation = db.Conversation
                .Where(x => x.ProjectId == Id || childIds.Contains(x.ProjectId))
                .Include(x => x.User)
                .Include(x => x.Parent).ThenInclude(x => x.User)
                .Include(x => x.Project).ThenInclude(x => x.Customer)
                .ToList();

            foreach (var item in X.Child)
            {
                M.Conversation.Add(new Conversation
                {
                    Message = item.Title,
                    CreatedAt = item.CT,
                    Attachment = "ChildForConversation",
                    UserId = "UserId"
                });
            }

            M.Conversation.Add(new Conversation
            {
                Message = X.Title,
                CreatedAt = X.CT,
                Attachment = "ChildForConversation",
                UserId = "UserId"
            });

            foreach (var item in X.ProjectReport)
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

                M.Conversation.Add(new Conversation
                {
                    Message = message,
                    CreatedAt = item.CreatedAt,
                    Attachment = "ProjectReport",
                    UserId = item.UserId,
                    User = item.User,
                    Role = Role.Designer
                });
            }

            foreach (var item in X.Child.SelectMany(x => x.ProjectReport))
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

                M.Conversation.Add(new Conversation
                {
                    Message = message,
                    CreatedAt = item.CreatedAt,
                    Attachment = "ProjectReport",
                    UserId = item.UserId,
                    User = item.User,
                    Role = Role.Designer
                });
            }

            M.Conversation = M.Conversation.OrderBy(x => x.CreatedAt).ToList();

            M.Child = X.Child.Where(x => x.IsDelete == false).ToList();

            var designerIds = new List<string>();

            designerIds.Add(X.DesignerId);
            designerIds.AddRange(X.Payment.Select(x => x.DesignerId));
            designerIds.AddRange(M.Child.SelectMany(x => x.Payment).Select(x => x.DesignerId));

            ViewData["Designer"] = _designerRepository.GetList(true, designerIds);

            return View(M);
        }

        [HttpPost("/Edit/{Id}")]
        public int Edit(ProjectAddViewModel model, string delivery, string receipt)
        {
            model.Receipt = PersianDateTime.Parse(receipt);

            if (delivery != null)
            {
                model.Delivery = PersianDateTime.Parse(delivery);
            }

            return _projectRepository.Update(model, User);
        }

        [HttpGet("/print/{id}")]
        public IActionResult Print(int id)
        {
            return View(_customerRepository.Print(id));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[HttpPost("/project/add-media")]
        //public IActionResult AddMedia(MediaViewModel model)
        //{
        //    if (model.Media == null || model.Media.Any() == false)
        //    {
        //        return Ok();
        //    }

        //    using var db = new PuzzleDbContext();

        //    var project = db.Project.Where(x => x.Id == model.ProjectId).Any();

        //    if (project == false)
        //    {
        //        return Ok();
        //    }

        //    var now = DateTime.Now;

        //    foreach (var item in model.Media)
        //    {
        //        db.Media.Add(new Media
        //        {
        //            CreatedAt = now,
        //            ProjectId = model.ProjectId,
        //            Url = item.UploadFile(model.ProjectId),
        //            Type = model.Type,
        //            Description = model.Description,
        //            Status = MediaStatus.Read
        //        });
        //    }

        //    db.SaveChanges();

        //    return Ok();
        //}

        //[HttpPost("/project/remove-media")]
        //public IActionResult RemoveMedia(int id)
        //{
        //    using var db = new PuzzleDbContext();

        //    var media = db.Media.Where(x => x.Id == id).FirstOrDefault();

        //    db.Media.Remove(media);

        //    db.SaveChanges();

        //    return Ok();
        //}

        [HttpPost("/project/get-media")]
        public IActionResult GetMedia(MediaViewModel model)
        {
            using var db = new PuzzleDbContext();

            var project = db.Project
                .Where(x => x.Id == model.ProjectId)
                .Where(x => x.IsDelete == false)
                .Include(x => x.Child)
                .First();

            var childIds = project.Child.Select(x => x.Id).ToList();

            //var media = db.Media.Where(x => x.ProjectId == model.ProjectId).ToList();

            //foreach (var item in media)
            //{
            //    item.Status = MediaStatus.New;
            //}

            var media = new List<Media>();

            var conversation = db.Conversation
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == model.ProjectId || childIds.Contains(x.ProjectId))
                .Where(x => x.Type == MediaType.Render || x.Type == MediaType.Screen)
                .Include(x => x.ConversationFavorite)
                .ToList();

            foreach (var item in conversation)
            {
                media.Add(new Media
                {
                    CreatedAt = item.CreatedAt,
                    Url = item.Attachment,
                    Type = item.Type.Value,
                    Description = item.Message ?? "Conversation",
                    Id = item.Id,
                    Status = item.ConversationFavorite.Any() ? MediaStatus.Read : MediaStatus.New
                });
            }

            media = media.OrderByDescending(x => x.CreatedAt).ToList();

            return PartialView("_MediaPartial", media);
        }

        //public IActionResult Notification()
        //{
        //    using var db = new PuzzleDbContext();

        //    var media = db.Media
        //        .OrderByDescending(x => x.CreatedAt)
        //        .GroupBy(x => x.Project)
        //        .Select(x => new
        //        {
        //            Project = x.Key,
        //            ModifiedAt = x.OrderByDescending(a => a.CreatedAt).First()
        //        })
        //        .ToList();

        //    return Ok();
        //}

        //[HttpGet("/timeline")]
        //public IActionResult Timeline(int? page = 1)
        //{
        //    using var db = new PuzzleDbContext();

        //    var result = _projectRepository.Timeline(page.Value);

        //    ViewData["Designer"] = _designerRepository.GetList(true);
        //    ViewData["FormTitle"] = "تایم‌لاین";

        //    return View("/Views/Project/Index.cshtml", result);
        //}

        [HttpGet("/report")]
        public IActionResult ProjectReport(int? page = 1)
        {
            using var db = new PuzzleDbContext();

            var result = _projectRepository.ProjectReport(page.Value);

            ViewData["Designer"] = _designerRepository.GetList(true);
            ViewData["FormTitle"] = "گزارش روزانه";

            return View("/Views/Project/Index.cshtml", result);
        }

        [HttpGet("/decoroham/project")]
        public IActionResult Decoroham(int page = 1, Enums.Enum.ProjectCategory category = ProjectCategory.ModernCabinet)
        {
            var result = _projectRepository.GetByCategoryId(category, page);

            ViewData["Designer"] = _designerRepository.GetList(true);
            ViewData["FormTitle"] = "دکورهام";
            ViewData["ShowCategory"] = true;

            return View("/Views/Project/Index.cshtml", result);
        }

        [HttpPost("/Repair/add")]
        public object RepairAdd(ProjectAddViewModel model, string Delivery, string Receipt)
        {
            if (Receipt != null)
            {
                model.Receipt = PersianDateTime.Parse(Receipt);
            }

            if (Delivery != null)
            {
                model.Delivery = PersianDateTime.Parse(Delivery);
            }

            var db = new PuzzleDbContext();

            var parent = db.Project
                .Where(x => x.Id == model.ProjectId)
                .Where(x => x.ProjectId == null)
                .Include(x => x.Child)
                .Include(x => x.Customer)
                .First();

            var project = new Project
            {
                ProjectId = model.ProjectId,
                CB = User.GetUserId(),
                CT = DateTime.Now,

                CustomerId = parent.CustomerId,

                Description = model.Description,

                OrderType = model.OrderType,
                Print = model.Print,

                ProjectCategory = model.ProjectCategory,
                Status = model.Status,

                TotalPrice = model.TotalPrice == null ? 0 : model.TotalPrice.Replace(",", null).ToInt(),
                Approximate = model.Approximate == null ? 0 : model.Approximate.Replace(",", null).ToInt(),

                Payment = new List<Payment>(),

                Receipt = model.Receipt.HasValue ? model.Receipt.Value.ToDateTime() : DateTime.Today,
                Delivery = model.Delivery?.ToDateTime().Date.AddHours(model.DeliveryTime)
            };

            if (project.Print)
            {
                project.PrintPrice = model.PrintPrice ?? Setting.Print(DateTime.Today);
            }

            for (int i = 0; i < model.PaymentType.Length; i++)
            {
                if (model.PaymentPrice[i] != null && model.PaymentPrice[i].Replace(",", null).ToInt() > 0 &&
                    (string.IsNullOrEmpty(model.PaymentDate[i]) || PersianDateTime.Parse(model.PaymentDate[i]).ToDateTime().Date == DateTime.Today))
                {
                    var payment = new Payment
                    {
                        CreatedAt = string.IsNullOrEmpty(model.PaymentDate[i]) ? DateTime.Today : PersianDateTime.Parse(model.PaymentDate[i]).ToDateTime(),
                        Price = model.PaymentPrice[i].Replace(",", null).ToInt(),
                        PaymentType = model.PaymentType[i].Value
                    };

                    //payment.CreatedAt = payment.CreatedAt.AddHours(model.PaymentTime[i].ToInt());

                    project.Payment.Add(payment);
                }
            }

            var positive = project.Payment
                .Where(c => c.PaymentType != PaymentType.Return)
                .Where(c => c.PaymentType != PaymentType.ToCredit)
                .Sum(c => c.Price);

            var negative = project.Payment
                .Where(c => c.PaymentType == PaymentType.Return || c.PaymentType == PaymentType.ToCredit)
                .Sum(c => c.Price);

            var fromCredit = project.Payment.Where(c => c.PaymentType == PaymentType.FromCredit).Sum(c => c.Price);

            if (fromCredit > 0)
            {
                var credit = parent.Customer.Credit;

                if (fromCredit > credit)
                {
                    return new { Status = 4 };
                }
            }

            project.TotalPayment = positive - negative;

            if (project.TotalPayment < 0)
            {
                return new { Status = 6 };
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                var count = parent.Child.Count;

                project.Title = $"متمم {count + 1}";
            }
            else
            {
                project.Title = model.Title;
            }

            #region ProjectAccounting

            project.ProjectAccounting = new List<ProjectAccounting>
            {
                new ProjectAccounting
                {
                    Approximate = project.Approximate,
                    CreatedAt = DateTime.Now,
                    DesignerPercent = project.DesignerPercent,
                    PrintPrice = project.PrintPrice,
                    TotalPayment = project.TotalPayment,
                    TotalPrice = project.TotalPrice,
                    DesignerId = project.DesignerId,
                    Wage = project.Wage
                }
            };

            #endregion

            db.Project.Add(project);

            db.SaveChanges();

            _projectRepository.UpdateChild(project.ProjectId.Value);

            _customerRepository.Balance(project.CustomerId);

            return new { Status = 2, project.Id };
        }

        [HttpPost("/Repair/edit")]
        public int RepairEdit(ProjectAddViewModel model, string Delivery, string Receipt)
        {
            using var db = new PuzzleDbContext();

            if (Receipt != null)
            {
                model.Receipt = PersianDateTime.Parse(Receipt);
            }

            if (Delivery != null)
            {
                model.Delivery = PersianDateTime.Parse(Delivery);
            }

            var project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.Id == model.Id)
                .Include(c => c.Payment)
                .Include(c => c.ProjectAccounting)
                .First();

            project.Description = model.Description;
            project.Eitaa = model.Eitaa;
            project.Follow = model.Follow;
            project.Notification = model.Notification;
            project.OrderType = model.OrderType;
            //project.Other = model.Other;
            project.Print = model.Print;
            project.PrintPrice = model.Print ? (model.PrintPrice ?? Setting.Print(DateTime.Today)) : null;
            project.PrintDelivery = model.PrintDelivery;
            project.ProjectCategory = model.ProjectCategory;
            project.Ready = model.Ready;
            //project.Rubika = model.Rubika;
            project.Screenshot = model.Screenshot;
            //project.Soroush = model.Soroush;
            project.Status = model.Status;
            project.Telegram = model.Telegram;
            project.Whatsapp = model.Whatsapp;
            project.Flash = model.Flash;
            project.App = model.App;

            project.Receipt = model.Receipt.HasValue ? model.Receipt.Value.ToDateTime() : DateTime.Today;
            project.Delivery = model.Delivery?.ToDateTime().Date.AddHours(model.DeliveryTime);

            project.TotalPrice = model.TotalPrice == null ? 0 : model.TotalPrice.Replace(",", null).ToInt();
            project.Approximate = model.Approximate == null ? 0 : model.Approximate.Replace(",", null).ToInt();

            project.Payment = project.Payment.Where(c => c.CreatedAt.Date != DateTime.Today).ToList();

            for (int i = 0; i < model.PaymentType.Length; i++)
            {
                if (model.PaymentPrice[i] != null && model.PaymentPrice[i].Replace(",", null).ToInt() > 0 &&
                    (string.IsNullOrEmpty(model.PaymentDate[i]) || PersianDateTime.Parse(model.PaymentDate[i]).ToDateTime().Date == DateTime.Today))
                {
                    project.Payment.Add(new Payment
                    {
                        CreatedAt = string.IsNullOrEmpty(model.PaymentDate[i]) ? DateTime.Today : PersianDateTime.Parse(model.PaymentDate[i]).ToDateTime(),
                        Price = model.PaymentPrice[i].Replace(",", null).ToInt(),
                        PaymentType = model.PaymentType[i].Value,
                        DesignerId = model.PaymentType[i].Value == PaymentType.ToDesigner ? model.PaymentDesignerId[i] : null
                    });
                }
            }

            var positive = project.Payment
                .Where(c => c.PaymentType != PaymentType.Return)
                .Where(c => c.PaymentType != PaymentType.ToCredit)
                .Sum(c => c.Price);

            var negative = project.Payment
                .Where(c => c.PaymentType == PaymentType.Return || c.PaymentType == PaymentType.ToCredit)
                .Sum(c => c.Price);

            var fromCredit = project.Payment.Where(c => c.PaymentType == PaymentType.FromCredit).Sum(c => c.Price);

            var toCredit = project.Payment.Where(c => c.PaymentType == PaymentType.ToCredit).Sum(c => c.Price);

            if (fromCredit > 0)
            {
                var credit = _customerRepository.Credit(project.CustomerId, project.Id);

                if (fromCredit > (credit + toCredit))
                {
                    return 4;
                }
            }

            project.TotalPayment = positive - negative;

            if (project.TotalPayment < 0)
            {
                return 6;
            }

            #region ProjectAccounting

            project.ProjectAccounting.Add(new ProjectAccounting
            {
                Approximate = project.Approximate,
                CreatedAt = DateTime.Now,
                DesignerPercent = project.DesignerPercent,
                PrintPrice = project.PrintPrice,
                TotalPayment = project.TotalPayment,
                TotalPrice = project.TotalPrice,
                DesignerId = project.DesignerId,
                Wage = project.Wage
            });

            #endregion

            db.SaveChanges();

            _projectRepository.UpdateChild(project.ProjectId.Value);

            _customerRepository.Balance(project.CustomerId);

            return 2;
        }

        [HttpPost("/Conversation/Read")]
        public IActionResult ReadConversation(int projectId, int conversationId)
        {
            using var db = new PuzzleDbContext();

            var conversation = db.Conversation
                .Where(x => x.ProjectId == projectId)
                .Where(x => x.UnreadByAdmin == true)
                .Where(x => x.Id <= conversationId)
                .ToList();

            if (conversation.Any())
            {
                foreach (var item in conversation)
                {
                    item.UnreadByAdmin = false;
                }

                db.SaveChanges();
            }

            return Ok();
        }

        [Route("instagram/{type}/{*url}")]
        public IActionResult Instagram(string url, string type, int width = 1600)
        {
            if (System.IO.File.Exists(Path.Combine("wwwroot", url.Trim('/').Trim('\\'))))
            {
                var height = (width / 4.0 * 3).ToInt();

                using var image = Image.FromStream(ResizeImage(url, width, height));

                var bitmap = new Bitmap(image, width, height);

                using var watermarkImage = Image.FromFile(Path.Combine("wwwroot", "mark-" + type + ".png"));

                using var imageGraphics = Graphics.FromImage(bitmap);

                using var watermarkBrush = new TextureBrush(watermarkImage);

                using var memoryStream = new MemoryStream();

                int x = bitmap.Width / 2 - watermarkImage.Width / 2;
                int y = bitmap.Height / 2 - watermarkImage.Height / 2;
                watermarkBrush.TranslateTransform(x, y);
                imageGraphics.FillRectangle(watermarkBrush, new Rectangle(new Point(x, y), new Size(watermarkImage.Width + 1, watermarkImage.Height)));
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                byte[] byteArray = memoryStream.ToArray();

                Response.Headers.Add("Cache-Control", "public, max-age=2592000");

                return File(byteArray, "image/jpeg");
            }
            else
            {
                return NotFound();
            }
        }

        public static MemoryStream ResizeImage(string url, int width, int height)
        {
            var memoryStream = new MemoryStream();

            Image image = Image.FromFile(Path.Combine("wwwroot", url.Trim('/').Trim('\\')));

            double imageWidth = image.Width;
            double imageHeight = image.Height;

            double W = width;
            double H = height;
            double A = imageWidth / width;
            double B = imageHeight / height;

            if (A > B)
            {
                W = imageWidth / B;
            }
            else
            {
                H = imageHeight / A;
            }

            double X = (width - W) / 2;
            double Y = (height - H) / 2;

            var destRect = new Rectangle(X.ToInt(), Y.ToInt(), W.ToInt(), H.ToInt());

            var bitmap = new Bitmap(width.ToInt(), height.ToInt());

            bitmap.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);
            }

            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

            return memoryStream;
        }
    }
}
