using Helper;
using MD.PersianDateTime.Standard;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Puzzle;
using Puzzle.Data;
using Puzzle.Models;
using Puzzle.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using ViewModels;

namespace Repository
{
    public class ProjectRepository
    {
        private readonly CustomerRepository customerRepository;
        private readonly DesignerRepository designerRepository;

        public ProjectRepository()
        {
            customerRepository = new CustomerRepository();
            designerRepository = new DesignerRepository();
        }

        public int GetTodayCount()
        {
            using var db = new PuzzleDbContext();

            var result = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.Receipt.Date == DateTime.Today)
                .Where(c => c.ProjectCategory != Enums.Enum.ProjectCategory.Amendment && c.ProjectCategory != Enums.Enum.ProjectCategory.Edit && c.ProjectCategory != Enums.Enum.ProjectCategory.Print)
                .Count();

            return result;
        }

        public List<Project> Open(string designerId = null)
        {
            using var db = new PuzzleDbContext();

            var firstOf1400 = new PersianDateTime(1400, 1, 1);

            var project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => !c.PrintDelivery && !c.Whatsapp && !c.Telegram && !c.Other && !c.Soroush && !c.Eitaa && !c.Rubika && !c.Flash && !c.App)
                .Where(x => x.Cancel == false)
                .Where(c => designerId == null || c.DesignerId == designerId)
                .Where(c => c.Receipt >= firstOf1400)
                .Include(c => c.Customer)
                .Include(c => c.Designer)
                //.Include(c => c.Media)
                .Include(c => c.Child)
                .Include(c => c.Parent)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport)
                .OrderByDescending(c => c.CT)
                .ToList();

            return project;
        }

        public List<Project> NoSettlement(string designerId = null)
        {
            using var db = new PuzzleDbContext();

            var firstOf1400 = new PersianDateTime(1400, 1, 1);

            var project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.TotalPrice > 0 ? c.TotalPayment != c.TotalPrice : c.TotalPayment != c.Approximate)
                .Where(c => designerId == null || c.DesignerId == designerId)
                .Where(c => c.Receipt >= firstOf1400)
                .Include(c => c.Customer)
                .Include(c => c.Designer)
                //.Include(c => c.Media)
                .Include(c => c.Child)
                .Include(c => c.Parent)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport)
                .OrderByDescending(c => c.CT)
                .ToList();

            return project;
        }

        public List<Project> NoDesigner()
        {
            using var db = new PuzzleDbContext();

            var firstOf1400 = new PersianDateTime(1400, 1, 1);

            var project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(x => x.Cancel == false)
                .Where(x => x.ProjectCategory != Enums.Enum.ProjectCategory.Cancel)
                .Where(c => c.DesignerId == null)
                .Where(c => c.Receipt >= firstOf1400)
                .Include(c => c.Customer)
                .Include(c => c.Designer)
                //.Include(x => x.Media)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport)
                .OrderByDescending(c => c.CT)
                .ToList();

            return project;
        }

        public List<Project> Follow()
        {
            using var db = new PuzzleDbContext();

            var project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(x => x.Cancel == false)
                .Where(c => c.Follow == true)
                .Include(c => c.Customer)
                .Include(c => c.Designer)
                //.Include(x => x.Media)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport)
                .OrderByDescending(c => c.CT)
                .ToList();

            return project;
        }

        public ProjectViewModel GetByCategoryId(Enums.Enum.ProjectCategory categoryId, int page)
        {
            using var db = new PuzzleDbContext();

            var result = new ProjectViewModel()
            {
                Page = page,
                ProjectCategory = categoryId
            };

            var query = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(x => x.ProjectCategory == categoryId);

            result.TotalPage = Math.Ceiling(query.Count() / 20.0).ToInt();

            result.Project = query
                .Include(c => c.Customer)
                .Include(c => c.Designer)
                //.Include(x => x.Media)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport)
                .OrderByDescending(c => c.CT)
                .Skip((page - 1) * 20)
                .Take(20)
                .ToList();

            return result;
        }

        public List<Project> ToList(PersianDateTime? from, PersianDateTime? to, int page)
        {
            using var db = new PuzzleDbContext();

            var X = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.Receipt.Date == DateTime.Today.AddDays(-page + 1))
                .Where(c => from == null || c.Receipt >= from)
                .Where(c => to == null || c.Receipt <= to)
                .Include(c => c.Customer)
                .Include(c => c.Designer)
                //.Include(x => x.Media)
                .Include(x => x.Parent)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport)
                .OrderByDescending(c => c.CT)
                .ToList();

            return X;
        }

        //public ProjectViewModel Timeline(int page)
        //{
        //    using var db = new PuzzleDbContext();

        //    var result = new ProjectViewModel { Page = page };

        //    var query = db.Project
        //        .Where(x => x.IsDelete == false)
        //        .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
        //        .Where(x => x.Media.Any() == true)
        //        .Where(x => (x.MediaDone == false && x.Cancel == false) || x.Media.Where(x => x.Status == Enums.Enum.MediaStatus.New).Any())
        //        .Include(c => c.Customer)
        //        .Include(c => c.Designer)
        //        .Include(x => x.Conversation)
        //        .Include(x => x.Media)
        //        .Include(x => x.ProjectReport);

        //    result.TotalPage = Math.Ceiling(query.Count() / 30.0).ToInt();

        //    result.Project = query.OrderByDescending(c => c.Media.OrderByDescending(a => a.CreatedAt).First().CreatedAt)
        //        .Skip((page - 1) * 30)
        //        .Take(30)
        //        .ToList();

        //    return result;
        //}

        public ProjectViewModel ProjectReport(int page)
        {
            using var db = new PuzzleDbContext();

            var result = new ProjectViewModel { Page = page, Project = new List<Project>() };

            var project = db.Project
                 .Where(x => x.IsDelete == false)
                 .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                 .Where(x => x.ProjectReport.Any() == true)
                 .Where(x => !x.PrintDelivery && !x.Whatsapp && !x.Telegram && !x.Other && !x.Soroush && !x.Eitaa && !x.Rubika && !x.Flash && !x.App)
                 .Where(x => (x.ProjectReport.Where(x => x.ReadByAdmin == false).Any() == true) || ((x.Delivery.Value.Date <= DateTime.Today) && !x.Cancel && x.ProjectCategory != Enums.Enum.ProjectCategory.Cancel))
                 .Where(x => (string.IsNullOrWhiteSpace(x.ProjectReport.OrderByDescending(x => x.Id).First().Description) == false) || ((x.ProjectReport.OrderByDescending(x => x.Id).First().Reason != Enums.Enum.ProjectReportReason.UntilNoon) && (x.ProjectReport.OrderByDescending(x => x.Id).First().Reason != Enums.Enum.ProjectReportReason.UntilEvening)))
                 .Include(c => c.Customer)
                 .Include(c => c.Designer)
                 .Include(x => x.Conversation)
                 //.Include(x => x.Media)
                 .Include(x => x.Favorite)
                 .Include(x => x.ProjectReport)
                 .Include(x => x.Child)
                 .Include(x => x.Parent)
                 .OrderByDescending(c => c.ProjectReport.OrderByDescending(a => a.CreatedAt).First().CreatedAt)
                 .ToList();

            foreach (var item in project)
            {
                var projectReport = item.ProjectReport.Last();

                if ((string.IsNullOrWhiteSpace(projectReport.Description) == false) || ((projectReport.Reason != Enums.Enum.ProjectReportReason.UntilNoon) && (projectReport.Reason != Enums.Enum.ProjectReportReason.UntilEvening)))
                {
                    result.Project.Add(item);
                }
            }

            return result;
        }

        public ProjectViewModel GetByCustomerId(int customerId, int page, int status)
        {
            using var db = new PuzzleDbContext();

            var result = new ProjectViewModel()
            {
                Page = page,
                Status = status
            };

            var query = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(x => x.CustomerId == customerId);

            if (status == 1)
            {
                query = query.Where(c => !c.PrintDelivery && !c.Whatsapp && !c.Telegram && !c.Other && !c.Soroush && !c.Eitaa && !c.Rubika && !c.Flash && !c.App);
            }
            else if (status == 2)
            {
                query = query.Where(x => x.TotalPrice > 0 ? x.TotalPrice != x.TotalPayment : x.Approximate != x.TotalPayment);
            }

            result.TotalPage = Math.Ceiling(query.Count() / 20.0).ToInt();

            result.Project = query.OrderByDescending(x => x.Receipt).ThenByDescending(x => x.Id)
                .Skip((page - 1) * 20)
                .Take(20)
                .Include(c => c.Customer)
                .Include(c => c.Designer)
                //.Include(x => x.Media)
                .Include(x => x.Parent)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport)
                .ToList();

            return result;
        }

        public Project GetById(int id)
        {
            using var db = new PuzzleDbContext();

            var project = db.Project
                .Where(x => x.IsDelete == false)
                .Include(c => c.Customer)
                .Include(c => c.Designer)
                .Include(c => c.Survey)
                .Include(c => c.Payment)
                .Include(c => c.Presenter).ThenInclude(c => c.Customer)
                //.Include(c => c.Media)
                .Include(c => c.Child).ThenInclude(c => c.Payment)
                .Include(c => c.Child).ThenInclude(c => c.ProjectReport).ThenInclude(x => x.User)
                //.Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport).ThenInclude(x => x.User)
                .First(c => c.Id == id);

            return project;
        }

        public string Remove(int id)
        {
            using var db = new PuzzleDbContext();

            var project = db.Project.Where(c => c.Id == id)
                .Include(c => c.Payment)
                //.Include(c => c.Media)
                //.Include(x => x.Conversation)
                .Include(x => x.Child).ThenInclude(x => x.Payment)
                .First();

            if (project.Payment.Where(x => x.CreatedAt.Date != DateTime.Today).Any())
            {
                return "پروژه مورد نظر دارای سوابق مالی می باشد و امکان حذف آن وجود ندارد";
            }

            if (project.ProjectId == null)
            {
                if (project.Child.Where(x => x.Payment.Where(a => a.CreatedAt.Date != DateTime.Today).Any()).Any())
                {
                    return "پروژه مورد نظر دارای سوابق مالی می باشد و امکان حذف آن وجود ندارد";
                }
            }

            project.IsDelete = true;

            db.SaveChanges();

            customerRepository.Balance(project.CustomerId);

            return null;
        }

        public void Favorite(int id, string userId)
        {
            using var db = new PuzzleDbContext();

            var favorite = db.Favorite
                .Where(c => c.ProjectId == id)
                .Where(c => c.UserId == userId)
                .FirstOrDefault();

            if (favorite == null)
            {
                db.Favorite.Add(new Favorite
                {
                    ProjectId = id,
                    UserId = userId
                });
            }
            else
            {
                db.Favorite.Remove(favorite);
            }

            db.SaveChanges();
        }

        public ProjectViewModel GetByDesignerId(string designerId, PersianDateTime? fromDate = null, PersianDateTime? toDate = null, int? page = null, int? status = null, int? customerId = null)
        {
            using var db = new PuzzleDbContext();

            var result = new ProjectViewModel { };

            if (page != null)
            {
                result.Page = page.Value;
            }

            var query = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.DesignerId == designerId)
                .Where(x => customerId == null || x.CustomerId == customerId)
                .Where(c => fromDate == null || c.Receipt >= fromDate)
                .Where(c => toDate == null || c.Receipt <= toDate);

            if (status == 1)
            {
                query = query.Where(c => !c.PrintDelivery && !c.Whatsapp && !c.Telegram && !c.Other && !c.Soroush && !c.Eitaa && !c.Rubika && !c.Flash && !c.App);
            }
            else if (status == 2)
            {
                query = query.Where(x => x.TotalPrice > 0 ? x.TotalPrice != x.TotalPayment : x.Approximate != x.TotalPayment);
            }

            result.TotalRow = query.Count();
            result.TotalPage = Math.Ceiling(result.TotalRow / 10.0).ToInt();

            result.Project = query.OrderByDescending(c => c.Receipt)
                .Skip(page != null ? (page.Value - 1) * 10 : 0)
                .Take(page != null ? 10 : int.MaxValue)
                .Include(c => c.Designer)
                .Include(c => c.Customer)
                .Include(c => c.Payment)
                //.Include(c => c.Media)
                .Include(c => c.Child)
                .Include(c => c.Parent)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport)
                .ToList();

            return result;
        }

        public int CountDesignerId(string designerId, PersianDateTime from, PersianDateTime to)
        {
            using var db = new PuzzleDbContext();

            return db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.DesignerId == designerId)
                .Where(c => c.Receipt >= from)
                .Where(c => c.Receipt <= to)
                .Count();
        }

        public double WageByDesignerId(string designerId, PersianDateTime? from = null, PersianDateTime? to = null)
        {
            using var db = new PuzzleDbContext();

            return db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.DesignerId == designerId)
                .Where(c => from == null || c.Receipt >= from)
                .Where(c => to == null || c.Receipt <= to)
                .Sum(x => x.Wage);
        }

        public DateTime FirstByDesignerId(string designerId)
        {
            using var db = new PuzzleDbContext();

            return db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.DesignerId == designerId)
                .OrderBy(c => c.Receipt)
                .Select(x => x.Receipt)
                .First();
        }

        public DateTime LastByDesignerId(string designerId)
        {
            using var db = new PuzzleDbContext();

            return db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.DesignerId == designerId)
                .OrderByDescending(c => c.Receipt)
                .Select(x => x.Receipt)
                .First();
        }

        public void Add(Project project)
        {
            using var db = new PuzzleDbContext();

            db.Update(project);

            db.SaveChanges();
        }

        public int Update(ProjectAddViewModel model, ClaimsPrincipal user)
        {
            using var db = new PuzzleDbContext();

            var project = db.Project
               .Where(x => x.IsDelete == false)
               .Where(x => x.Id == model.Id)
               .Include(c => c.Payment)
               .Include(c => c.ProjectAccounting)
               .First();

            int oldCustomerId = project.CustomerId;

            var customer = customerRepository.GetByMobile(model.Mobile);

            if (customer == null)
            {
                return 5;
            }

            if (model.DesignerId == null)
            {
                project.DesignerPercent = 0;

                project.AssignedToDesignerAt = null;
            }
            else if (project.DesignerId != model.DesignerId)
            {
                project.DesignerPercent = designerRepository.GetPercent(model.DesignerId, project.CT);

                project.AssignedToDesignerAt = DateTime.Now;
            }
            else if (user.IsInRole("Manager"))
            {
                project.DesignerPercent = model.DesignerPercent;
            }

            project.Receipt = model.Receipt.HasValue ? model.Receipt.Value.ToDateTime() : DateTime.Today;
            project.Delivery = model.Delivery?.ToDateTime().Date.AddHours(model.DeliveryTime);
            project.Description = model.Description;
            project.ProjectCategory = model.ProjectCategory;
            project.DesignerId = model.DesignerId;
            project.Status = model.Status;
            project.TotalPrice = model.TotalPrice == null ? 0 : model.TotalPrice.Replace(",", null).ToInt();
            project.OrderType = model.OrderType;
            project.Approximate = model.Approximate == null ? 0 : model.Approximate.Replace(",", null).ToInt();
            project.Print = model.Print;
            project.Title = model.Title;
            project.Type = model.Type;

            project.PrintDelivery = model.PrintDelivery;
            project.Telegram = model.Telegram;
            project.Whatsapp = model.Whatsapp;
            //project.Soroush = model.Soroush;
            project.Eitaa = model.Eitaa;
            //project.Rubika = model.Rubika;
            project.Flash = model.Flash;
            project.App = model.App;

            project.Notification = model.Notification;
            project.Ready = model.Ready;
            project.Screenshot = model.Screenshot;
            project.Follow = model.Follow;

            project.Payment = project.Payment.Where(c => c.CreatedAt.Date != DateTime.Today).ToList();

            project.CustomerId = customer.Id;

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
                        DesignerId = model.PaymentType[i].Value == Enums.Enum.PaymentType.ToDesigner ? model.PaymentDesignerId[i] : null,
                        Description = model.PaymentDescription[i]
                    });
                }
            }

            var positive = project.Payment
                .Where(c => c.PaymentType != Enums.Enum.PaymentType.Return)
                .Where(c => c.PaymentType != Enums.Enum.PaymentType.ToCredit)
                .Sum(c => c.Price);

            var negative = project.Payment
                .Where(c => c.PaymentType == Enums.Enum.PaymentType.Return || c.PaymentType == Enums.Enum.PaymentType.ToCredit)
                .Sum(c => c.Price);

            var fromCredit = project.Payment.Where(c => c.PaymentType == Enums.Enum.PaymentType.FromCredit).Sum(c => c.Price);

            var toCredit = project.Payment.Where(c => c.PaymentType == Enums.Enum.PaymentType.ToCredit).Sum(c => c.Price);

            if (fromCredit > 0)
            {
                var credit = customerRepository.Credit(customer.Id, project.Id);

                if (fromCredit > (credit + toCredit))
                {
                    return 4;
                }
            }

            project.TotalPayment = positive - negative;

            if (project.Print)
            {
                project.PrintPrice = model.PrintPrice ?? Setting.Print(DateTime.Today);
            }
            else
            {
                project.PrintPrice = null;
            }

            if (project.DesignerId != null)
            {
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
            }
            else
            {
                project.Wage = 0;
            }

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

            UpdateChild(project.Id);

            customerRepository.Balance(project.CustomerId);

            if (oldCustomerId != project.CustomerId)
            {
                customerRepository.Balance(oldCustomerId);
            }

            return 2;
        }

        public void UpdateChild(int projectId)
        {
            using var db = new PuzzleDbContext();

            var project = db.Project.Where(x => x.Id == projectId)
                .Include(x => x.Child)
                .First();

            var designerRepository = new DesignerRepository();

            foreach (var item in project.Child)
            {
                if (project.DesignerId == null)
                {
                    item.AssignedToDesignerAt = null;
                }
                else if (project.DesignerId != item.DesignerId)
                {
                    item.AssignedToDesignerAt = DateTime.Now;
                }

                item.DesignerId = project.DesignerId;
                item.CustomerId = project.CustomerId;

                item.DesignerPercent = project.DesignerPercent;

                if (item.DesignerId != null)
                {
                    int max = item.TotalPrice > 0 ? item.TotalPrice : item.Approximate;

                    int print = item.PrintPrice ?? 0;

                    if (item.TotalPayment < max)
                    {
                        item.Wage = item.DesignerPercent * (item.TotalPayment - print) / 100.0;
                    }
                    else
                    {
                        item.Wage = item.DesignerPercent * (max - print) / 100.0;
                    }
                }
                else
                {
                    item.Wage = 0;
                }
            }

            db.SaveChanges();
        }
    }
}
