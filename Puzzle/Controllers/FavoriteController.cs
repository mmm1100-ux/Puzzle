using Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Puzzle.Data;
using Puzzle.Helper;
using Puzzle.ViewModels;
using Repository;
using System;
using System.Linq;

namespace Puzzle.Controllers
{
    [TimeFilter]
    [Authorize(Roles = "Admin")]
    public class FavoriteController : Controller
    {
        private readonly DesignerRepository designerRepository;

        public FavoriteController()
        {
            designerRepository = new DesignerRepository();
        }

        public IActionResult Index(int? page = 1)
        {
            using var db = new PuzzleDbContext();

            var result = new ProjectViewModel { Page = page.Value };

            var query = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(x => x.Favorite.Any());

            result.TotalPage = Math.Ceiling(query.Count() / 20.0).ToInt();

            result.Project = query.OrderByDescending(x => x.Favorite.First().Id).ThenByDescending(x => x.Id)
                .Skip((page.Value - 1) * 20)
                .Take(20)
                .Include(c => c.Customer)
                .Include(c => c.Designer)
                //.Include(x => x.Media)
                .Include(x => x.Parent)
                .Include(x => x.Conversation).ThenInclude(x=>x.ConversationFavorite)
                .Include(x => x.Favorite).ThenInclude(x => x.User)
                .Include(x => x.ProjectReport)
                .ToList();

            ViewData["Designer"] = designerRepository.GetList(true);
            ViewData["ShowDelivery"] = false;
            ViewData["ShowOrderType"] = false;
            ViewData["ShowFavoriteUser"] = true;
            ViewData["FormTitle"] = "علاقه‌مندی‌ها";

            return View(result);
        }
    }
}
