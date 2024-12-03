using Helper;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Puzzle.Data;
using Puzzle.Helper;
using Puzzle.Models;
using Repository;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ViewModels;
using static Controllers.MyController;
using static Enums.Enum;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Controllers
{
    [TimeFilter]
    [Authorize(Roles = "Admin")]
    public class CustomerController : Controller
    {
        private readonly CustomerRepository _customerRepository;
        private readonly DesignerRepository _designerRepository;
        private readonly ProjectRepository _projectRepository;

        public CustomerController()
        {
            _customerRepository = new CustomerRepository();
            _designerRepository = new DesignerRepository();
            _projectRepository = new ProjectRepository();
        }

        public IActionResult Index(int Page = 1, OrderBy OrderBy = OrderBy.Support, bool Asc = false, int Count = 20, string keyword = null)
        {
            var X = _customerRepository.ToList(Page, OrderBy, Asc, Count, keyword);

            if (keyword != null)
            {
                keyword = "&keyword=" + keyword;
            }

            ViewData["Keyword"] = keyword;

            return View(X);
        }

        public IActionResult Details(int id)
        {
            var X = _customerRepository.Project(id);

            if (X == null)
            {
                return NotFound();
            }

            var designerId = X.Transaction.Select(x => x.DesignerId).ToList();

            ViewData["Designer"] = _designerRepository.GetList(true, designerId);

            ViewData["CustomerId"] = id;

            return View(X);
        }

        public IActionResult Payment(int id)
        {
            using var db = new PuzzleDbContext();

            var customer = db.Customer.Where(x => x.Id == id).First();

            ViewData["Customer"] = customer;

            var customerChat = db.CustomerChat
                .Where(x => x.CustomerId == id)
                .Where(x => x.IsDelete == false)
                .ToList();

            foreach (var item in customerChat)
            {
                item.UnreadByAdmin = false;
            }
            db.SaveChanges();
            return View(customerChat);
        }

        [HttpPost]
        public int Edit(CustomerViewModel customer, string birthday)
        {
            var mobile = _customerRepository.GetByMobile(customer.Mobile);

            if (mobile != null && mobile.Id != customer.Id)
            {
                return 1;
            }

            if (birthday != null)
            {
                customer.Birthday = PersianDateTime.Parse(birthday);
            }

            _customerRepository.Update(customer);

            return 2;
        }

        [HttpPost]
        public IActionResult Delete(int Id)
        {
            using var db = new PuzzleDbContext();

            var anyProject = db.Project
                .Where(x => x.CustomerId == Id)
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Any();

            if (anyProject)
            {
                return Conflict(new { Message = "مشتری مورد نظر دارای پروژه می باشد و امکان حذف آن وجود ندارد" });
            }

            anyProject = db.CustomerChat
   .Where(x => x.CustomerId == Id)
   .Any();

            if (anyProject)
            {
                return Conflict(new { Message = "مشتری مورد نظر دارای سابقه واریز می باشد و امکان حذف آن وجود ندارد" });
            }


            anyProject = db.Transaction
   .Where(x => x.CustomerId == Id)
   .Any();

            if (anyProject)
            {
                return Conflict(new { Message = "مشتری مورد نظر دارای سابقه واریز می باشد و امکان حذف آن وجود ندارد" });
            }

            _customerRepository.Remove(Id);

            return Ok();
        }

        [HttpPost]
        public IActionResult Project(int id, int page, int status)
        {
            var model = _projectRepository.GetByCustomerId(id, page, status);
            ViewData["Designer"] = _designerRepository.GetList(true);
            return View(model);
        }

        public IActionResult AddProject(int Id)
        {
            var X = _customerRepository.GetById(Id);

            if (X == null)
            {
                return NotFound();
            }

            ViewData["Designer"] = _designerRepository.GetList(true);
            ViewData["Number"] = _projectRepository.GetTodayCount();
            ViewData["Duplicate"] = _customerRepository.Duplicate(X.Id);

            return View(X);
        }

        public IActionResult Search(string Keyword, int Page = 1)
        {
            var X = _customerRepository.Search(Keyword, Page);
            if (X == null)
            {
                return NotFound();
            }
            return View(X);
        }

        [HttpPost]
        public int Merge(string oldMobile, string newMobile)
        {
            using var db = new PuzzleDbContext();

            var customer = db.Customer
                .Where(x => x.Mobile == newMobile)
                .FirstOrDefault();

            var oldCustomer = db.Customer
                .Where(x => x.Mobile == oldMobile)
                .Include(x => x.Project)
                .Include(x => x.Transaction)
                .FirstOrDefault();

            if (customer == null || oldCustomer == null)
            {
                return 1;
            }

            foreach (var item in oldCustomer.Project)
            {
                item.CustomerId = customer.Id;
            }

            foreach (var item in oldCustomer.Transaction)
            {
                item.CustomerId = customer.Id;
            }

            db.SaveChanges();

            _customerRepository.Balance(customer.Id);
            _customerRepository.Balance(oldCustomer.Id);

            return 2;
        }

        [HttpPost("/customer/toggle-done")]
        public bool ToggleDone(int customerId)
        {
            using var db = new PuzzleDbContext();

            var customer = db.Customer
                .Where(x => x.Id == customerId)
                .First();

            customer.DoneByAdmin = !customer.DoneByAdmin;

            db.SaveChanges();

            return true;
        }

        [HttpPost]
        public IActionResult Payment(PaymentModel model)
        {
            using var db = new PuzzleDbContext();

            var customerChat = new CustomerChat
            {
                Attachment = model.Attachment.UploadFile("customerchat/" + model.CustomerId),
                CreatedAt = DateTime.Now,
                CustomerId = model.CustomerId,
                UnreadByCustomer = true,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            };

            db.CustomerChat.Add(customerChat);

            db.SaveChanges();

            return Ok();
        }
    }
}