using Helper;
using Microsoft.EntityFrameworkCore;
using Models;
using Puzzle.Data;
using Puzzle.Models;
using Puzzle.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Repository
{
    public class DepositRepository
    {
        public Deposit GetById(int id)
        {
            using var db = new PuzzleDbContext();

            var result = db.Deposit.First(c => c.Id == id);

            return result;
        }

        public List<Deposit> GetByDesignerId(string designerId)
        {
            using var db = new PuzzleDbContext();

            var result = db.Deposit
                .Where(c => c.DesignerId == designerId)
                .Include(c => c.Designer)
                .ToList();

            var payment = db.Payment
                .Where(x => x.DesignerId == designerId)
                .Where(x => x.Project.IsDelete == false)
                .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                .Include(x => x.Designer)
                .Include(x => x.Project).ThenInclude(x => x.Customer)
                .ToList();

            foreach (var item in payment)
            {
                result.Add(new Deposit
                {
                    CreatedAt = item.CreatedAt,
                    Description = "ProjectId=" + (item.Project.ProjectId == null ? item.ProjectId : item.Project.ProjectId + "?id=" + item.ProjectId) + ";Customer=" + item.Project.Customer.FirstName + " " + item.Project.Customer.LastName,
                    DesignerId = item.DesignerId,
                    Designer = item.Designer,
                    Price = item.Price
                });
            }

            var transaction = db.Transaction
               .Where(x => x.IsDelete == false)
               .Where(x => x.DesignerId == designerId)
               .Include(x => x.Customer)
               .Include(x => x.Designer)
               .ToList();

            foreach (var item in transaction)
            {
                result.Add(new Deposit
                {
                    CreatedAt = item.CreatedAt,
                    Description = "CustomerId=" + item.CustomerId + ";Customer=" + item.Customer.FirstName + " " + item.Customer.LastName,
                    DesignerId = item.DesignerId,
                    Designer = item.Designer,
                    Price = item.Price
                });
            }

            return result.OrderByDescending(x => x.CreatedAt).ToList();
        }

        public DepositViewModel GetList(int page)
        {
            using var db = new PuzzleDbContext();

            var result = new DepositViewModel { Page = page };

            var deposit = db.Deposit.Include(x => x.Designer).ToList();

            var payment = db.Payment
                .Where(x => x.DesignerId != null)
                .Where(x => x.Project.IsDelete == false)
                .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                .Include(x => x.Designer)
                .Include(x => x.Project).ThenInclude(x => x.Customer)
                .ToList();

            foreach (var item in payment)
            {
                deposit.Add(new Deposit
                {
                    CreatedAt = item.CreatedAt,
                    Description = "ProjectId=" + (item.Project.ProjectId == null ? item.ProjectId : item.Project.ProjectId + "?id=" + item.ProjectId) + ";Customer=" + item.Project.Customer.FirstName + " " + item.Project.Customer.LastName,
                    DesignerId = item.DesignerId,
                    Designer = item.Designer,
                    Price = item.Price
                });
            }

            var transaction = db.Transaction
                .Where(x => x.IsDelete == false)
                .Where(x => x.PaymentType == Enums.Enum.PaymentType.ToDesigner)
                .Include(x => x.Customer)
                .Include(x => x.Designer)
                .ToList();

            foreach (var item in transaction)
            {
                deposit.Add(new Deposit
                {
                    CreatedAt = item.CreatedAt,
                    Description = "CustomerId=" + item.CustomerId + ";Customer=" + item.Customer.FirstName + " " + item.Customer.LastName,
                    DesignerId = item.DesignerId,
                    Designer = item.Designer,
                    Price = item.Price
                });
            }

            result.TotalPage = Math.Ceiling(deposit.Count / 30.0).ToInt();

            result.Deposit = deposit.OrderByDescending(x => x.CreatedAt)
                .Skip((page - 1) * 30)
                .Take(30)
                .ToList();

            return result;
        }

        public void Add(Deposit deposit)
        {
            using var db = new PuzzleDbContext();

            db.Deposit.Add(deposit);

            db.SaveChanges();
        }

        public void Update(Deposit deposit)
        {
            using var db = new PuzzleDbContext();

            db.Deposit.Update(deposit);

            db.SaveChanges();
        }
    }
}
