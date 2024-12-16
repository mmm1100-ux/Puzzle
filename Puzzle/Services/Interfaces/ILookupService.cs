using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModels;

namespace Puzzle.Services.Interfaces
{
    public interface ILookupService
    {
        Task<List<CustomerSelectListViewModel>> GetAllCustomers();
        Task<List<DesignerSelectListViewModel>> GetAllDesigners();
    }
}
