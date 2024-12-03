using Extensions;
using Helper;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Puzzle;
using Puzzle.Helper;
using Puzzle.Models;
using Puzzle.ViewModels;
using Repository;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;

namespace Controllers
{
    [TimeFilter]
    [Authorize(Roles = "Manager")]
    public class DesignerController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DesignerRepository _designerRepository;
        private readonly WageRepository _wageRepository;
        private readonly DepositRepository _depositRepository;
        private readonly ProjectRepository _projectRepository;
        private readonly PaymentRepository _paymentRepository;
        private readonly IMemoryCache _memoryCache;

        public DesignerController(UserManager<User> userManager, IMemoryCache memoryCache)
        {
            _userManager = userManager;
            _designerRepository = new DesignerRepository();
            _wageRepository = new WageRepository();
            _depositRepository = new DepositRepository();
            _projectRepository = new ProjectRepository();
            _paymentRepository = new PaymentRepository();
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            if (_designerRepository.IsValid(HttpContext, _memoryCache) == false)
            {
                return RedirectToAction("VerifyCode", "Designer", new { url = "/designer" });
            }

            var model = _designerRepository.GetList();
            return View(model);
        }

        [HttpPost]
        public IActionResult Authorize()
        {
            if (_designerRepository.IsValid(HttpContext, _memoryCache) == false)
            {
                return Ok(false);
            }

            return Ok(true);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (_designerRepository.IsValid(HttpContext, _memoryCache) == false)
            {
                return RedirectToAction("VerifyCode", "Designer", new { url = "/designer" });
            }

            return View();
        }

        [HttpPost]
        public async Task<int> Create(UserViewModel user, string birthday)
        {
            string phoneNumber = "0" + user.PhoneNumber.Substring(user.PhoneNumber.Length - 10, 10);

            if (await _userManager.FindByNameAsync(phoneNumber) != null)
            {
                return 2;
            }

            if (birthday != null)
            {
                user.Birthday = PersianDateTime.Parse(birthday);
            }

            var applicationUser = new User
            {
                UserName = phoneNumber,
                PhoneNumber = phoneNumber,
                IsActive = user.IsActive,
                Description = user.Description,
                Name = user.Name,
                Birthday = user.Birthday?.ToDateTime(),
                //CardNumber = user.CardNumber,
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(applicationUser, user.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(applicationUser, "Designer");
                _wageRepository.Add(applicationUser.Id, user.Percent);
                return 1;
            }

            return 0;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id, string from, string to, bool layout = true)
        {
            if (_designerRepository.IsValid(HttpContext, _memoryCache) == false)
            {
                return RedirectToAction("VerifyCode", "Designer", new { url = "/designer" });
            }

            var designer = await _userManager.FindByIdAsync(id);

            PersianDateTime f;
            PersianDateTime t;

            if (from != null)
            {
                f = PersianDateTime.Parse(from);
            }
            else
            {
                f = new PersianDateTime(_projectRepository.FirstByDesignerId(id));
            }

            if (to != null)
            {
                t = PersianDateTime.Parse(to);
            }
            else
            {
                t = new PersianDateTime(_projectRepository.LastByDesignerId(id));
            }

            var model = new UserViewModel
            {
                Birthday = designer.Birthday?.ToShamsi(),
                Description = designer.Description,
                PhoneNumber = designer.PhoneNumber,
                IsActive = designer.IsActive,
                Name = designer.Name,
                Percent = _wageRepository.GetByDesignerId(id).Percent,
                Image = designer.Image,
                Deposit = _depositRepository.GetByDesignerId(id),
                Project = _projectRepository.GetByDesignerId(id, f, t, 1),
                Payment = _paymentRepository.GetByDesignerId(id, f, t, 1),
                PaymentTotalRow = _paymentRepository.CountByDesignerId(id, f, t),
                Priority = designer.Priority,
                FromDate = f,
                ToDate = t,
                Wage = _projectRepository.WageByDesignerId(id, f, t),
                TotalWage = _projectRepository.WageByDesignerId(id)
            };

            ViewData["Layout"] = layout;
            ViewData["ShowDesigner"] = false;
            ViewData["Designer"] = _designerRepository.GetList(true);
            ViewData["ShowName"] = true;

            return View(model);
        }

        [HttpPost]
        public async Task<int> Edit(UserViewModel user, string birthday)
        {
            string phoneNumber = "0" + user.PhoneNumber.Substring(user.PhoneNumber.Length - 10, 10);

            var designer = await _userManager.FindByNameAsync(phoneNumber);

            if (designer != null && designer.Id != user.Id)
            {
                return 2;
            }

            designer = await _userManager.FindByIdAsync(user.Id);

            if (birthday != null)
            {
                user.Birthday = PersianDateTime.Parse(birthday);
            }

            designer.UserName = phoneNumber;
            designer.PhoneNumber = phoneNumber;
            designer.IsActive = user.IsActive;
            designer.Description = user.Description;
            designer.Name = user.Name;
            designer.Birthday = user.Birthday?.ToDateTime();
            //designer.CardNumber = user.CardNumber;
            designer.Priority = user.Priority;

            var result = await _userManager.UpdateAsync(designer);

            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                var remove = await _userManager.RemovePasswordAsync(designer);
                var add = await _userManager.AddPasswordAsync(designer, user.Password);

                if (add.Succeeded)
                {
                    return 1;
                }

                return 0;
            }

            if (result.Succeeded)
            {
                _wageRepository.Add(designer.Id, user.Percent);
                return 1;
            }

            return 0;
        }

        [HttpPost]
        public IActionResult Project(string id, string from, string to, int page, int? status)
        {
            PersianDateTime? f = null;
            PersianDateTime? t = null;

            if (from != null)
            {
                f = PersianDateTime.Parse(from);
            }

            if (to != null)
            {
                t = PersianDateTime.Parse(to);
            }

            ViewData["Designer"] = _designerRepository.GetList(true);
            ViewData["ShowName"] = true;

            return PartialView("_ProjectPartial", _projectRepository.GetByDesignerId(id, f, t, page, status));
        }

        [HttpPost]
        public IActionResult Payment(string id, string from, string to, int page)
        {
            var f = PersianDateTime.Parse(from);
            var t = PersianDateTime.Parse(to);

            return PartialView("_PaymentPartial", _paymentRepository.GetByDesignerId(id, f, t, page));
        }

        [HttpPost]
        public IActionResult SendCode()
        {
            _memoryCache.TryGetValue("code", out List<SendCodeViewModel> cacheEntry);

            if (cacheEntry == null)
            {
                cacheEntry = new List<SendCodeViewModel>();
            }

            int code = new Random().Next(1000, 10000);

            if (Setting.IsDevelopment)
            {
                new SmsService().Send("09124534189", Enums.Enum.SmsTemplate.PuzzleAutoVerify, code.ToString(), "LoginToAdmin", null);
            }
            else
            {
                new SmsService().Send("09126035433", Enums.Enum.SmsTemplate.PuzzleAutoVerify, code.ToString(), "LoginToAdmin", null);
            }
          
            cacheEntry.Add(new SendCodeViewModel { Code = code, CreatedAt = DateTime.Now });

            _memoryCache.Set("code", cacheEntry, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) });

            return Ok();
        }

        [HttpGet]
        public IActionResult VerifyCode(string url)
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyCode(int code, string url)
        {
            if (_memoryCache.TryGetValue("code", out List<SendCodeViewModel> cacheEntry))
            {
                if (cacheEntry.Where(c => c.Code == code).Where(c => c.CreatedAt.AddMinutes(2) >= DateTime.Now).Any())
                {
                    var token = Guid.NewGuid().ToString();

                    HttpContext.Response.Cookies.Append("token", token, new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(60)
                    });

                    _memoryCache.Set("token", token, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) });

                    return LocalRedirect(url);
                }
            }

            return View();
        }
    }
}