using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Services.Interfaces;
using Puzzle.ViewModels.Feedbacks;
using System.Data.Odbc;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Controllers
{
    [Route("[controller]/")]
    //[Authorize]
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

        [HttpGet("[action]/{designerId}")]
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

        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody] UpsertFeedbackFormViewModel model)
        {
            model.DesignerId = "108ceaaf-84e6-439a-baf9-dbac522f570c";

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(string.Join(", ", errors)); // Return validation errors as a response
            }

            var result = await _feedbackFormService.CreateAsync(model);

            if (result.IsFailure)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }


        [HttpGet("/FeedbackForms/LoadModalContent")]
        public IActionResult LoadModalContent(string designerId)
        {
            if (string.IsNullOrEmpty(designerId))
            {
                return BadRequest("DesignerId is required.");
            }

            var model = new UpsertFeedbackFormViewModel
            {
                DesignerId = designerId
            };

            // بازگرداندن یک PartialView که محتوای مودال را رندر می‌کند
            return PartialView("Create", model);
        }


    }
}
