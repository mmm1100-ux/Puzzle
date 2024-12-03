using System.Collections.Generic;

namespace ViewModels
{
    public class SearchViewModel
    {
        public List<CustomerViewModel> Customer { get; set; }

        public PageViewModel Page { get; set; }

        public FilterViewModel Filter { get; set; }
    }
}
