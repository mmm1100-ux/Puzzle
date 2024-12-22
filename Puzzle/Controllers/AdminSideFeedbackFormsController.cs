using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Services.Interfaces;
using Puzzle.ViewModels.Feedbacks;
using Repository;
using System.Threading.Tasks;

namespace Puzzle.Controllers
{
    [Route("[controller]/")]
    [Authorize(Roles = "Admin")]
    public class AdminSideFeedbackFormsController : Controller
    {
        #region Fields

        private readonly IFeedbackFormService _feedbackFormService;

        #endregion

        #region Ctor

        public AdminSideFeedbackFormsController(IFeedbackFormService feedbackFormService)
        {
            _feedbackFormService = feedbackFormService;
        }

        #endregion

        [HttpGet("[action]")]
        public async Task<IActionResult> Filter(FilterFeedbackFormViewModel model)
        {
            if (model is null)
            {
                return BadRequest();
            }

            var result = await _feedbackFormService.Filter(model);

            if (result.IsFailure)
            {
                return BadRequest();
            }

            ViewData["FormTitle"] = "نظرسنجی ها";
            ViewData["ShowCalender"] = false;
            ViewBag.Designer = new DesignerRepository().GetList(true);
            ViewBag.CustomerList = new CustomerRepository().ToList(1, Enums.Enum.OrderBy.Alphabet, true, int.MaxValue, null, isHaveFirstNameOrLastName: true).Customer;

            return View(model);
        }

        //[HttpDelete("[action]/{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    if (id <= 0)
        //    {
        //        return BadRequest();
        //    }

        //    var result = await _feedbackFormService.DeleteAsync(id);

        //    if (result.IsFailure)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return RedirectToAction(nameof(Filter), new FilterFeedbackFormViewModel());
        //}
    }
}
