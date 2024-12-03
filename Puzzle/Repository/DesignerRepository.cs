using Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Puzzle;
using Puzzle.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using ViewModels;

namespace Repository
{
    public class DesignerRepository
    {
        public List<UserViewModel> GetList(bool isActive = false, List<string> ids = null)
        {
            using var db = new PuzzleDbContext();

            if (ids == null)
            {
                ids = new List<string>();
            }

            var roleId = db.Roles.First(c => c.Name == "Designer").Id;

            var user = db.Users
                .Where(c => isActive == false || c.IsActive == true || ids.Contains(c.Id))
                .OrderBy(c => c.CreatedAt)
                .ToList();

            var userRole = db.UserRoles.ToList();

            var result = new List<UserViewModel>();

            foreach (var item in user)
            {
                if (userRole.Where(c => c.RoleId == roleId && c.UserId == item.Id).Any())
                {
                    var designer = new UserViewModel
                    {
                        Name = item.Name,
                        PhoneNumber = item.PhoneNumber,
                        Id = item.Id,
                        Birthday = item.Birthday?.ToShamsi(),
                        IsActive = item.IsActive,
                        CreatedAt = item.CreatedAt,
                        Priority = item.Priority,
                        Percent = db.Wage.Where(c => c.DesignerId == item.Id).OrderByDescending(c => c.CreatedAt).First().Percent
                    };

                    var lstProject = db.Project
                        .Where(x => x.IsDelete == false)
                        .Where(x => x.ProjectId == null || x.Parent.IsDelete == false)
                        .Where(c => c.DesignerId == designer.Id && c.AssignedToDesignerAt.Value.Date == DateTime.Today)
                        .ToList();

                    designer.TodayCount = lstProject.Count();

                    foreach (var project in lstProject)
                    {
                        int max = project.TotalPrice > 0 ? project.TotalPrice : project.Approximate;
                        designer.TodayPrice += max;
                    }

                    result.Add(designer);
                }
            }

            return result;
        }

        public List<UserViewModel> Admin()
        {
            using var db = new PuzzleDbContext();

            var roleId = db.Roles.First(c => c.Name == "Admin").Id;

            var user = db.Users
                .OrderBy(c => c.CreatedAt)
                .ToList();

            var userRole = db.UserRoles.ToList();

            var result = new List<UserViewModel>();
            foreach (var item in user)
            {
                if (userRole.Where(c => c.RoleId == roleId && c.UserId == item.Id).Any())
                {
                    var designer = new UserViewModel
                    {
                        Name = item.Name,
                        PhoneNumber = item.PhoneNumber,
                        Id = item.Id,
                        Birthday = item.Birthday?.ToShamsi(),
                        IsActive = item.IsActive
                    };

                    result.Add(designer);
                }
            }

            return result;
        }

        public int GetPercent(string designerId, DateTime createdAt)
        {
            using var db = new PuzzleDbContext();

            var wage = db.Wage
                .Where(c => c.DesignerId == designerId)
                .Where(c => c.CreatedAt <= createdAt)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefault();

            if (wage == null)
            {
                wage = db.Wage
                    .Where(c => c.DesignerId == designerId)
                    .OrderByDescending(c => c.CreatedAt)
                    .First();
            }

            return wage.Percent;
        }

        public bool IsValid(HttpContext httpContext, IMemoryCache memoryCache)
        {
            if (Setting.IsDevelopment)
            {
                return true;
            }

            var token = httpContext.Request.Cookies["token"];

            if (memoryCache.TryGetValue("token", out string cacheEntry))
            {
                if (token == cacheEntry)
                {
                    memoryCache.Set("token", cacheEntry, new MemoryCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) });

                    return true;
                }
            }

            return false;
        }
    }
}
