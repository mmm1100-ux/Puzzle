using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Puzzle.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModels;

namespace Puzzle.Controllers
{
    [Route("[controller]/")]
    [Authorize(Roles = "Admin")]
    public class LookupController : Controller
    {
        #region Fields

        private readonly ILookupService _lookupService;

        #endregion

        #region Ctor

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        #endregion

        [HttpGet("[action]")]
        public async Task<List<CustomerSelectListViewModel>> GetAllCustomers()
        {
            var result = await _lookupService.GetAllCustomers();

            return result;
        }

        [HttpGet("[action]")]
        public async Task<List<DesignerSelectListViewModel>> GetAllDesigners()
        {
            var result = await _lookupService.GetAllDesigners();

            return result;
        }

    }
}
