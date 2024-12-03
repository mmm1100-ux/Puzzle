using Extensions;
using Helper;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Puzzle.Data;
using Puzzle.Helper;
using Puzzle.Models;
using Puzzle.ViewModels;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Puzzle.Controllers
{
    [TimeFilter]
    [Authorize(Roles = "Admin")]
    public class ProjectController : Controller
    {
        private readonly ProjectRepository projectRepository;
        private readonly DesignerRepository designerRepository;

        public ProjectController()
        {
            projectRepository = new ProjectRepository();
            designerRepository = new DesignerRepository();
        }

        public IActionResult Index(int? page = 1)
        {
            var project = projectRepository.ToList(null, null, page.Value);

            ViewData["Designer"] = designerRepository.GetList(true);
            ViewData["ShowCalender"] = true;
            ViewData["SimpleTitle"] = false;
            ViewData["ShowQuickAccess"] = true;

            var result = new ProjectViewModel { Page = page.Value, Project = project };

            return View(result);
        }

        [HttpGet("/project/open")]
        public IActionResult Open()
        {
            var project = projectRepository.Open();

            ViewData["Designer"] = designerRepository.GetList(true);
            ViewData["FormTitle"] = "تحویل نشده‌ها";

            var result = new ProjectViewModel { Project = project };

            return View("Index", result);
        }

        [HttpGet("/project/nodesigner")]
        public IActionResult NoDesigner()
        {
            var project = projectRepository.NoDesigner();

            ViewData["Designer"] = designerRepository.GetList(true);
            ViewData["FormTitle"] = "بدون طراح‌ها";

            var result = new ProjectViewModel { Project = project };

            return View("Index", result);
        }

        [HttpGet("/project/no-settlement")]
        public IActionResult NoSettlement()
        {
            var project = projectRepository.NoSettlement();

            ViewData["Designer"] = designerRepository.GetList(true);
            ViewData["FormTitle"] = "تسویه نشده‌ها";

            var result = new ProjectViewModel { Project = project };

            return View("Index", result);
        }

        [HttpGet("/project/follow")]
        public IActionResult Follow()
        {
            var project = projectRepository.Follow();

            ViewData["Designer"] = designerRepository.GetList(true);
            ViewData["FormTitle"] = "نیاز به پیگیری";

            var result = new ProjectViewModel { Project = project };

            return View("Index", result);
        }

        [HttpPost]
        public IActionResult Remove(int id)
        {
            var result = projectRepository.Remove(id);

            return Ok(result);
        }

        [HttpPost("/project/favorite")]
        public IActionResult Favorite(int id)
        {
            var userId = User.GetUserId();

            projectRepository.Favorite(id, userId);

            return Ok();
        }

        [HttpGet("/project/instagram/{id}")]
        public IActionResult Instagram(int id)
        {
            using var db = new PuzzleDbContext();

            //var media = db.Media
            //    .Where(x => x.ProjectId == id)
            //    .Where(x => x.Type == Enums.Enum.MediaType.Render)
            //    .ToList();

            var media = new List<Media>();

            var conversation = db.Conversation
                .Where(x => x.ProjectId == id)
                .Where(x => x.IsDelete == false)
                .Where(x => x.Type == Enums.Enum.MediaType.Render)
                .ToList();

            foreach (var item in conversation)
            {
                media.Add(new Media { Url = item.Attachment, CreatedAt = item.CreatedAt });
            }

            media = media.OrderByDescending(x => x.CreatedAt).ToList();

            return View(media);
        }

        [Route("/project/search")]
        public IActionResult Search(ProjectViewModel.Search model)
        {
            using var db = new PuzzleDbContext();

            if (model.Page <= 0)
            {
                model.Page = 1;
            }

            ViewData["JustProject"] = false;

            if (Request.Method == "POST")
            {
                ViewData["JustProject"] = true;
            }

            var result = new ProjectViewModel { Page = model.Page };

            ViewData["Designer"] = designerRepository.GetList(true);

            PersianDateTime? fromDate = null;
            PersianDateTime? toDate = null;

            if (model.FromDate != null)
            {
                fromDate = PersianDateTime.Parse(model.FromDate);
            }

            if (model.ToDate != null)
            {
                toDate = PersianDateTime.Parse(model.ToDate);
            }

            var query = db.Project
                .Where(x => x.IsDelete == false)
                .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                .Where(x => model.ProjectCategory == null || x.ProjectCategory == model.ProjectCategory)
                .Where(x => model.Type == null || x.Type == model.Type)
                .Where(x => model.DesignerId == null || x.DesignerId == model.DesignerId)
                .Where(x => fromDate == null || x.Receipt >= fromDate.Value.ToDateTime())
                .Where(x => toDate == null || x.Receipt <= toDate.Value.ToDateTime())
                .Where(x => model.FromPrice == null || (x.TotalPrice > 0 ? x.TotalPrice >= model.FromPrice : x.Approximate >= model.FromPrice))
                .Where(x => model.ToPrice == null || (x.TotalPrice > 0 ? x.TotalPrice <= model.ToPrice : x.Approximate <= model.ToPrice))
                .Where(x => string.IsNullOrEmpty(model.Title) || x.Title.Contains(model.Title));

            result.TotalPage = Math.Ceiling(query.Count() / 30.0).ToInt();

            result.Project = query.OrderByDescending(c => c.Receipt)
                .Skip((model.Page - 1) * 30)
                .Take(30)
                .Include(c => c.Customer)
                .Include(c => c.Designer)
                //.Include(c => c.Media)
                .Include(c => c.Child)
                .Include(c => c.Parent)
                .Include(x => x.Conversation)
                .Include(x => x.Favorite)
                .Include(x => x.ProjectReport)
                .ToList();

            return View(result);
        }
    }
}
