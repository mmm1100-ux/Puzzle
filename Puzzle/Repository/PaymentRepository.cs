using Microsoft.EntityFrameworkCore;
using Models;
using Puzzle.Data;
using Puzzle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewModels;

namespace Repository
{
    public class PaymentRepository
    {
        public List<PaymentViewModel> GetList(int page)
        {
            using var db = new PuzzleDbContext();

            var payment = db.Payment
                .Where(c => c.CreatedAt.Date == DateTime.Today.AddDays(-page + 1))
                .Where(x => x.Project.IsDelete == false)
                .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                .Include(c => c.Project).ThenInclude(c => c.Customer)
                .Include(c => c.Project).ThenInclude(c => c.Designer)
                .Include(c => c.Designer)
                .OrderByDescending(c => c.Id)
                .ToList();

            var result = new List<PaymentViewModel>();

            foreach (var item in payment)
            {
                result.Add(new PaymentViewModel
                {
                    Balance = item.Project.Customer.Balance,
                    Credit = item.Project.Customer.Credit,
                    CreatedAt = item.CreatedAt,
                    Project = item.Project,
                    PaymentType = item.PaymentType,
                    Price = item.Price,
                    Id = item.Id,
                    Designer = item.Designer,
                    Customer = item.Project.Customer,
                    Receipt = item.Project.Receipt.Date,
                    Description = item.Description
                });
            }

            var transaction = db.Transaction
                .Where(x => x.IsDelete == false)
                .Where(x => x.CreatedAt.Date == DateTime.Today.AddDays(-page + 1))
                .Include(x => x.Customer)
                .Include(x => x.Designer)
                .ToList();

            foreach (var item in transaction)
            {
                result.Add(new PaymentViewModel
                {
                    Balance = item.Customer.Balance,
                    Credit = item.Customer.Credit,
                    CreatedAt = item.CreatedAt,
                    //Project = item.Project,
                    PaymentType = item.PaymentType,
                    Price = item.Price,
                    Id = item.Id,
                    Designer = item.Designer,
                    IsTransaction = true,
                    Customer = item.Customer,
                    Receipt = item.CreatedAt.Date,
                    Description = item.Description
                });
            }

            return result;
        }

        public List<Payment> GetByDesignerId(string designerId, DateTime from, DateTime to, int page)
        {
            using var db = new PuzzleDbContext();

            var result = db.Payment
                .Where(x => x.Project.IsDelete == false)
                .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                .Where(c => c.Project.DesignerId == designerId)
                .Where(c => c.CreatedAt >= from)
                .Where(c => c.CreatedAt <= to)
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * 10)
                .Take(10)
                .Include(c => c.Project).ThenInclude(c => c.Customer)
                .ToList();

            return result;
        }

        public int CountByDesignerId(string designerId, DateTime from, DateTime to)
        {
            using var db = new PuzzleDbContext();

            return db.Payment
                .Where(x => x.Project.IsDelete == false)
                .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                .Where(c => c.Project.DesignerId == designerId)
                .Where(c => c.CreatedAt >= from)
                .Where(c => c.CreatedAt <= to)
                .Count();
        }
    }
}
