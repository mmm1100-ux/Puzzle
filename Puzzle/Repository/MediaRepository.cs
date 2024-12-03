using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Puzzle.Data;
using System.Linq;
using System.Security.Claims;
using static Enums.Enum;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Puzzle.Repository
{
    public class MediaRepository
    {
        //public int Count()
        //{
        //    using var db = new PuzzleDbContext();

        //    return db.Media.Where(x => x.Status == Enums.Enum.MediaStatus.New).Count();
        //}

        public int Unread()
        {
            using var db = new PuzzleDbContext();

            return db.Conversation
                .Where(x => x.Project.IsDelete == false)
                .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                .Where(x => x.IsDelete == false)
                .Where(x => x.UnreadByAdmin == true)
                .Count();
        }

        public int UnreadCustomerChat()
        {
            using var db = new PuzzleDbContext();

            return db.CustomerChat
                .Where(x => x.IsDelete == false)
                .Where(x => x.UnreadByAdmin == true)
                .Count();
        }

        public int UnreadProjectReport()
        {
            using var db = new PuzzleDbContext();

            return db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(x => !x.PrintDelivery && !x.Whatsapp && !x.Telegram && !x.Other && !x.Soroush && !x.Eitaa && !x.Rubika && !x.Flash && !x.App)
                .Where(x => x.ProjectReport.Where(x => x.ReadByAdmin == false).Any() == true)
                .Where(x => (string.IsNullOrWhiteSpace(x.ProjectReport.OrderByDescending(x=>x.Id).First().Description) == false) || ((x.ProjectReport.OrderByDescending(x => x.Id).First().Reason != Enums.Enum.ProjectReportReason.UntilNoon) && (x.ProjectReport.OrderByDescending(x => x.Id).First().Reason != Enums.Enum.ProjectReportReason.UntilEvening)))
                .Count(); 
        }

        public int UnreadByDesigner(string userId)
        {
            using var db = new PuzzleDbContext();

            return db.Conversation
                .Where(x => x.Project.IsDelete == false)
                .Where(x => x.Project.ProjectId == null || x.Project.Parent.IsDelete == false)
                .Where(x => x.IsDelete == false)
                .Where(x => x.Project.DesignerId == userId)
                .Where(x => x.UnreadByDesigner == true)
                .Where(x => x.Accepted)
                .Count();
        }
        
        public int GalleryCount(string category)
        {
            using var db = new PuzzleDbContext();

            var query = db.Project
    .Where(x => x.Favorite.Any())
    .Where(x => x.Conversation.Where(a => a.ConversationFavorite.Any()).Any());

            if (category == "cabinet")
            {
                query = query.Where(x => x.ProjectCategory == Enums.Enum.ProjectCategory.Cabinet_Modern || x.ProjectCategory == Enums.Enum.ProjectCategory.Cabinet_Classic ||
                x.ProjectCategory == Enums.Enum.ProjectCategory.Cabinet_NeoClassic || x.ProjectCategory == Enums.Enum.ProjectCategory.Cabinet_Enzo ||
                x.ProjectCategory == Enums.Enum.ProjectCategory.Cabinet_Other ||
                x.ProjectCategory == Enums.Enum.ProjectCategory.ModernCabinet || x.ProjectCategory == Enums.Enum.ProjectCategory.ClassicCabinet ||
                x.ProjectCategory == Enums.Enum.ProjectCategory.ModernVray || x.ProjectCategory == Enums.Enum.ProjectCategory.ClassicVray);
            }
            else if (category == "decor")
            {
                query = query.Where(x => x.ProjectCategory == Enums.Enum.ProjectCategory.Decor_Closet || x.ProjectCategory == Enums.Enum.ProjectCategory.Decor_Full ||
                x.ProjectCategory == Enums.Enum.ProjectCategory.Decor_Other ||
                x.ProjectCategory == Enums.Enum.ProjectCategory.Decor || x.ProjectCategory == Enums.Enum.ProjectCategory.DecorVray);
            }
            else if (category == "official")
            {
                query = query.Where(x => x.ProjectCategory == Enums.Enum.ProjectCategory.Official);
            }
            else if (category == "area")
            {
                query = query.Where(x => x.ProjectCategory == Enums.Enum.ProjectCategory.Facade);
            }

            return query.Count();
        }
    }
}
