using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using static Enums.Enum;

namespace ViewModels
{
    public class ConversationViewModel
    {
        public class Add
        {
            public int Id { get; set; }

            public int ProjectId { get; set; }

            public string Message { get; set; }

            public IFormFile Attachment { get; set; }

            public MediaType? Type { get; set; }

            public int? ConversationId { get; set; }
        }

        public class AddImage
        {
            public int ProjectId { get; set; }

            public string Message { get; set; }

            public List<IFormFile> Attachment { get; set; }

            public MediaType Type { get; set; }
        }

        public class Accepted
        {
            public bool Status { get; set; }

            public List<int> ConversationId { get; set; }
        }

        public class Remove
        {
            public int Id { get; set; }
        }
    }
}
