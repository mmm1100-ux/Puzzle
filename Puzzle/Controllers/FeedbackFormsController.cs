using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Services.Interfaces;
using Puzzle.ViewModels.Feedbacks;
using System.Threading.Tasks;

namespace Puzzle.Controllers
{
    [Route("[controller]/")]
    [Authorize]
    public class FeedbackFormsController : Controller
    {
        #region Fields

        private readonly IFeedbackFormService _feedbackFormService;

        #endregion

        #region Ctor

        public FeedbackFormsController(IFeedbackFormService feedbackFormService)
        {
            _feedbackFormService = feedbackFormService;
        }

        #endregion

        [HttpGet]
        public IActionResult Create(string designerId)
        {
            if (string.IsNullOrEmpty(designerId))
            {
                return BadRequest();
            }

            var model = new UpsertFeedbackFormViewModel
            {
                DesignerId = designerId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UpsertFeedbackFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _feedbackFormService.CreateAsync(model);

            if (result.IsFailure)
            {
                return View(model);
            }

            return RedirectToAction("Index", "Home",);
        }

    }
}
