using Puzzle.Shared;
using System.ComponentModel.DataAnnotations;

namespace Puzzle.ViewModels.Feedbacks
{
    public class UpsertFeedbackFormViewModel : BaseViewModel<int>
    {
        [Display(Name = "طراح")]
        [Required(ErrorMessage = "لطفا {0} این را وارد کنید")]
        public string DesignerId { get; set; }

        [Display(Name = "مشتری")]
        //[Required(ErrorMessage = "لطفا {0} این را وارد کنید")]
        public int? CustomerId { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "امتیاز")]
        [Range(1, 5, ErrorMessage = "{0} باید بین {1} و {2} باشد")]
        public int Vote { get; set; }

        [Display(Name = "کیفیت طراحی")]
        [Range(1, 5, ErrorMessage = "{0} باید بین {1} و {2} باشد")]
        public int? DesignQualityVote { get; set; }

        [Display(Name = "تطابق سفارش با طراحی")]
        [Range(1, 5, ErrorMessage = "{0} باید بین {1} و {2} باشد")]
        public int? ComplianceOfTheOrderWithTheDesignVote { get; set; } 
        
        [Display(Name = "زمان تحویل")]
        [Range(1, 5, ErrorMessage = "{0} باید بین {1} و {2} باشد")]
        public int? DeliveryTimeVote { get; set; }

        [Display(Name = "رفتار مناسب طراح")]
        [Range(1, 5, ErrorMessage = "{0} باید بین {1} و {2} باشد")]
        public int? AppropriateApproachOfTheDesignerVote { get; set; }

        [Display(Name = "قیمت")]
        [Range(1, 5, ErrorMessage = "{0} باید بین {1} و {2} باشد")]
        public int? PriceVote { get; set; }

        [Display(Name = "رفتار مناسب مدیران و کارمندان")]
        [Range(1, 5, ErrorMessage = "{0} باید بین {1} و {2} باشد")]
        public int? AppropriateTreatmentOfManagementAndOfficeWorkersVote { get; set; }

        public void Sanitize()
        {
            Description = Description?.Trim();
        }
    }
}
