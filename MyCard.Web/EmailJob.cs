using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using System.Net.Mail;
using MyCard.Domain;
using MyCard.Helper;
using System.Web.Hosting;
using System.Text;

namespace MyCard.Web
{
    public class EmailJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {

            LicenseExpiryAlert obj = new LicenseExpiryAlert();
            obj.SendAlerts();
            
        }
    }
}