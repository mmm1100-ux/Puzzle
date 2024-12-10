using Microsoft.EntityFrameworkCore.Infrastructure;
using Puzzle.Models;
using Puzzle.ViewModels.Feedbacks;

namespace Puzzle.Mapper
{
    public static class FeedbackFormMapper
    {
        public static FeedbackFormViewModel ToViewModel(this FeedbackForm model)
        {
            return new FeedbackFormViewModel
            {
                Id = model.Id,
                DesignerId = model.DesignerId,
                CustomerId = model.CustomerId,
                Description = model.Description,
                Vote = model.Vote,
                DesignQualityVote = model.DesignQualityVote,
                ComplianceOfTheOrderWithTheDesignVote = model.ComplianceOfTheOrderWithTheDesignVote,
                AppropriateApproachOfTheDesignerVote = model.AppropriateApproachOfTheDesignerVote,
                PriceVote = model.PriceVote,
                AppropriateTreatmentOfManagementAndOfficeWorkersVote = model.AppropriateTreatmentOfManagementAndOfficeWorkersVote,
                CustomerName = $"{model.Customer?.FirstName} {model.Customer?.LastName}" ?? model.Customer?.Phone,
                DesignerName = model.Designer?.Name,
            };
        }

        public static FeedbackForm ToModel(this UpsertFeedbackFormViewModel model)
        {
            return new FeedbackForm
            {
                Id = model.Id,
                DesignerId = model.DesignerId,
                CustomerId = model.CustomerId ?? default,
                Description = model.Description,
                Vote = model.Vote,
                DesignQualityVote = model.DesignQualityVote,
                ComplianceOfTheOrderWithTheDesignVote = model.ComplianceOfTheOrderWithTheDesignVote,
                AppropriateApproachOfTheDesignerVote = model.AppropriateApproachOfTheDesignerVote,
                PriceVote = model.PriceVote,
                AppropriateTreatmentOfManagementAndOfficeWorkersVote = model.AppropriateTreatmentOfManagementAndOfficeWorkersVote
            };
        }
    }
}
