using MyCard.BoundedContext.Repositories;
using MyCard.Domain;
using MyCard.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;

namespace MyCard.Web
{
    public class LicenseExpiryAlert
    {
        private ICompanyRepository _companyRepository;
        private IPurchaseOrderRepository _purchaseOrderRepository;
        private IEmailTemplateRepository _emailTemplateRepository;
        private ILicenseTypeRepository _licenseTypeRepository;

        public LicenseExpiryAlert()
        {
            BoundedContext.IMyCardUnitOfWork obj = new BoundedContext.MyCardUnitOfWork();
            _companyRepository = new CompanyRepository(obj);
            _purchaseOrderRepository = new PurchaseOrderRepository(obj);
            _emailTemplateRepository = new EmailTemplateRepository(obj);
            _licenseTypeRepository = new LicenseTypeRepository(obj);

            //if (purchaseOrderRepository == (IPurchaseOrderRepository)null)
            //    throw new ArgumentNullException("purchaseOrderRepository");
            //_purchaseOrderRepository = purchaseOrderRepository;

            //if (emailTemplateRepository == (IEmailTemplateRepository)null)
            //    throw new ArgumentNullException("emailTemplateRepository");
            //_emailTemplateRepository = emailTemplateRepository;

        }

        public void SendAlerts()
        {
            //Send Notifications to customers with yearly subscription
            IList<PurchaseOrder> poList = new List<PurchaseOrder>();
            DateTime dt = DateTime.UtcNow.AddYears(-1);

            poList = _purchaseOrderRepository.GetFilteredElements(x => x.Approved == 1 && x.LicenseId==1 && x.ApprovalDate > dt).ToList();
            
            foreach (PurchaseOrder item in poList)
            {
                DateTime OneYearCompletionDate = item.ApprovalDate.AddYears(1);
                //DateTime OneYearCompletionDate = item.ApprovalDate.AddDays(61);
                DateTime SixtyDaysPrior = OneYearCompletionDate.AddDays(-60);
                DateTime ThirtyDaysPrior = OneYearCompletionDate.AddDays(-30);

                Company company = _companyRepository.GetElementById(item.CompanyId);

                if (SixtyDaysPrior.Date == DateTime.UtcNow.Date)
                {
                    //Email Template
                    string emailBody = string.Empty;
                    string emailSubject = string.Empty;
                    EmailTemplate emailTemplate = null;
                    //emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.Name == "Account Expiry").FirstOrDefault();
                    emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 4).FirstOrDefault();


                    if (emailTemplate != null)
                    {
                        emailBody = emailTemplate.Template;
                        emailSubject = emailTemplate.Subject;
                    }
                    StringBuilder sb = new StringBuilder(emailBody);
                    sb.Replace("[CompanyName]", company.Name);
                    sb.Replace("[NotifyDays]", "60");
                    sb.Replace("[ExpiryDate]", OneYearCompletionDate.ToString("d-MMM-yyyy"));
                    sb.Replace("[PurchaseOrderNo]", item.OrderNumber);
                    sb.Replace("[NumberOfLicenses]", item.LicensesOrdered.ToString());
                    sb.Replace("[Amount]", item.OrderTotal.ToString());

                    LicenseType licenseType = _licenseTypeRepository.GetElementById(item.LicenseId);
                    if(licenseType!=null)
                    {
                        sb.Replace("[LicenseType]", licenseType.LicenseTypeName);
                    }
                    //Email Template

                    HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
                    {
                        MailHelper mail = new MailHelper();
                        string subject = emailSubject;// MyCard.Web.Resources.CaptionsAll.LicenseActivationSubject;
                        string body = sb.ToString();// String.Format(@Resources.CaptionsAll.LicenseActivationEmailMessage, company.Name);
                        await mail.SendAsync(company.Email, subject, body, String.Empty);

                    });
                }

                if (ThirtyDaysPrior.Date == DateTime.UtcNow.Date)
                {
                    //Email Template
                    string emailBody = string.Empty;
                    string emailSubject = string.Empty;
                    EmailTemplate emailTemplate = null;
                    //emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.Name == "Account Expiry").FirstOrDefault();
                    emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 4).FirstOrDefault();


                    if (emailTemplate != null)
                    {
                        emailBody = emailTemplate.Template;
                        emailSubject = emailTemplate.Subject;
                    }
                    StringBuilder sb = new StringBuilder(emailBody);
                    sb.Replace("[CompanyName]", company.Name);
                    sb.Replace("[NotifyDays]", "60");
                    sb.Replace("[ExpiryDate]", OneYearCompletionDate.ToString("d-MMM-yyyy"));
                    sb.Replace("[PurchaseOrderNo]", item.OrderNumber);
                    sb.Replace("[NumberOfLicenses]", item.LicensesOrdered.ToString());
                    sb.Replace("[Amount]", item.OrderTotal.ToString());

                    LicenseType licenseType = _licenseTypeRepository.GetElementById(item.LicenseId);
                    if (licenseType != null)
                    {
                        sb.Replace("[LicenseType]", licenseType.LicenseTypeName);
                    }
                    //Email Template

                    HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
                    {
                        MailHelper mail = new MailHelper();
                        string subject = emailSubject;// MyCard.Web.Resources.CaptionsAll.LicenseActivationSubject;
                        string body = sb.ToString();// String.Format(@Resources.CaptionsAll.LicenseActivationEmailMessage, company.Name);
                        await mail.SendAsync(company.Email, subject, body, String.Empty);

                    });
                }
            }
        }
    }
}
