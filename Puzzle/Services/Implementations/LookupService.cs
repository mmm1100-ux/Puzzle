using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Puzzle.Data;
using Puzzle.Models;
using Puzzle.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;

namespace Puzzle.Services.Implementations
{
    public class LookupService : ILookupService
    {
        #region Fields

        private readonly PuzzleDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        #endregion

        #region Ctor

        public LookupService(PuzzleDbContext db,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        #endregion

        public async Task<List<CustomerSelectListViewModel>> GetAllCustomers()
        {
            var customers = await _db.Customer.Select(a => new CustomerSelectListViewModel
            {
                Id = a.Id,
                FullName = $"{a.FirstName} {a.LastName}" ?? a.Mobile.ToString()
            }).ToListAsync();

            return customers;
        }

        public async Task<List<DesignerSelectListViewModel>> GetAllDesigners()
        {
            var isExist = await _roleManager.RoleExistsAsync("Designer");

            if (!isExist)
            {
                return new List<DesignerSelectListViewModel>();
            }

            var users = (await _userManager.GetUsersInRoleAsync("Designer")).ToList();

            var model = users.Select(a => new DesignerSelectListViewModel
            {
                Id = a.Id,
                FullName = a.Name ?? a.NormalizedUserName ?? a.Email,
            }).ToList();

            return model;
        }
    }
}
