using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class InquiryListingViewModel
    {
        public IList<InquiryListViewModel> InquiryList { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public string SearchString { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}