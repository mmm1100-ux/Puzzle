using Extensions;
using MD.PersianDateTime.Standard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using Puzzle.Helper;
using Puzzle.Models;
using Repository;
using System;
using System.Threading.Tasks;
using ViewModels;

namespace Puzzle.Controllers
{
    [TimeFilter]
    [Authorize(Roles = "Manager")]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly DesignerRepository _designerRepository;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
            _designerRepository = new DesignerRepository();
        }

        public IActionResult Index()
        {
            var model = _designerRepository.Admin();
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
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
                Name = user.Name,
                Birthday = user.Birthday?.ToDateTime(),
                CreatedAt = DateTime.Now
            };

            var result = await _userManager.CreateAsync(applicationUser, user.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(applicationUser, "Admin");
                return 1;
            }

            return 0;
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var designer = await _userManager.FindByIdAsync(id);

            var model = new UserViewModel
            {
                Birthday = designer.Birthday?.ToShamsi(),
                PhoneNumber = designer.PhoneNumber,
                IsActive = designer.IsActive,
                Name = designer.Name,
                FromTime = designer.FromTime,
                ToTime = designer.ToTime
            };

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
            designer.Name = user.Name;
            designer.Birthday = user.Birthday?.ToDateTime();
            designer.FromTime = user.FromTime;
            designer.ToTime = user.ToTime;

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
                return 1;
            }

            return 0;
        }
    }
}
