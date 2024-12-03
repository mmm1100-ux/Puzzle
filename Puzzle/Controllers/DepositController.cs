using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Puzzle.Helper;
using Puzzle.Models;
using Repository;
using System;

namespace Controllers
{
    [TimeFilter]
    [Authorize(Roles = "Manager")]
    public class DepositController : Controller
    {
        private readonly DepositRepository _depositRepository;
        private readonly DesignerRepository _designerRepository;
        private readonly ProjectRepository _projectRepository;
        private readonly IMemoryCache _memoryCache;

        public DepositController(IMemoryCache memoryCache)
        {
            _depositRepository = new DepositRepository();
            _designerRepository = new DesignerRepository();
            _projectRepository = new ProjectRepository();
            _memoryCache = memoryCache;
        }

        public IActionResult Index(int? page = 1)
        {
            if (_designerRepository.IsValid(HttpContext, _memoryCache) == false)
            {
                return RedirectToAction("VerifyCode", "Designer", new { url = "/deposit" });
            }

            var model = _depositRepository.GetList(page.Value);
            return View(model);
        }

        public IActionResult Create()
        {
            if (_designerRepository.IsValid(HttpContext, _memoryCache) == false)
            {
                return RedirectToAction("VerifyCode", "Designer", new { url = "/deposit" });
            }

            ViewData["Designer"] = new SelectList(_designerRepository.GetList(true), "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Deposit deposit, string CreatedAt)
        {
            if (CreatedAt != null)
            {
                deposit.CreatedAt = PersianDateTime.Parse(CreatedAt).ToDateTime();
            }
            else
            {
                deposit.CreatedAt = DateTime.Today;
            }

            _depositRepository.Add(deposit);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            if (_designerRepository.IsValid(HttpContext, _memoryCache) == false)
            {
                return RedirectToAction("VerifyCode", "Designer", new { url = "/deposit" });
            }

            ViewData["Designer"] = new SelectList(_designerRepository.GetList(true), "Id", "Name");
            var model = _depositRepository.GetById(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(Deposit deposit, string CreatedAt)
        {
            if (CreatedAt != null)
            {
                deposit.CreatedAt = PersianDateTime.Parse(CreatedAt).ToDateTime();
            }
            else
            {
                deposit.CreatedAt = DateTime.Today;
            }

            _depositRepository.Update(deposit);
            return RedirectToAction(nameof(Index));
        }
    }
}