using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Puzzle.Data;
using Puzzle.Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Controllers
{
    public class TransactionController : Controller
    {
        private readonly DesignerRepository designerRepository;
        private readonly CustomerRepository customerRepository;

        public TransactionController()
        {
            designerRepository = new DesignerRepository();
            customerRepository = new CustomerRepository();
        }

        public IActionResult Add(Transaction model)
        {
            using var db = new PuzzleDbContext();

            model.CreatedAt = DateTime.Today;

            if (model.PaymentType != Enums.Enum.PaymentType.ToDesigner)
            {
                model.DesignerId = null;
            }

            db.Transaction.Add(model);

            db.SaveChanges();

            customerRepository.Balance(model.CustomerId);

            return Ok();
        }

        //public IActionResult Update(Transaction model)
        //{
        //    using var db = new PuzzleDbContext();

        //    var transaction = db.Transaction
        //        .Where(x => x.IsDelete == false)
        //        .Where(x => x.Id == model.Id)
        //        .First();

        //    transaction.PaymentType = model.PaymentType;
        //    transaction.Price = model.Price;

        //    db.SaveChanges();

        //    return Ok();
        //}

        public IActionResult Remove(int id)
        {
            using var db = new PuzzleDbContext();

            var transaction = db.Transaction
                .Where(x => x.IsDelete == false)
                .Where(x => x.Id == id)
                .Where(x => x.CreatedAt.Date == DateTime.Today)
                .First();

            transaction.IsDelete = true;

            db.SaveChanges();

            customerRepository.Balance(transaction.CustomerId);

            return Ok();
        }

        [HttpPost]
        public IActionResult GetList(int customerId, int page = 1)
        {
            using var db = new PuzzleDbContext();

            var transaction = db.Transaction
                .Where(x => x.IsDelete == false)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.Id)
                //.Skip((page - 1) * 20)
                //.Take(20)
                .Include(x => x.Designer)
                .ToList();

            var designerIds = transaction.Select(x => x.DesignerId).ToList();

            ViewData["Designer"] = designerRepository.GetList(true, designerIds);

            return View("_TransactionPartial", transaction);
        }
    }
}
