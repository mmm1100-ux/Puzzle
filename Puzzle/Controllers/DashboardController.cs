using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    public class DashboardController : Controller
    {
        //public IActionResult Index()
        //{
        //    using var db = new PuzzleDbContext();

        //    var lastDayOfMonth = new PersianDateTime(DateTime.Now).GetPersianDateOfLastDayOfMonth();
        //    var firstDayOfMonth = lastDayOfMonth.AddDays(-(lastDayOfMonth.GetMonthDays - 1));

        //    var totalProject = db.Project
        //        .Where(x => x.Receipt.Date >= firstDayOfMonth)
        //        .Where(x => x.Receipt.Date <= lastDayOfMonth)
        //        .Where(x => x.ProjectCategory != Enums.Enum.ProjectCategory.Edit)
        //        .Where(x => x.ProjectCategory != Enums.Enum.ProjectCategory.Amendment)
        //        .Where(x => x.ProjectCategory != Enums.Enum.ProjectCategory.Print)
        //        .Count();

        //    var payment = db.Payment
        //        .Where(x => x.CreatedAt.Date >= firstDayOfMonth)
        //        .Where(x => x.CreatedAt.Date <= lastDayOfMonth)
        //        .ToList();

        //    var totalPayment = payment.Where(x => x.PaymentType != Enums.Enum.PaymentType.FromCredit)
        //        .Where(x => x.PaymentType != Enums.Enum.PaymentType.ToCredit)
        //        .Where(x => x.PaymentType != Enums.Enum.PaymentType.Return)
        //        .Select(x => x.Price)
        //        .Sum()
        //        -
        //        payment.Where(x => x.PaymentType == Enums.Enum.PaymentType.Return).Select(x => x.Price)
        //        .Sum();

        //    var totalUser = db.Project
        //        .Where(x => x.Receipt.Date >= firstDayOfMonth)
        //        .Where(x => x.Receipt.Date <= lastDayOfMonth)
        //        .GroupBy(x => x.CustomerId)
        //        .Count();

        //    var newUser = db.Customer
        //        .Where(x => x.Project.Where(a => a.Receipt <= firstDayOfMonth).Any() == false)
        //        .Where(x => x.Project.Where(a => a.Receipt.Date >= firstDayOfMonth && a.Receipt.Date <= lastDayOfMonth).Any() == true)
        //        .Count();

        //    var result = new Dashboard
        //    {
        //        NewCustomer = newUser,
        //        TotalCustomer = totalUser,
        //        TotalPayment = totalPayment,
        //        TotalProject = totalProject
        //    };

        //    return View(result);
        //}
    }
}