using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyCard.Web.Models
{
    public class AdminDashboardViewModel
    {
        public int TotalComapnies { get; set; }
        public int TotalCardsShared { get; set; }
        public int TotalPendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}