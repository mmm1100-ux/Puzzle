using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Puzzle.Controllers
{
    [Authorize(Roles ="Admin")]
    public class BaseAdminController : Controller
    {
        public static string SuccessMessage = "SuccessMessage";
        public static string ErrorMessage = "ErrorMessage";
        public static string InfoMessage = "InfoMessage";
        public static string WarningMessage = "WarningMessage";
        public static string ErrorMessageWithURL = "ErrorMessageWithURL";
    }
}
