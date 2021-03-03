using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class CompanyDashboardViewModel
    {

        public string EmailID { get; set; }
        public string Name { get; set; }
        public string BusinessTitle { get; set; }
        public string CustomerName { get; set; }
        public string DomainName { get; set; }
        public int TotalPurchase { get; set; }
        public string LastPurchaseDate { get; set; }
        public string MemberSince { get; set; }
        public int TotalCompaniesCreated { get; set; }
        public int TotalCardsShared { get; set; }

    }
}