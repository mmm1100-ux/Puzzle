using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Puzzle;
using Puzzle.Data;
using Puzzle.Helper;
using Puzzle.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewModels;

namespace Controllers
{
    [TimeFilter]
    [Authorize(Roles = "Admin")]
    public class PaymentController : Controller
    {
        private readonly PaymentRepository _paymentRepository;
        private readonly DesignerRepository _designerRepository;
        private readonly IMemoryCache _memoryCache;

        public PaymentController(IMemoryCache memoryCache)
        {
            _paymentRepository = new PaymentRepository();
            _designerRepository = new DesignerRepository();
            _memoryCache = memoryCache;
        }

        public IActionResult Index(int page = 1)
        {
            ViewData["Designer"] = _designerRepository.GetList(true);
            var model = new SearchPaymentViewModel
            {
                Page = new PageViewModel { CurrentPage = page },
                Payment = _paymentRepository.GetList(page)
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Details(int id)
        {
            if (_designerRepository.IsValid(HttpContext, _memoryCache) == false)
            {
                return Unauthorized();
            }

            using var db = new PuzzleDbContext();

            var payment = db.Payment.Where(x => x.Id == id).First();

            ViewData["Designer"] = _designerRepository.GetList(true, new List<string> { payment.DesignerId });

            return View(payment);
        }

        [HttpPost]
        public IActionResult Edit(int id, string date, int price, Enums.Enum.PaymentType type, string description, string designerId)
        {
            if (_designerRepository.IsValid(HttpContext, _memoryCache) == false)
            {
                return Unauthorized();
            }

            using var db = new PuzzleDbContext();

            var project = db.Project
                .Where(x => x.Payment.Where(a => a.Id == id).Any())
                .Include(x => x.Payment)
                .First();

            var payment = project.Payment
                .Where(x => x.Id == id)
                .First();

            payment.CreatedAt = PersianDateTime.Parse(date).ToDateTime().Date;
            payment.Description = description;
            payment.PaymentType = type;
            payment.Price = price;
            payment.DesignerId = type == Enums.Enum.PaymentType.ToDesigner ? designerId : null;


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
                var credit = new CustomerRepository().Credit(project.CustomerId, project.Id);

                if (fromCredit > (credit + toCredit))
                {
                    return UnprocessableEntity(4);
                }
            }

            project.TotalPayment = positive - negative;

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
                return UnprocessableEntity(6);
            }

            db.SaveChanges();

            new CustomerRepository().Balance(project.CustomerId);

            return Ok();
        }
    }
}