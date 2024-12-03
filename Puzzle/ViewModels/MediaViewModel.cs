using Microsoft.AspNetCore.Http;
using static Enums.Enum;

namespace Puzzle.ViewModels
{
    public class MediaViewModel
    {
        public IFormFileCollection Media { get; set; }

        public MediaType Type { get; set; }

        public int ProjectId { get; set; }

        public string Description { get; set; }
    }
}
