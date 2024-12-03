using System.Collections.Generic;
using static Enums.Enum;

namespace ViewModels
{
    public class CustomerListViewModel
    {
        public List<CustomerViewModel> Customer { get; set; }
        public PageViewModel Page { get; set; }
        public FilterViewModel Filter { get; set; }
        public OrderBy OrderBy { get; set; }
        public bool Asc { get; set; }
    }
}
