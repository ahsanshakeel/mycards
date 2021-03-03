using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class EmployeeListingViewModel
    {
        public IList<EmployeeListViewModel> EmployeesList { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
        public string SearchString { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int CompanyID { get; set; }
    }
}