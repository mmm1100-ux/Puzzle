using Extensions;
using Microsoft.EntityFrameworkCore;
using Models;
using Puzzle.Data;
using Puzzle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewModels;
using static Enums.Enum;

namespace Repository
{
    public class CustomerRepository
    {
        private readonly RatingRepository ratingRepository;

        public CustomerRepository()
        {
            ratingRepository = new RatingRepository();
        }

        public CustomerListViewModel ToList(int Page, OrderBy OrderBy, bool Asc, int Count, string keyword, string designerId = null)
        {
            var M = new CustomerListViewModel()
            {
                Customer = new List<CustomerViewModel>(),
                Page = new PageViewModel() { CurrentPage = Page },
                OrderBy = OrderBy,
                Asc = Asc
            };

            using var db = new PuzzleDbContext();

            var query = db.Customer
                .Where(x => designerId == null || x.Project.Where(a => a.DesignerId == designerId).Any())
                .Where(x => keyword == null || x.Mobile.Contains(keyword) || x.LastName.Contains(keyword) || x.FirstName.Contains(keyword) ||
                            (x.FirstName + " " + x.LastName).Contains(keyword) || (x.LastName + " " + x.FirstName).Contains(keyword) ||
                            x.Social.Contains(keyword) || x.Address.Contains(keyword) || x.Phone.Contains(keyword));

            if (OrderBy == OrderBy.Alphabet)
            {
                if (Asc)
                {
                    query = query.OrderBy(c => c.LastName);
                }
                else
                {
                    query = query.OrderByDescending(c => c.LastName);
                }
            }
            else if (OrderBy == OrderBy.LastVisitTime)
            {
                if (Asc)
                {
                    query = query.OrderByDescending(c => c.Project.OrderByDescending(p => p.Receipt).FirstOrDefault().Receipt);
                }
                else
                {
                    query = query.OrderBy(c => c.Project.OrderByDescending(p => p.Receipt).FirstOrDefault().Receipt);
                }
            }
            else if (OrderBy == OrderBy.NumberOfProject)
            {
                if (Asc)
                {
                    query = query.OrderBy(c => c.Project.Count);
                }
                else
                {
                    query = query.OrderByDescending(c => c.Project.Count);
                }

            }
            //else if (OrderBy == OrderBy.LastMonthRating)
            //{
            //    var Shamsi = DateTime.Now.ToShamsi().AddMonths(-1);
            //    Shamsi = new PersianDateTime(Shamsi.Year, Shamsi.Month, 1);

            //    if (Asc)
            //    {
            //        lstCustomer = _db.Rating.Where(c => c.DateTime == Shamsi.ToDateTime()).OrderBy(c => c.Grade).Select(c => c.Customer).ToList();
            //    }
            //    else
            //    {
            //        lstCustomer = _db.Rating.Where(c => c.DateTime == Shamsi.ToDateTime()).OrderByDescending(c => c.Grade).Select(c => c.Customer).ToList();
            //    }
            //}
            else if (OrderBy == OrderBy.LastMonthRating)
            {
                if (Asc)
                {
                    query = query.Where(x => x.Credit != 0).OrderByDescending(c => c.Credit);
                }
                else
                {
                    query = query.Where(x => x.Credit != 0).OrderBy(c => c.Credit);
                }
            }
            else if (OrderBy == OrderBy.Debtor)
            {
                if (Asc)
                {
                    query = query.Where(x => x.Balance < 0).OrderByDescending(c => c.Balance);
                }
                else
                {
                    query = query.Where(x => x.Balance < 0).OrderBy(c => c.Balance);
                }
            }
            else if (OrderBy == OrderBy.Creditor)
            {
                if (Asc)
                {
                    query = query.Where(x => x.Balance > 0).OrderBy(c => c.Balance);
                }
                else
                {
                    query = query.OrderByDescending(c => c.Balance);
                }
            }
            else if (OrderBy == OrderBy.Balance)
            {
                if (Asc)
                {
                    query = query.OrderBy(c => c.Balance);
                }
                else
                {
                    query = query.OrderByDescending(c => c.Balance);
                }
            }
            else if (OrderBy == OrderBy.Credit)
            {
                if (Asc)
                {
                    query = query.OrderBy(c => c.Credit);
                }
                else
                {
                    query = query.OrderByDescending(c => c.Credit);
                }
            }
            else if (OrderBy == OrderBy.LastActivity)
            {
                if (Asc)
                {
                    query = query.OrderBy(c => c.LastActivity);
                }
                else
                {
                    query = query.OrderByDescending(c => c.LastActivity);
                }
            }
            else if (OrderBy == OrderBy.Support)
            {
                if (Asc)
                {
                    query = query.Where(x => x.CustomerChat.Any())
                        .Where(x => x.CustomerChat.Where(a => a.UnreadByAdmin).Any() || (x.DoneByAdmin == false))
                        .OrderBy(c => c.CustomerChat.OrderBy(x => x.CreatedAt).Last().CreatedAt);
                }
                else
                {
                    query = query.Where(x => x.CustomerChat.Any())
                        .Where(x => x.CustomerChat.Where(a => a.UnreadByAdmin).Any() || (x.DoneByAdmin == false))
                        .OrderByDescending(c => c.CustomerChat.OrderBy(x => x.CreatedAt).Last().CreatedAt);
                }
            }
            //else if (OrderBy == OrderBy.Critical)
            //{
            //    if (Asc)
            //    {
            //        var date = DateTime.Today.AddDays(-30);
            //        lstCustomer = await _db.Rating.Where(c => c.Grade > 3)
            //            .Where(c => c.Customer.Project.Any())
            //            .Where(c => c.Customer.Project.Max(p => p.Receipt) < date)
            //            .OrderBy(c => c.Customer.Project.Max(p => p.Receipt))
            //            .Select(c => c.Customer)
            //            .ToListAsync();
            //    }
            //    else
            //    {
            //        var date = DateTime.Today.AddDays(-30);
            //        lstCustomer = await _db.Rating.Where(c => c.Grade > 3)
            //            .Where(c => c.Customer.Project.Any())
            //            .Where(c => c.Customer.Project.Max(p => p.Receipt) < date)
            //            .OrderByDescending(c => c.Customer.Project.Max(p => p.Receipt))
            //            .Select(c => c.Customer)
            //            .ToListAsync();
            //    }
            //}
            //else if (OrderBy == OrderBy.Special)
            //{
            //    if (Asc)
            //    {
            //        lstCustomer = _db.Customer.Where(c => c.IsSpecial == true)
            //            .OrderByDescending(c => c.Project.OrderByDescending(p => p.Receipt)
            //            .FirstOrDefault().Receipt)
            //            .ToList();
            //    }
            //    else
            //    {
            //        lstCustomer = _db.Customer.Where(c => c.IsSpecial == true)
            //            .OrderBy(c => c.Project.OrderByDescending(p => p.Receipt)
            //            .FirstOrDefault().Receipt)
            //            .ToList();
            //    }
            //}

            M.Filter = new FilterViewModel() { Count = query.Count() };
            M.Page.TotalPage = (int)Math.Ceiling((decimal)query.Count() / Count);

            var lstCustomer = query.Skip((Page - 1) * Count).Take(Count).ToList();

            foreach (var item in lstCustomer)
            {
                var customer = ToViewModel(item);
                customer.LastTime = ratingRepository.GetLastTime(item.Id)?.GetElapsedTime();
                customer.TotalProject = ratingRepository.GetTotalProject(item.Id);
                customer.CustomerChat = db.CustomerChat.Where(x => x.CustomerId == item.Id).ToList();
                M.Customer.Add(customer);
            }

            return M;
        }

        public Customer GetByMobile(string Mobile)
        {
            using var db = new PuzzleDbContext();

            Mobile = "0" + Mobile.Substring(Mobile.Length - 10, 10);

            return db.Customer
                .Where(x => x.Mobile == Mobile)
                .FirstOrDefault();
        }

        public CustomerViewModel GetById(int Id)
        {
            using var db = new PuzzleDbContext();
            var X = db.Customer.FirstOrDefault(c => c.Id == Id);
            return ToViewModel(X);
        }

        public SearchViewModel Search(string Keyword, int Page, int Count = 10, string designerId = null)
        {
            using var db = new PuzzleDbContext();


            var query = db.Customer.Where(x => designerId == null || x.Project.Where(a => a.DesignerId == designerId).Any()).Where(c => Keyword == null || c.Mobile.Contains(Keyword) ||
                    c.LastName.Contains(Keyword) || c.FirstName.Contains(Keyword) ||
                    (c.FirstName + " " + c.LastName).Contains(Keyword) || (c.LastName + " " + c.FirstName).Contains(Keyword) ||
                    c.Social.Contains(Keyword) || c.Address.Contains(Keyword) || c.Phone.Contains(Keyword));

            var totalRow = query.Count();

            var S = new SearchViewModel()
            {
                Customer = new List<CustomerViewModel>(),

                Filter = new FilterViewModel()
                {
                    Count = totalRow,
                    Keyword = Keyword
                },

                Page = new PageViewModel()
                {
                    CurrentPage = Page,
                    TotalPage = (int)Math.Ceiling((decimal)totalRow / Count)
                }
            };

            var lstCustomer = query.OrderByDescending(c => c.Project.Any() ? c.Project.Where(a => designerId == null || a.DesignerId == designerId).Max(p => p.Receipt) : DateTime.MinValue)
                .Skip((Page - 1) * Count)
                .Take(Count)
                .Include(c => c.Project)
                .ToList();

            foreach (var item in lstCustomer)
            {
                var customer = ToViewModel(item);
                customer.LastTime = ratingRepository.GetLastTime(item.Id, designerId)?.GetElapsedTime();
                customer.TotalProject = ratingRepository.GetTotalProject(item.Id, designerId);

                S.Customer.Add(customer);
            }

            return S;
        }

        public CustomerViewModel Project(int Id)
        {
            using var db = new PuzzleDbContext();

            var X = db.Customer
                .Include(c => c.Project).ThenInclude(c => c.Designer)
                //.Include(c => c.Project).ThenInclude(c => c.Media)
                .First(c => c.Id == Id);

            CustomerViewModel M = ToViewModel(X);

            M.Project = new ProjectRepository().GetByCustomerId(Id, 1, 0);

            M.LastTime = ratingRepository.GetLastTime(Id)?.GetElapsedTime();

            M.TotalProject = ratingRepository.GetTotalProject(Id);

            M.SumLastMonth = ratingRepository.GetSumLastMonth(Id);

            M.TimeOfMembership = ratingRepository.GetTimeOfMembership(Id);

            M.Transaction = db.Transaction
                .Where(x => x.CustomerId == Id)
                .Where(x => x.IsDelete == false)
                .OrderByDescending(x => x.Id)
                //.Take(20)
                .Include(x => x.Designer)
                .ToList();

            return M;
        }

        public void Balance(int customerId)
        {
            using var db = new PuzzleDbContext();

            var customer = db.Customer
                .Where(x => x.Id == customerId)
                .Include(c => c.Project).ThenInclude(x => x.Parent)
                .First();

            int totalPrice = 0;
            int totalPayment = 0;

            foreach (var item in customer.Project.Where(x => x.IsDelete == false).Where(x => x.ProjectId == null || x.Parent.IsDelete == false))
            {
                totalPrice += item.TotalPrice > 0 ? item.TotalPrice : item.Approximate;
                totalPayment += item.TotalPayment;
            }

            customer.Balance = totalPayment - totalPrice;

            db.SaveChanges();

            Credit(customerId);
        }

        public void Credit(int customerId)
        {
            using var db = new PuzzleDbContext();

            var customer = db.Customer.Where(x => x.Id == customerId).First();

            var toCredit = db.Payment
                .Where(x => x.Project.IsDelete == false)
                .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                .Where(c => c.Project.CustomerId == customerId)
                .Where(c => c.PaymentType == PaymentType.ToCredit)
                .Sum(c => c.Price);

            var fromCredit = db.Payment
                .Where(x => x.Project.IsDelete == false)
                .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                .Where(c => c.Project.CustomerId == customerId)
                .Where(c => c.PaymentType == PaymentType.FromCredit)
                .Sum(c => c.Price);

            customer.Credit = toCredit - fromCredit;

            var transaction = db.Transaction.Where(x => x.IsDelete == false).Where(x => x.CustomerId == customerId).ToList();

            customer.Credit += transaction.Where(c => c.PaymentType != PaymentType.Return).Sum(c => c.Price);
            customer.Credit -= transaction.Where(c => c.PaymentType == PaymentType.Return).Sum(c => c.Price);

            db.SaveChanges();
        }

        public int Credit(int customerId, int projectId)
        {
            using var db = new PuzzleDbContext();

            var toCredit = db.Payment
                .Where(x => x.Project.IsDelete == false)
                .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                .Where(c => c.Project.CustomerId == customerId)
                .Where(c => c.PaymentType == PaymentType.ToCredit)
                .Where(c => c.ProjectId != projectId)
                .Sum(c => c.Price);

            var fromCredit = db.Payment
                .Where(x => x.Project.IsDelete == false)
                .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                .Where(c => c.Project.CustomerId == customerId)
                .Where(c => c.PaymentType == PaymentType.FromCredit)
                .Where(c => c.ProjectId != projectId)
                .Sum(c => c.Price);

            var credit = toCredit - fromCredit;

            var transaction = db.Transaction.Where(x => x.IsDelete == false).Where(x => x.CustomerId == customerId).ToList();

            credit += transaction.Where(c => c.PaymentType != PaymentType.Return).Sum(c => c.Price);
            credit -= transaction.Where(c => c.PaymentType == PaymentType.Return).Sum(c => c.Price);

            return credit;
        }

        public bool Duplicate(int customerId)
        {
            using var db = new PuzzleDbContext();

            var X = db.Project.Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.CustomerId == customerId && c.Receipt == DateTime.Today)
                .Any();

            return X;
        }

        public void Update(CustomerViewModel customer)
        {
            using var db = new PuzzleDbContext();

            var X = db.Customer.FirstOrDefault(c => c.Id == customer.Id);

            X.Address = customer.Address;
            X.Birthday = customer.Birthday?.ToDateTime();
            X.Mobile = "0" + customer.Mobile.Substring(customer.Mobile.Length - 10, 10);
            X.FirstName = customer.FirstName;
            X.LastName = customer.LastName;
            X.Phone = customer.Phone;
            X.Social = customer.Social;
            X.IsSpecial = customer.IsSpecial;
            X.Description = customer.CustomerDescription;

            db.SaveChanges();
        }

        public void Remove(int Id)
        {
            using var db = new PuzzleDbContext();

            var X = db.Customer.Include(x => x.Token).FirstOrDefault(c => c.Id == Id);

            db.Customer.Remove(X);

            db.SaveChanges();
        }

        public CustomerViewModel ToViewModel(Customer C)
        {
            return new CustomerViewModel()
            {
                Id = C.Id,
                Address = C.Address,
                Phone = C.Phone,
                Birthday = C.Birthday?.ToShamsi(),
                Mobile = C.Mobile,
                LastName = C.LastName,
                FirstName = C.FirstName,
                Social = C.Social,
                IsSpecial = C.IsSpecial,
                //Project = new List<ProjectViewModel>(),
                CustomerDescription = C.Description,
                Balance = C.Balance,
                Credit = C.Credit,
                LastActivity = C.LastActivity,
                DoneByAdmin = C.DoneByAdmin
            };
        }

        public PrintViewModel Print(int customerId)
        {
            using var db = new PuzzleDbContext();

            var result = new PrintViewModel
            {
                Customer = GetById(customerId),

                Project = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(c => c.CustomerId == customerId)
                .Where(c => c.TotalPrice > 0)
                .Where(c => c.TotalPrice != c.TotalPayment)
                .ToList()
            };

            return result;
        }
    }
}
