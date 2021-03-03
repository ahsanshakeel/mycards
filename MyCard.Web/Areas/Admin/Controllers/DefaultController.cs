using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCard.Web.Models;
using System.Net.Mail;
using MyCard.Domain;
using System.Threading.Tasks;
using System.Web.Security;
using System.Linq.Expressions;
using System.Threading;
using PagedList;
using System.Web.Hosting;
using MyCard.Helper;
using MyCard.Web.Attributes;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;

namespace MyCard.Web.Areas.Admin.Controllers
{

    [ErrorHandlerAttribute]
    [Authorize(Roles= "super_admin")]
    public class DefaultController : Controller
    {
        private ICompanyRepository _companyRepository;
        private IUserRepository _userRepository;
        private IPurchaseOrderRepository _purchaseOrderRepository;
        private IContactShareActivityRepository _contactShareActivityRepository;
        private ILicenseTypeRepository _licenseTypeRepository;
        private IEmailTemplateRepository _emailTemplateRepository;
        private IInquiryRepository _inquiryRepository;
        private IMyCardFileRepository _myCardFileRepository;
        private ILicenseDescriptionRepository _licenseDescriptionRepository;
        private ICityRepository _cityRepository;
        private IIndustryRepository _industryRepository;
        public DefaultController(ICompanyRepository companyRepository, 
            IUserRepository userRepository, 
            IPurchaseOrderRepository purchaseOrderRepository, 
            IContactShareActivityRepository contactShareActivityRepository, 
            ILicenseTypeRepository licenseTypeRepository,
            IEmailTemplateRepository emailTemplateRepository,
            IInquiryRepository inquiryRepository,
            IMyCardFileRepository myCardFileRepository, ICityRepository cityRepository, IIndustryRepository industryRepository,
            ILicenseDescriptionRepository licenseDescriptionRepository)
        {
            if (companyRepository == (ICompanyRepository)null)
                throw new ArgumentNullException("companyRepository");
            _companyRepository = companyRepository;

            if (userRepository == (IUserRepository)null)
                throw new ArgumentNullException("userRepository");
            _userRepository = userRepository;

            if (purchaseOrderRepository == (IPurchaseOrderRepository)null)
                throw new ArgumentNullException("purchaseOrderRepository");
            _purchaseOrderRepository = purchaseOrderRepository;

            if (contactShareActivityRepository == (IContactShareActivityRepository)null)
                throw new ArgumentNullException("contactShareActivityRepository");
            _contactShareActivityRepository = contactShareActivityRepository;


            if (licenseTypeRepository == (ILicenseTypeRepository)null)
                throw new ArgumentNullException("licenseTypeRepository");
            _licenseTypeRepository = licenseTypeRepository;

            if (emailTemplateRepository == (IEmailTemplateRepository)null)
                throw new ArgumentNullException("EmailTemplateRepository");
            _emailTemplateRepository = emailTemplateRepository;

            if (inquiryRepository == (IInquiryRepository)null)
                throw new ArgumentNullException("inquiryRepository");
            _inquiryRepository = inquiryRepository;

            if (myCardFileRepository == (IMyCardFileRepository)null)
                throw new ArgumentNullException("myCardFileRepository");
            _myCardFileRepository = myCardFileRepository;

            if (licenseDescriptionRepository == (ILicenseDescriptionRepository)null)
                throw new ArgumentNullException("licenseDescriptionRepository");
            _licenseDescriptionRepository = licenseDescriptionRepository;

            if (cityRepository == (ICityRepository)null)
                throw new ArgumentNullException("cityRepository");
            _cityRepository = cityRepository;

            if (industryRepository == (IIndustryRepository)null)
                throw new ArgumentNullException("industryRepository");
            _industryRepository = industryRepository;
        }

        // GET: Admin/Dashboard
        public ActionResult Index()
        {
            //if(Request.IsAuthenticated)
            //{

                AdminDashboardViewModel model = new AdminDashboardViewModel();
                
                model.TotalComapnies = _companyRepository.GetAllElements().Count();
                model.TotalCardsShared = _contactShareActivityRepository.GetAllElements().Count();
                model.TotalPendingOrders = _purchaseOrderRepository.GetFilteredElements(o => o.Approved == 0 && o.FileName!= null).Count();
                model.TotalRevenue = _purchaseOrderRepository.GetFilteredElements(o => o.Approved == 1).Sum(t => t.OrderTotal);
                

                return View(model);
            //}
            //else
            //{
            //    return RedirectToAction("Login", "Default");
            //}
            
        }
        
        //Inquiry Export
        public ActionResult InquiryListingExport(string SearchString, DateTime? DateTo, DateTime? DateFrom)
        {

            if (DateTo != null)
            {
                DateTo = DateTo.Value.AddHours(23).AddMinutes(59);
            }
            ///////////Filtering Records, Setting Filter
            Expression<Func<Inquiry, bool>> filter = null;
            if (!String.IsNullOrEmpty(SearchString) && (DateFrom == null || DateTo == null))
            {
                filter = x => x.BusinessName.Contains(SearchString) || x.Email.Contains(SearchString) || x.CustomerName.Contains(SearchString);
            }

            if (DateFrom != null && DateTo != null && String.IsNullOrEmpty(SearchString))
            {
                DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CreationDate >= DateFrom.Value && x.CreationDate <= DateTo.Value;
            }

            if (DateFrom != null && DateTo != null && !String.IsNullOrEmpty(SearchString))
            {
                DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CreationDate >= DateFrom.Value && x.CreationDate <= DateTo.Value && (x.BusinessName.Contains(SearchString) || x.Email.Contains(SearchString)
                || x.CustomerName.Contains(SearchString));
            }

            List<InquiryListViewModel> modelList = new List<InquiryListViewModel>();
            InquiryListViewModel model = null;

            IList<Inquiry> inquiries = null;

            if (filter == null)
            {
                inquiries = _inquiryRepository.GetAllElements().ToList();
            }
            else
            {
                inquiries = _inquiryRepository.GetFilteredElements(filter).ToList();
            }

            foreach (Inquiry item in inquiries)
            {
                model = new InquiryListViewModel();
                model.BusinessName = item.BusinessName;
                model.CustomerName = item.CustomerName;
                model.Email = item.Email;
                model.InquiryDate = item.CreationDate.ToString("d-MMM-yyyy");
                //model.TotalLicenses = _userRepository.GetFilteredElements(o => o.CompanyId == item.Id).Count();
                modelList.Add(model);
            }

            string filename = "InqueriesExportFile" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            return View(modelList);
        }
        //InquiryExport End

        //New company Listing Export
        public ActionResult CompanyListingExport(string SearchString, DateTime? DateTo, DateTime? DateFrom)
        {

            if (DateTo != null)
            {
                DateTo = DateTo.Value.AddHours(23).AddMinutes(59);
            }
            
            Expression<Func<Company, bool>> filter = null;
            if (!String.IsNullOrEmpty(SearchString) && (DateFrom == null || DateTo == null))
            {
                filter = x => x.Name.Contains(SearchString) || x.Email.Contains(SearchString) || x.Domain.Contains(SearchString);
            }

            if (DateFrom != null && DateTo != null && String.IsNullOrEmpty(SearchString))
            {
                DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CreationDate >= DateFrom.Value && x.CreationDate <= DateTo.Value;
            }

            if (DateFrom != null && DateTo != null && !String.IsNullOrEmpty(SearchString))
            {
                DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CreationDate >= DateFrom.Value && x.CreationDate <= DateTo.Value && (x.Name.Contains(SearchString) || x.Email.Contains(SearchString)
                || x.Domain.Contains(SearchString));
            }

            List<CompanyListViewModel> modelList = new List<CompanyListViewModel>();
            CompanyListViewModel model = null;

            IList<Company> companies = null;

            if (filter == null)
            {
                companies = _companyRepository.GetAllElements().ToList();
            }
            else
            {
                companies = _companyRepository.GetFilteredElements(filter).ToList();
            }
            
            foreach (Company item in companies)
            {
                model = new CompanyListViewModel();
                model.CompanyID = item.Id;
                model.CompanyName = item.Name;
                model.DomainName = item.Domain;
                model.EmailID = item.Email;
                model.RegistrationDate = item.CreationDate.ToString("d-MMM-yyyy");
                //model.TotalLicenses = _userRepository.GetFilteredElements(o => o.CompanyId == item.Id).Count();
                var poTotalAmount = _purchaseOrderRepository.GetFilteredElements(o => o.CompanyId == item.Id && o.Approved == 1).Sum(o => o.LicensesOrdered);
                model.TotalLicenses = poTotalAmount;
                modelList.Add(model);
            }

            
            string filename = "CompaniesExportFile" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            
            return View(modelList);
        }
        
        //PurchaseOrder Export
        public ActionResult PurchaseOrdersExport(string SearchString, DateTime? DateTo, DateTime? DateFrom, int POStatus)
        {
            List<int> companyIdList = new List<int>();
            int totalRows = 0;
            IList<Company> companyList = new List<Company>();

            ///////////Filtering Records, Setting Filter
            Expression<Func<PurchaseOrder, bool>> filter = null;
            if (!String.IsNullOrEmpty(SearchString) && (DateFrom == null || DateTo == null))
            {
                companyList = _companyRepository.GetFilteredElements(o => o.Name.Contains(SearchString) || o.Email.Contains(SearchString)).ToList();
                if (companyList != null)
                {
                    foreach (Company item in companyList)
                    {
                        companyIdList.Add(item.Id);
                    }

                }
                filter = x => companyIdList.Contains(x.CompanyId) && (x.Approved == POStatus || POStatus == 99);
            }
            if (DateTo != null)
            {
                DateTo = DateTo.Value.AddHours(23).AddMinutes(59);
            }

            if (DateFrom != null && DateTo != null && String.IsNullOrEmpty(SearchString))
            {
                filter = x => x.CreationDate >= DateFrom.Value && x.CreationDate <= DateTo.Value && (x.Approved == POStatus || POStatus == 99);
            }

            if (DateFrom != null && DateTo != null && !String.IsNullOrEmpty(SearchString))
            {
                companyList = _companyRepository.GetFilteredElements(o => o.Name.Contains(SearchString) || o.Email.Contains(SearchString)).ToList();
                if (companyList != null)
                {
                    foreach (Company item in companyList)
                    {
                        companyIdList.Add(item.Id);
                    }

                }
                filter = x => (companyIdList.Contains(x.CompanyId)) && (x.Approved == POStatus || POStatus == 99) && (x.CreationDate >= DateFrom.Value && x.CreationDate <= DateTo.Value);
            }
            /////////Filtering Records
            IList<PurchaseOrder> poList = new List<PurchaseOrder>();
            if (filter == null)
            {
                poList = _purchaseOrderRepository.GetFilteredElements(x => x.Approved == POStatus || POStatus == 99).ToList();
            }
            else
            {
                poList = _purchaseOrderRepository.GetFilteredElements(filter).ToList();
            }

            if (poList != null)
            {
                totalRows = poList.Count();
            }

            List<POListViewModel> modelList = new List<POListViewModel>();
            POListViewModel model = null;

            IList<PurchaseOrder> purchaseOrders = null;

           
            if (filter == null)
            {
                purchaseOrders = _purchaseOrderRepository.GetFilteredElements(x => x.Approved == POStatus || POStatus == 99).ToList();
            }
            else
            {
                purchaseOrders = _purchaseOrderRepository.GetFilteredElements(filter).ToList();
            }

            //purchaseOrder = _purchaseOrderRepository.GetAllElements().ToList();

            foreach (PurchaseOrder item in purchaseOrders)
            {
                model = new POListViewModel();
                Company comp = null;
                comp = _companyRepository.GetFilteredElements(o => o.Id == item.CompanyId).FirstOrDefault();
                if (comp != null)
                {
                    model.CompanyName = comp.Name;
                    model.Email = comp.Email;
                }
                else
                {
                    model.CompanyName = string.Empty;
                    model.Email = string.Empty;
                }
                model.ID = item.Id;
                model.OrderNumber = item.OrderNumber;
                model.PODate = item.CreationDate.ToString("d-MMM-yyyy");
                model.Quantity = item.LicensesOrdered;
                model.OrderTotal = item.OrderTotal;
                model.OrderFile = item.OrderFile;
                model.Approved = item.Approved;
                model.FileName = item.FileName;
                model.ContentType = item.ContentType;
                model.SendInvoice = item.SendInvoice ? "Yes" : "No";
                model.RejectReason = item.RejectReason;

                LicenseType licenseType = _licenseTypeRepository.GetElementById(item.LicenseId);

                if (licenseType != null)
                {
                    model.LicenseTypeName = licenseType.LicenseTypeName;
                }
                else
                {
                    model.LicenseTypeName = "";
                }

                if (item.Approved == 1)
                {
                    model.ApprovalDate = item.ApprovalDate.ToString("d-MMM-yyyy");
                    //model.ExpiryDate = item.ApprovalDate.AddYears(1).ToString("d-MMM-yyyy");
                    model.ExpiryDate = item.ApprovalDate.AddDays(61).ToString("d-MMM-yyyy");
                }
                else
                {
                    model.ApprovalDate = "";
                    model.ExpiryDate = "";
                }
                modelList.Add(model);
            }
            //return View(modelList.ToPagedList(pageno, 4));

            string filename = "PurchaseOrderExportFile" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            //Response.WriteFile(filename);
            //Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            return View(modelList);
        }

        
        //CompaniesEmployees Export
        public ActionResult CompaniesEmployeesExport(string SearchString, DateTime? DateTo, DateTime? DateFrom, int CompanyID)
        {

            
            Expression<Func<User, bool>> filter = null;
            if (DateTo != null)
            {
                DateTo = DateTo.Value.AddHours(23).AddMinutes(59);
            }

            if (!String.IsNullOrEmpty(SearchString) && (DateFrom == null || DateTo == null))
            {
                filter = x => x.CompanyId == CompanyID && x.Active == 1 && (x.Name.Contains(SearchString) || x.Email.Contains(SearchString));
            }

            if (DateFrom != null && DateTo != null && String.IsNullOrEmpty(SearchString))
            {
                DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CompanyId == CompanyID && x.Active == 1 && x.CreationDate >= DateFrom.Value && x.CreationDate <= DateTo.Value;
            }

            if (DateFrom != null && DateTo != null && !String.IsNullOrEmpty(SearchString))
            {
                DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CompanyId == CompanyID && x.Active == 1 && (x.Name.Contains(SearchString) || x.Email.Contains(SearchString))
                && (x.CreationDate >= DateFrom.Value && x.CreationDate <= DateTo.Value);
            }
            /////////Filtering Records
            IList<User> userList = new List<User>();
            if (filter == null)
            {
                userList = _userRepository.GetFilteredElements(x => x.CompanyId == CompanyID && x.Active == 1, x => x.UserRoles).ToList();
            }
            else
            {
                userList = _userRepository.GetFilteredElements(filter, x => x.UserRoles).ToList();
            }

            IList<int> userIds = new List<int>();
            if (userList != null)
            {
                //totalRows = userList.Count();
                foreach (User item in userList)
                {
                    foreach (UserRole ur in item.UserRoles)
                    {
                        if (ur.RoleId == 3)
                        {
                            userIds.Add(ur.UserId);
                        }
                    }
                }
            }

            List<EmployeeListViewModel> modelList = new List<EmployeeListViewModel>();
            EmployeeListViewModel model = null;

            IList<User> users = null;

            //employeeListingModel.CompanyID = companyid;
            if (filter == null)
            {
                users = _userRepository.GetFilteredElements(x => x.CompanyId == CompanyID && userIds.Contains(x.Id)).ToList();
            }
            else
            {
                users = _userRepository.GetFilteredElements(x => x.CompanyId == CompanyID && userIds.Contains(x.Id)).ToList();
            }

            foreach (var item in users)
            {
                model = new EmployeeListViewModel();
                model.ID = item.Id;
                model.EmployeeName = item.Name;
                model.EmailID = item.Email;
                model.JoiningDate = item.CreationDate.ToString("d-MMM-yyyy");
                model.Status = item.Active == 1 ? "Activated" : "Deactivated";
                modelList.Add(model);
            }
            string filename = "EmployeesExporFile" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            Response.AddHeader("content-disposition", "attachment;filename=" + filename);
            Response.AddHeader("Content-Type", "application/vnd.ms-excel");
            //Response.WriteFile(filename);
            //Response.ContentEncoding = System.Text.Encoding.GetEncoding("utf-8");
            //return View(modelList);
            return View(modelList);
        }
        
        
        //CompaniesListing
        //GET: Admin/Default/CompaniesListing
        public ActionResult CompaniesListing()
        {
            CompanyListingViewModel listingModel = new CompanyListingViewModel();
            CompanyListViewModel model;
            IList<CompanyListViewModel> modelList = new List<CompanyListViewModel>();
            int pageSize = 10;
            IList<Company> companyList = new List<Company>();
            int totalRows = _companyRepository.GetAllElements().Count();

            double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(pageSize));
            listingModel.PageCount = (int)Math.Ceiling(pageCount);
            if (listingModel.CurrentPageIndex == 0)
            {
                listingModel.CurrentPageIndex = 0;
            }
            listingModel.PageSize = pageSize;

            IList<Company> companies = null;
            companies = _companyRepository.GetPagedElements(listingModel.CurrentPageIndex, listingModel.PageSize, o => o.Id, false).ToList();

            foreach (Company item in companies)
            {
                model = new CompanyListViewModel();
                model.CompanyID = item.Id;
                model.CompanyName = item.Name;
                model.DomainName = item.Domain;
                model.EmailID = item.Email;
                model.RegistrationDate = item.CreationDate.ToString("d-MMM-yyyy");
                //model.TotalLicenses = _userRepository.GetFilteredElements(o => o.CompanyId == item.Id).Count();
                var poTotalAmount = _purchaseOrderRepository.GetFilteredElements(o => o.CompanyId == item.Id && o.Approved == 1).Sum(o => o.LicensesOrdered);
                model.TotalLicenses = poTotalAmount;
                model.City = item.City;
                model.Country = item.Country;
                model.Indutry = item.Industry;
                model.CustomerType = item.CustomerType;
                model.Approved = item.Approved;
                model.AccountType = item.AccountType;
                modelList.Add(model);
            }

            listingModel.Companies = modelList;
            return View(listingModel);
        }

        [HttpPost]
        public ActionResult CompaniesListing(CompanyListingViewModel companyListingModel)
        {
            int totalRows = 0;
            if (companyListingModel.DateTo != null)
            {
                companyListingModel.DateTo = companyListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
            }
            ///////////Filtering Records, Setting Filter
            Expression<Func<Company, bool>> filter = null;
            if (!String.IsNullOrEmpty(companyListingModel.SearchString) && (companyListingModel.DateFrom == null || companyListingModel.DateTo == null))
            {
                filter = x => x.Name.Contains(companyListingModel.SearchString) || x.Email.Contains(companyListingModel.SearchString) || x.Domain.Contains(companyListingModel.SearchString);
            }

            if (companyListingModel.DateFrom != null && companyListingModel.DateTo != null && String.IsNullOrEmpty(companyListingModel.SearchString))
            {
                companyListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CreationDate >= companyListingModel.DateFrom.Value && x.CreationDate <= companyListingModel.DateTo.Value;
            }

            if (companyListingModel.DateFrom != null && companyListingModel.DateTo != null && !String.IsNullOrEmpty(companyListingModel.SearchString))
            {
                companyListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CreationDate >= companyListingModel.DateFrom.Value && x.CreationDate <= companyListingModel.DateTo.Value && (x.Name.Contains(companyListingModel.SearchString) || x.Email.Contains(companyListingModel.SearchString)
                || x.Domain.Contains(companyListingModel.SearchString));
            }
            /////////Filtering Records

            IList<Company> companyList = new List<Company>();

            if (filter == null)
            {
                companyList = _companyRepository.GetAllElements().ToList();
            }
            else
            {
                companyList = _companyRepository.GetFilteredElements(filter).ToList();
            }

            if (companyList != null)
            {
                totalRows = companyList.Count();
            }

            //CompanyListingViewModel companyListingModel = new CompanyListingViewModel();
            List<CompanyListViewModel> modelList = new List<CompanyListViewModel>();
            CompanyListViewModel model = null;

            IList<Company> companies = null;

            double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(companyListingModel.PageSize));

            companyListingModel.PageCount = (int)Math.Ceiling(pageCount);

            if (filter == null)
            {
                companies = _companyRepository.GetPagedElements(companyListingModel.CurrentPageIndex, companyListingModel.PageSize, o => o.Id, false).ToList();
            }
            else
            {
                companies = _companyRepository.GetPagedElements(companyListingModel.CurrentPageIndex, companyListingModel.PageSize, o => o.Id, filter, false).ToList();
            }


            foreach (Company item in companies)
            {
                model = new CompanyListViewModel();
                model.CompanyID = item.Id;
                model.CompanyName = item.Name;
                model.DomainName = item.Domain;
                model.EmailID = item.Email;
                model.RegistrationDate = item.CreationDate.ToString("d-MMM-yyyy");
                //model.TotalLicenses = _userRepository.GetFilteredElements(o => o.CompanyId == item.Id).Count();
                var poTotalAmount = _purchaseOrderRepository.GetFilteredElements(o => o.CompanyId == item.Id && o.Approved == 1).Sum(o => o.LicensesOrdered);
                model.TotalLicenses = poTotalAmount;
                model.City = item.City;
                model.Country = item.Country;
                model.Indutry = item.Industry;
                model.CustomerType = item.CustomerType;
                model.Approved = item.Approved;
                model.AccountType = item.AccountType;
                modelList.Add(model);
            }

            companyListingModel.Companies = modelList;

            //return companyListingModel;
            return PartialView("_PartialCompanies", companyListingModel);
        }

        //CompaniesListing End

        //PurchaseOrdersListing
        public ActionResult PurchaseOrdersListing()
        {
            POListingViewModel listingModel = new POListingViewModel();
            POListViewModel model;
            IList<POListViewModel> modelList = new List<POListViewModel>();

            if (listingModel.POStatus == null)
            {
                listingModel.POStatus = 0;
            }

            int pageSize = 10;
            IList<PurchaseOrder> poList = new List<PurchaseOrder>();
            int totalRows = _purchaseOrderRepository.GetFilteredElements(x => x.Approved == listingModel.POStatus && x.FileName != null).Count();

            double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(pageSize));
            listingModel.PageCount = (int)Math.Ceiling(pageCount);
            if (listingModel.CurrentPageIndex == 0)
            {
                listingModel.CurrentPageIndex = 0;
            }
            listingModel.PageSize = pageSize;


            IList<PurchaseOrder> pos = null;
            pos = _purchaseOrderRepository.GetPagedElements(listingModel.CurrentPageIndex, listingModel.PageSize, o => o.Id, x => x.Approved == listingModel.POStatus && x.FileName != null, false).ToList();

            foreach (PurchaseOrder item in pos)
            {
                model = new POListViewModel();
                Company comp = null;
                comp = _companyRepository.GetFilteredElements(o => o.Id == item.CompanyId).FirstOrDefault();
                if (comp != null)
                {
                    model.CompanyName = comp.Name;
                    model.Email = comp.Email;
                }
                else
                {
                    model.CompanyName = string.Empty;
                    model.Email = string.Empty;
                }
                model.ID = item.Id;
                model.OrderNumber = item.OrderNumber;
                model.PODate = item.CreationDate.ToString("d-MMM-yyyy");
                model.Quantity = item.LicensesOrdered;
                model.OrderTotal = item.OrderTotal;
                model.OrderFile = item.OrderFile;
                model.Approved = item.Approved;
                model.FileName = item.FileName;
                model.ContentType = item.ContentType;
                model.SendInvoice = item.SendInvoice ? "Yes" : "No";
                model.RejectReason = item.RejectReason;
                model.LicenseId = item.LicenseId;

                LicenseType licenseType = _licenseTypeRepository.GetElementById(item.LicenseId);

                if(licenseType!=null)
                {
                    model.LicenseTypeName = licenseType.LicenseTypeName;
                }
                else
                {
                    model.LicenseTypeName = "";
                }
                

                if (item.Approved == 1)
                {
                    model.ApprovalDate = item.ApprovalDate.ToString("d-MMM-yyyy");
                    //model.ExpiryDate = item.ApprovalDate.AddYears(1).AddDays(-1).ToString("d-MMM-yyyy");
                    model.ExpiryDate = item.ApprovalDate.AddDays(61).ToString("d-MMM-yyyy");
                }
                else
                {
                    model.ApprovalDate = "";
                    model.ExpiryDate = "";
                }
                
                modelList.Add(model);
            }

            var poStatausList = new List<POStatus>();
            poStatausList.Add(new POStatus() { Value = 0, Text = "Pending" });
            poStatausList.Add(new POStatus() { Value = 1, Text = "Accepted" });
            poStatausList.Add(new POStatus() { Value = 2, Text = "Rejected" });
            listingModel.POStatusList = poStatausList;

            listingModel.POList = modelList;
            return View(listingModel);
        }

        [HttpPost]
        public ActionResult PurchaseOrdersListing(POListingViewModel poListingModel)
        {
            List<int> companyIdList = new List<int>();
            int totalRows = 0;
            IList<Company> companyList = new List<Company>();

            ///////////Filtering Records, Setting Filter
            Expression<Func<PurchaseOrder, bool>> filter = null;
            if (!String.IsNullOrEmpty(poListingModel.SearchString) && (poListingModel.DateFrom == null || poListingModel.DateTo == null))
            {
                companyList = _companyRepository.GetFilteredElements(o => o.Name.Contains(poListingModel.SearchString) || o.Email.Contains(poListingModel.SearchString)).ToList();
                if (companyList != null)
                {
                    foreach (Company item in companyList)
                    {
                        companyIdList.Add(item.Id);
                    }

                }
                filter = x => companyIdList.Contains(x.CompanyId) && x.FileName != null && (x.Approved == poListingModel.POStatus || poListingModel.POStatus == 99);
            }
            if (poListingModel.DateTo != null)
            {
                poListingModel.DateTo = poListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
            }

            if (poListingModel.DateFrom != null && poListingModel.DateTo != null && String.IsNullOrEmpty(poListingModel.SearchString))
            {
                filter = x => x.CreationDate >= poListingModel.DateFrom.Value && x.CreationDate <= poListingModel.DateTo.Value && x.FileName != null && (x.Approved == poListingModel.POStatus || poListingModel.POStatus == 99);
            }

            if (poListingModel.DateFrom != null && poListingModel.DateTo != null && !String.IsNullOrEmpty(poListingModel.SearchString))
            {
                companyList = _companyRepository.GetFilteredElements(o => o.Name.Contains(poListingModel.SearchString) || o.Email.Contains(poListingModel.SearchString)).ToList();
                if (companyList != null)
                {
                    foreach (Company item in companyList)
                    {
                        companyIdList.Add(item.Id);
                    }

                }
                filter = x => (companyIdList.Contains(x.CompanyId)) && x.FileName != null && (x.Approved == poListingModel.POStatus || poListingModel.POStatus == 99) && (x.CreationDate >= poListingModel.DateFrom.Value && x.CreationDate <= poListingModel.DateTo.Value);
            }
            /////////Filtering Records
            IList<PurchaseOrder> poList = new List<PurchaseOrder>();
            if (filter == null)
            {
                poList = _purchaseOrderRepository.GetFilteredElements(x => x.FileName != null && (x.Approved == poListingModel.POStatus || poListingModel.POStatus == 99)).ToList();
            }
            else
            {
                poList = _purchaseOrderRepository.GetFilteredElements(filter).ToList();
            }

            if (poList != null)
            {
                totalRows = poList.Count();
            }

            List<POListViewModel> modelList = new List<POListViewModel>();
            POListViewModel model = null;

            IList<PurchaseOrder> purchaseOrders = null;

            double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(poListingModel.PageSize));
            poListingModel.PageCount = (int)Math.Ceiling(pageCount);

            if (filter == null)
            {
                purchaseOrders = _purchaseOrderRepository.GetPagedElements(poListingModel.CurrentPageIndex, poListingModel.PageSize, o => o.Id, x => x.FileName != null && (x.Approved == poListingModel.POStatus || poListingModel.POStatus == 99), false).ToList();
            }
            else
            {
                purchaseOrders = _purchaseOrderRepository.GetPagedElements(poListingModel.CurrentPageIndex, poListingModel.PageSize, o => o.Id, filter, false).ToList();
            }

            //purchaseOrder = _purchaseOrderRepository.GetAllElements().ToList();

            foreach (PurchaseOrder item in purchaseOrders)
            {
                model = new POListViewModel();
                Company comp = null;
                comp = _companyRepository.GetFilteredElements(o => o.Id == item.CompanyId).FirstOrDefault();
                if (comp != null)
                {
                    model.CompanyName = comp.Name;
                    model.Email = comp.Email;
                }
                else
                {
                    model.CompanyName = string.Empty;
                    model.Email = string.Empty;
                }
                model.ID = item.Id;
                model.OrderNumber = item.OrderNumber;
                model.PODate = item.CreationDate.ToString("d-MMM-yyyy");
                model.Quantity = item.LicensesOrdered;
                model.OrderTotal = item.OrderTotal;
                model.OrderFile = item.OrderFile;
                model.Approved = item.Approved;
                model.FileName = item.FileName;
                model.ContentType = item.ContentType;
                model.SendInvoice = item.SendInvoice ? "Yes" : "No";
                model.RejectReason = item.RejectReason;
                model.LicenseId = item.LicenseId;

                LicenseType licenseType = _licenseTypeRepository.GetElementById(item.LicenseId);

                if (licenseType != null)
                {
                    model.LicenseTypeName = licenseType.LicenseTypeName;
                }
                else
                {
                    model.LicenseTypeName = "";
                }

                if (item.Approved == 1)
                {
                    model.ApprovalDate = item.ApprovalDate.ToString("d-MMM-yyyy");
                    //model.ExpiryDate = item.ApprovalDate.AddYears(1).AddDays(-1).ToString("d-MMM-yyyy");
                    if(item.LicenseId==1)
                    {
                        model.ExpiryDate = item.ApprovalDate.AddDays(61).ToString("d-MMM-yyyy");
                    }
                    else
                    {
                        model.ExpiryDate = "";
                    }
                    
                }
                else
                {
                    model.ApprovalDate = "";
                    model.ExpiryDate = "";
                }
                modelList.Add(model);
            }
            //return View(modelList.ToPagedList(pageno, 4));

            var poStatausList = new List<POStatus>();
            poStatausList.Add(new POStatus() { Value = 0, Text = "Pending" });
            poStatausList.Add(new POStatus() { Value = 1, Text = "Accepted" });
            poStatausList.Add(new POStatus() { Value = 2, Text = "Rejected" });
            poListingModel.POStatusList = poStatausList;


            poListingModel.POList = modelList;
            //return View(poListingModel);
            return PartialView("_PartialPurchaseOrders", poListingModel);
        }
        //PurchaseOrdersListing

        //CompaniesEmployees
        //for getting employees list page wise
        public ActionResult CompaniesEmployees(int companyId)
        {
            EmployeeListingViewModel listingModel = new EmployeeListingViewModel();
            EmployeeListViewModel model;
            IList<EmployeeListViewModel> modelList = new List<EmployeeListViewModel>();
            int pageSize = 10;

            listingModel.CompanyID = companyId;

            IList<User> EmployeeList = new List<User>();
            //int totalRows = _userRepository.GetFilteredElements(x=>x.CompanyId == listingModel.CompanyID).Count();
            int totalRows = 0;
            IList<int> userIds = new List<int>();
            EmployeeList = _userRepository.GetFilteredElements(x => x.CompanyId == listingModel.CompanyID && x.Active == 1, x => x.UserRoles).ToList();

            foreach (User item in EmployeeList)
            {
                foreach (UserRole ur in item.UserRoles)
                {
                    if (ur.RoleId == 3)
                    {
                        userIds.Add(ur.UserId);
                        totalRows++;
                    }
                }
            }

            double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(pageSize));
            listingModel.PageCount = (int)Math.Ceiling(pageCount);

            if (listingModel.CurrentPageIndex == 0)
            {
                listingModel.CurrentPageIndex = 0;
            }
            listingModel.PageSize = pageSize;

            IList<User> users = null;
            users = _userRepository.GetPagedElements(listingModel.CurrentPageIndex, listingModel.PageSize, o => o.Id, x => x.CompanyId == listingModel.CompanyID && x.Active == 1 && userIds.Contains(x.Id), false).ToList();

            foreach (User item in users)
            {
                model = new EmployeeListViewModel();
                model.ID = item.Id;
                model.EmployeeName = item.Name;
                model.EmailID = item.Email;
                model.JoiningDate = item.CreationDate.ToString("d-MMM-yyyy");
                model.Status = item.Active == 1 ? "Activated" : "Deactivated";
                modelList.Add(model);
            }

            listingModel.EmployeesList = modelList;
            return View(listingModel);
        }

        [HttpPost]
        public ActionResult CompaniesEmployees(EmployeeListingViewModel employeeListingModel)
        {

            int totalRows = 0;

            ///////////Filtering Records, Setting Filter
            Expression<Func<User, bool>> filter = null;
            if (employeeListingModel.DateTo != null)
            {
                employeeListingModel.DateTo = employeeListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
            }

            if (!String.IsNullOrEmpty(employeeListingModel.SearchString) && (employeeListingModel.DateFrom == null || employeeListingModel.DateTo == null))
            {
                filter = x => x.CompanyId == employeeListingModel.CompanyID && x.Active == 1 && (x.Name.Contains(employeeListingModel.SearchString) || x.Email.Contains(employeeListingModel.SearchString));
            }

            if (employeeListingModel.DateFrom != null && employeeListingModel.DateTo != null && String.IsNullOrEmpty(employeeListingModel.SearchString))
            {
                employeeListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CompanyId == employeeListingModel.CompanyID && x.Active == 1 && x.CreationDate >= employeeListingModel.DateFrom.Value && x.CreationDate <= employeeListingModel.DateTo.Value;
            }

            if (employeeListingModel.DateFrom != null && employeeListingModel.DateTo != null && !String.IsNullOrEmpty(employeeListingModel.SearchString))
            {
                employeeListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CompanyId == employeeListingModel.CompanyID && x.Active == 1 && (x.Name.Contains(employeeListingModel.SearchString) || x.Email.Contains(employeeListingModel.SearchString))
                && (x.CreationDate >= employeeListingModel.DateFrom.Value && x.CreationDate <= employeeListingModel.DateTo.Value);
            }
            /////////Filtering Records
            IList<User> userList = new List<User>();
            if (filter == null)
            {
                userList = _userRepository.GetFilteredElements(x => x.CompanyId == employeeListingModel.CompanyID && x.Active == 1, x => x.UserRoles).ToList();
            }
            else
            {
                userList = _userRepository.GetFilteredElements(filter, x => x.UserRoles).ToList();
            }

            IList<int> userIds = new List<int>();
            if (userList != null)
            {
                //totalRows = userList.Count();
                foreach (User item in userList)
                {
                    foreach (UserRole ur in item.UserRoles)
                    {
                        if (ur.RoleId == 3)
                        {
                            userIds.Add(ur.UserId);
                            totalRows++;
                        }
                    }
                }
            }

            List<EmployeeListViewModel> modelList = new List<EmployeeListViewModel>();
            EmployeeListViewModel model = null;

            IList<User> users = null;

            double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(employeeListingModel.PageSize));
            employeeListingModel.PageCount = (int)Math.Ceiling(pageCount);
            //employeeListingModel.CompanyID = companyid;
            if (filter == null)
            {
                users = _userRepository.GetPagedElements(employeeListingModel.CurrentPageIndex, employeeListingModel.PageSize, o => o.Id, x => x.CompanyId == employeeListingModel.CompanyID && userIds.Contains(x.Id), false).ToList();
            }
            else
            {
                users = _userRepository.GetPagedElements(employeeListingModel.CurrentPageIndex, employeeListingModel.PageSize, o => o.Id, x => x.CompanyId == employeeListingModel.CompanyID && userIds.Contains(x.Id), false).ToList();
            }

            foreach (var item in users)
            {
                model = new EmployeeListViewModel();
                model.ID = item.Id;
                model.EmployeeName = item.Name;
                model.EmailID = item.Email;
                model.JoiningDate = item.CreationDate.ToString("d-MMM-yyyy");
                model.Status = item.Active == 1 ? "Activated" : "Deactivated";
                modelList.Add(model);
            }
            employeeListingModel.EmployeesList = modelList;

            return PartialView("_PartialCompaniesEmployees", employeeListingModel);
        }

        //InquiryListing
        public ActionResult InquiryListing()
        {
            InquiryListingViewModel listingModel = new InquiryListingViewModel();
            InquiryListViewModel model;
            IList<InquiryListViewModel> modelList = new List<InquiryListViewModel>();
            int pageSize = 10;
            IList<Inquiry> inquiryList = new List<Inquiry>();
            int totalRows = _inquiryRepository.GetAllElements().Count();

            double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(pageSize));
            listingModel.PageCount = (int)Math.Ceiling(pageCount);
            if (listingModel.CurrentPageIndex == 0)
            {
                listingModel.CurrentPageIndex = 0;
            }
            listingModel.PageSize = pageSize;

            IList<Inquiry> inqueries = null;
            inqueries = _inquiryRepository.GetPagedElements(listingModel.CurrentPageIndex, listingModel.PageSize, o => o.Id, false).ToList();

            foreach (Inquiry item in inqueries)
            {
                model = new InquiryListViewModel();
                model.BusinessName = item.BusinessName;
                model.CustomerName = item.CustomerName;
                model.Email = item.Email;
                model.InquiryDate = item.CreationDate.ToString("d-MMM-yyyy");
                modelList.Add(model);
            }

            listingModel.InquiryList = modelList;
            return View(listingModel);
        }

        [HttpPost]
        public ActionResult InquiryListing(InquiryListingViewModel inquiryListingModel)
        {
            int totalRows = 0;
            if (inquiryListingModel.DateTo != null)
            {
                inquiryListingModel.DateTo = inquiryListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
            }
            ///////////Filtering Records, Setting Filter
            Expression<Func<Inquiry, bool>> filter = null;
            if (!String.IsNullOrEmpty(inquiryListingModel.SearchString) && (inquiryListingModel.DateFrom == null || inquiryListingModel.DateTo == null))
            {
                filter = x => x.BusinessName.Contains(inquiryListingModel.SearchString) || x.Email.Contains(inquiryListingModel.SearchString) || x.CustomerName.Contains(inquiryListingModel.SearchString);
            }

            if (inquiryListingModel.DateFrom != null && inquiryListingModel.DateTo != null && String.IsNullOrEmpty(inquiryListingModel.SearchString))
            {
                inquiryListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CreationDate >= inquiryListingModel.DateFrom.Value && x.CreationDate <= inquiryListingModel.DateTo.Value;
            }

            if (inquiryListingModel.DateFrom != null && inquiryListingModel.DateTo != null && !String.IsNullOrEmpty(inquiryListingModel.SearchString))
            {
                inquiryListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
                filter = x => x.CreationDate >= inquiryListingModel.DateFrom.Value && x.CreationDate <= inquiryListingModel.DateTo.Value && (x.BusinessName.Contains(inquiryListingModel.SearchString) || x.Email.Contains(inquiryListingModel.SearchString)
                || x.CustomerName.Contains(inquiryListingModel.SearchString));
            }
            /////////Filtering Records

            IList<Inquiry> inquiryList = new List<Inquiry>();

            if (filter == null)
            {
                inquiryList = _inquiryRepository.GetAllElements().ToList();
            }
            else
            {
                inquiryList = _inquiryRepository.GetFilteredElements(filter).ToList();
            }

            if (inquiryList != null)
            {
                totalRows = inquiryList.Count();
            }

            //CompanyListingViewModel companyListingModel = new CompanyListingViewModel();
            List<InquiryListViewModel> modelList = new List<InquiryListViewModel>();
            InquiryListViewModel model = null;

            IList<Inquiry> inquiries = null;

            double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(inquiryListingModel.PageSize));

            inquiryListingModel.PageCount = (int)Math.Ceiling(pageCount);

            if (filter == null)
            {
                inquiries = _inquiryRepository.GetPagedElements(inquiryListingModel.CurrentPageIndex, inquiryListingModel.PageSize, o => o.Id, false).ToList();
            }
            else
            {
                inquiries = _inquiryRepository.GetPagedElements(inquiryListingModel.CurrentPageIndex, inquiryListingModel.PageSize, o => o.Id, filter, false).ToList();
            }


            foreach (Inquiry item in inquiries)
            {
                model = new InquiryListViewModel();
                model.BusinessName = item.BusinessName;
                model.CustomerName = item.CustomerName;
                model.Email = item.Email;
                model.InquiryDate = item.CreationDate.ToString("d-MMM-yyyy");
                modelList.Add(model);                
            }

            inquiryListingModel.InquiryList = modelList;

            //return companyListingModel;
            return PartialView("_PartialInquiries", inquiryListingModel);
        }

        [HttpPost]
        public async Task<JsonResult> UpdateCompanyStatus(string approved, int companyId, string rejectReason)
        {
            //Email Template
            string emailBody = string.Empty;
            string emailSubject = string.Empty;
            EmailTemplate emailTemplate = null;
            //emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.Name == "Registration").FirstOrDefault();
            emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 9).FirstOrDefault();


            if (emailTemplate != null)
            {
                emailBody = emailTemplate.Template;
                emailSubject = emailTemplate.Subject;
            }
            StringBuilder sbEmail = new StringBuilder(emailBody);

            Company company = _companyRepository.GetElementById(companyId);

            string CompanyName = company.Name;
            string EmailID = company.Email;

            company.Approved = approved;

            _companyRepository.SetModified(company);
            await _companyRepository.UnitOfWork.CommitAsync();

            sbEmail.Replace("[CompanyName]", CompanyName);

            HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
            {
                MailHelper mail = new MailHelper();
                string subject = emailSubject;// MyCard.Web.Resources.CaptionsAll.EmailPasswordCreationSubjectMSO;
                                              //string body = String.Format(@Resources.CaptionsAll.PasswordCreationEmailBody, model.Name, domainName, EncodedCompanyID);
                string body = sbEmail.ToString();//String.Format(@Resources.CaptionsAll.PasswordCreationEmailBodyMSO, model.Name, domainName);
                await mail.SendAsync(EmailID, subject, body, String.Empty);

            });

            return Json(null);
        }

        [HttpPost]
        public async Task<JsonResult> UpdatePO(int approved, int orderId, string rejectReason)
        {
            //Update PO now
            PurchaseOrder po = null;
            int companyId = 0;

            string CompanyName = string.Empty;
            string UserEmail = string.Empty;
            string attachmentName = string.Empty;
            string customerName = string.Empty;
            string customerTitle = string.Empty;
            string qoutationDate = DateTime.UtcNow.Date.ToString("d-MMM-yyyy");
            string orderNo = string.Empty;
            string licenseTypeName = string.Empty;
            decimal price = 0;
            decimal totalCost = 0;
            int NoOfLicense = 0;
           

            po = await _purchaseOrderRepository.GetFirstOrDefaultAsync(o => o.Id == orderId);
            orderNo = po.OrderNumber;
            totalCost = po.OrderTotal;
            NoOfLicense = po.LicensesOrdered;

            var licenseTypes = _licenseTypeRepository.GetElementById(po.LicenseId);

            if (licenseTypes != null)
            {
                licenseTypeName = licenseTypes.LicenseTypeName;
                price = licenseTypes.Price;
            }

            companyId = po.CompanyId;
            po.Approved = approved;
            po.ApprovalDate = DateTime.UtcNow;
            po.RejectReason = rejectReason;

            _purchaseOrderRepository.SetModified(po);
            await _purchaseOrderRepository.UnitOfWork.CommitAsync();


            Company company = _companyRepository.GetElementById(companyId);

            CompanyName = company.Name;
            customerName = company.CustomerName;
            customerTitle = company.BusinessTitle;

            //change status to Premium
            //if(company.CustomerType == "Freemium")
            //{
            //    company.CustomerType = "Premium";
            //    _companyRepository.SetModified(company);
            //    await _companyRepository.UnitOfWork.CommitAsync();
            //}
            

            //Email Template
            string emailBody = string.Empty;
            string emailSubject = string.Empty;
            EmailTemplate emailTemplate = null;
            //emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.Name == "Order Approval").FirstOrDefault();

            if(po.LicenseId==1)
            {
                emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 8).FirstOrDefault();
            }
            else if(po.LicenseId == 2)
            {
                emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 7).FirstOrDefault();
            }
            

            if (emailTemplate != null)
            {
                emailBody = emailTemplate.Template;
                emailSubject = emailTemplate.Subject;
            }
            StringBuilder sb = new StringBuilder(emailBody);
            sb.Replace("[CompanyName]", company.Name);

            //Email Template

            

            if (approved == 1)
            {
                //Make Invoice pdf Start
                Document pdfDoc = new Document(PageSize.A4, 20f, 20f, 40f, 0f);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                    pdfDoc.Open();

                    try
                    {


                        string imageURL = Server.MapPath("~") + "/img/logo.png";

                        iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);

                        //Resize image depend upon your need
                        jpg.ScaleToFit(100f, 100f);

                        //Give space before image
                        jpg.SpacingBefore = 10f;

                        //Give some space after the image
                        jpg.SpacingAfter = 1f;
                        //jpg.Alignment = Element.ALIGN_LEFT | iTextSharp.text.Image.UNDERLYING;
                        jpg.Alignment = iTextSharp.text.Image.UNDERLYING;

                        pdfDoc.Add(Chunk.NEWLINE);
                        //pdfDoc.Add(paragraph);
                        pdfDoc.Add(jpg);

                        pdfDoc.Add(Chunk.NEWLINE);
                        //Registering Font
                        string fontpath = Server.MapPath("~") + "/css/Gotham/";

                        BaseFont customfont = BaseFont.CreateFont(fontpath + "Gotham-Book.otf", BaseFont.CP1252, BaseFont.EMBEDDED);

                        //iTextSharp.text.Font font = new iTextSharp.text.Font(customfont, 16, Font.BOLD , iTextSharp.text.BaseColor.BLUE);
                        iTextSharp.text.Font mainHeadingfont = new iTextSharp.text.Font(customfont, 16, Font.NORMAL, new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml("#2ea0da")));

                        Font normalFont = new Font(customfont, 11, Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                        Font normalFont10 = new Font(customfont, 10, Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                        Font normalFontWhite = new Font(customfont, 11, Font.NORMAL, iTextSharp.text.BaseColor.WHITE);
                        Font normalFontWhite10 = new Font(customfont, 10, Font.NORMAL, iTextSharp.text.BaseColor.WHITE);
                        Font normalFont12 = new Font(customfont, 12, Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                        Font normalFont12LightBlue = new Font(customfont, 12, Font.NORMAL, new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml("#2ea0da")));

                        Font boldFont10 = new Font(customfont, 10, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                        Font boldFont11 = new Font(customfont, 11, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                        Font boldFont11UnderLine = new Font(customfont, 11, Font.BOLD | Font.UNDERLINE, iTextSharp.text.BaseColor.BLACK);

                        //Font lightblue = new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL, new Color(43, 145, 175));
                        //Registering Font

                        Paragraph pInvoice = new Paragraph("Invoice", mainHeadingfont);
                        pInvoice.Alignment = Element.ALIGN_CENTER;
                        pInvoice.SpacingBefore = 30f;
                        //pInvoice.b
                        pdfDoc.Add(pInvoice);


                        Paragraph pDate = new Paragraph(qoutationDate, normalFont);
                        pDate.Alignment = Element.ALIGN_RIGHT;
                        pdfDoc.Add(pDate);

                        Paragraph pDateCustomerName = new Paragraph(customerName, normalFont12);
                        pDateCustomerName.Alignment = Element.ALIGN_LEFT;
                        pdfDoc.Add(pDateCustomerName);

                        Paragraph pTitle = new Paragraph(customerTitle, normalFont12);
                        pTitle.Alignment = Element.ALIGN_LEFT;
                        pdfDoc.Add(pTitle);

                        Paragraph pCompany = new Paragraph(CompanyName, normalFont12);
                        pCompany.Alignment = Element.ALIGN_LEFT;
                        pdfDoc.Add(pCompany);

                        pdfDoc.Add(Chunk.NEWLINE);

                        Paragraph pQno = new Paragraph("Invoice no: MCI-" + orderNo.Substring(4, orderNo.Length - 4), normalFont12);
                        pQno.Alignment = Element.ALIGN_LEFT;
                        pdfDoc.Add(pQno);

                        //pdfDoc.Add(Chunk.NEWLINE);

                        PdfPTable table = new PdfPTable(6);
                        table.WidthPercentage = 100;
                        table.SpacingBefore = 20f;

                        PdfPCell cell = new PdfPCell(new Phrase("Description", normalFontWhite));
                        cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#2ea0da"));
                        cell.MinimumHeight = 40f;
                        cell.Colspan = 2;
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("License Type", normalFontWhite));
                        cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#2ea0da"));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("No Of Licenses", normalFontWhite));
                        cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#2ea0da"));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("Unit Price (USD)", normalFontWhite));
                        cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#2ea0da"));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("Total", normalFontWhite));
                        cell.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#2ea0da"));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);


                        cell = new PdfPCell(new Phrase("myCards Enterprise Software as a Service (Saas)"));
                        cell.MinimumHeight = 40f;
                        cell.Colspan = 2;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(licenseTypeName));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(NoOfLicense.ToString()));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(String.Format("{0:n}", price)));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(String.Format("{0:n}", totalCost)));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.MinimumHeight = 40f;
                        cell.Colspan = 2;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("Sub Total (USD)", boldFont10));
                        cell.MinimumHeight = 35f;
                        cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                        cell.Colspan = 2;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(String.Format("{0:n}", totalCost)));
                        cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("Value Added Tax (VAT)", boldFont10));
                        cell.MinimumHeight = 25f;
                        cell.Colspan = 2;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("5%", boldFont11));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase("Total Amount (USD)", boldFont11));
                        cell.MinimumHeight = 40f;
                        cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                        cell.Colspan = 2;
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        cell = new PdfPCell(new Phrase(""));
                        cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        Decimal totalOrderCost = totalCost + (totalCost * 5 / 100);

                        cell = new PdfPCell(new Phrase(String.Format("{0:n}", totalOrderCost)));
                        cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        table.AddCell(cell);

                        pdfDoc.Add(table);

                        //pdfDoc.Add(Chunk.NEWLINE);
                        ///Bank Details Table Start
                        PdfPTable tableBank = new PdfPTable(2);
                        tableBank.WidthPercentage = 94;
                        tableBank.SpacingBefore = 10f;

                        PdfPCell cell1 = new PdfPCell(new Phrase("Account Details", normalFontWhite10));
                        cell1.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#2ea0da"));
                        cell1.MinimumHeight = 20f;
                        cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("Reference", normalFontWhite10));
                        cell1.BackgroundColor = new BaseColor(System.Drawing.ColorTranslator.FromHtml("#2ea0da"));
                        cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("Bank Name", normalFont10));
                        cell1.MinimumHeight = 20f;
                        cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("First Abu Dhabi Bank", normalFont10));
                        cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("Account Name", normalFont10));
                        cell1.MinimumHeight = 20f;
                        cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("MYCARDS LIMITED", normalFont10));
                        cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("Account Number", normalFont10));
                        cell1.MinimumHeight = 20f;
                        cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("1411323867592014", normalFont10));
                        cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("IBAN Number", normalFont10));
                        cell1.MinimumHeight = 20f;
                        cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("AE820351411323867592014", normalFont10));
                        cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("Swift Code", normalFont10));
                        cell1.MinimumHeight = 20f;
                        cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        cell1 = new PdfPCell(new Phrase("NBADAEAA", normalFont10));
                        cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
                        tableBank.AddCell(cell1);

                        pdfDoc.Add(tableBank);
                        ///Bank Details Table End


                        //pdfDoc.Add(Chunk.NEWLINE);


                        Paragraph tc = new Paragraph("Terms & Conditions", boldFont11UnderLine);
                        tc.Alignment = Element.ALIGN_LEFT;
                        tc.SpacingBefore = 20f;
                        pdfDoc.Add(tc);

                        Paragraph tcLine1 = new Paragraph("    •  Payment Terms: 45 days from invoice date", normalFont);
                        tcLine1.Alignment = Element.ALIGN_LEFT;
                        //tcLine1.SpacingBefore = 20f;
                        pdfDoc.Add(tcLine1);

                        Paragraph tcLine2 = new Paragraph("    •  Access to the myCards Enterprise Solution will be locked if payment is 15 days ", normalFont);
                        tcLine2.Alignment = Element.ALIGN_LEFT;
                        //tcLine2.SpacingBefore = 20f;
                        pdfDoc.Add(tcLine2);

                        Paragraph tcLine21 = new Paragraph("        overdue", normalFont);
                        tcLine21.Alignment = Element.ALIGN_LEFT;
                        //tcLine2.SpacingBefore = 20f;
                        pdfDoc.Add(tcLine21);

                        string imageSecondURL = Server.MapPath("~") + "/img/mail-bottom.png";

                        iTextSharp.text.Image jpgSecond = iTextSharp.text.Image.GetInstance(imageSecondURL);

                        jpgSecond.SpacingAfter = 1f;
                        //jpgSecond.Alignment = Element.ALIGN_RIGHT | iTextSharp.text.Image.UNDERLYING;
                        jpgSecond.Alignment = iTextSharp.text.Image.UNDERLYING;
                        //jpg.Alignment = Element.ALIGN_LEFT | iTextSharp.text.Image.UNDERLYING;

                        jpgSecond.ScaleToFit(250f, 250f);

                        //jpgSecond.Alignment = Image.TEXTWRAP | Image.ALIGN_RIGHT;

                        jpgSecond.IndentationLeft = 9f;

                        jpgSecond.SpacingAfter = 9f;

                        //jpgSecond.BorderWidthTop = 36f;

                        //jpg.BorderColorTop = Color.WHITE;

                        jpgSecond.SetAbsolutePosition(330, 30);

                        pdfDoc.Add(jpgSecond);

                        pdfDoc.Add(Chunk.NEWLINE);
                        pdfDoc.Add(Chunk.NEWLINE);


                        Paragraph br = new Paragraph("Best Regards,", normalFont12);
                        br.Alignment = Element.ALIGN_LEFT;
                        pdfDoc.Add(br);

                        pdfDoc.Add(Chunk.NEWLINE);

                        Paragraph mst = new Paragraph("myCards Accounts Team", normalFont12);
                        mst.Alignment = Element.ALIGN_LEFT;
                        pdfDoc.Add(mst);

                        Paragraph emailString = new Paragraph("accounts@mycards.com", normalFont12LightBlue);
                        emailString.Alignment = Element.ALIGN_LEFT;
                        pdfDoc.Add(emailString);

                    }

                    catch (Exception ex)
                    {
                    }

                    finally
                    {
                        pdfDoc.Close();
                    }



                    attachmentName = "MyCard-Invoice-" + orderNo + ".pdf";

                    pdfDoc.Close();
                    byte[] bytes = memoryStream.ToArray();
                    memoryStream.Close();
                    
                    UserEmail = company.Email;
                    //UserEmail = "jehangir.ali@gmail.com";
                    //Make Invoice pdf End
                                        
                    //stream.Read(fileBytes, 0, fileBytes.Length);
                    //stream.Close();

                    if (approved == 1)
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
                        {
                            MailHelper mail = new MailHelper();
                            string subject = emailSubject;// MyCard.Web.Resources.CaptionsAll.QoutationEmailSubject;
                            string body = sb.ToString();// String.Format(MyCard.Web.Resources.CaptionsAll.QoutationEmailBody, CompanyName);
                            await mail.SendAsyncAttach(UserEmail, subject, body, bytes, attachmentName);
                                                        
                        });
                    }

                }
            }



            //if (approved==1)
            //{

            //    HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
            //    {
            //        MailHelper mail = new MailHelper();
            //        string subject = emailSubject;// MyCard.Web.Resources.CaptionsAll.LicenseActivationSubject;
            //        string body = sb.ToString();// String.Format(@Resources.CaptionsAll.LicenseActivationEmailMessage, company.Name);
            //        await mail.SendAsync(company.Email, subject, body, String.Empty);

            //    });

            //}

            return Json(null);
        }

        [HttpGet]
        public FileResult DownloadFile(int orderId)
        {
            PurchaseOrder po = null;
            byte[] bytes = null;
            string fileName = string.Empty, contentType = string.Empty;

            //contentType = "application/pdf";
            //fileName = "order";

            po = _purchaseOrderRepository.GetFilteredElements(o=>o.Id == orderId).SingleOrDefault();

            if(po != null)
            {
                bytes = (byte[])po.OrderFile;
                contentType = po.ContentType;
                fileName = po.FileName + ".pdf";
                return File(bytes, contentType, fileName);
            }
            else
            {
                return null;
            }


            
            
        }

        //Logout
        //[HttpPost]
        public ActionResult Logout()
        {
            System.Web.HttpContext.Current.Session.Clear();
            System.Web.HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Default");
        }

        // GET: Admin/Default/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Admin/Default/Login
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            string DecrptedPassword = string.Empty;
            bool IsAdmin=false;
            string rolename = string.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    Expression<Func<User, object>>[] includes =
                    {
                        x => x.UserRoles
                    };

                    string pass = string.Empty;
                    pass = MyCard.Helper.EncryptionHelper.DecryptString("MHrz6Gk3lAa0PsxexTspAA==");

                    DecrptedPassword = MyCard.Helper.EncryptionHelper.EncryptString(model.Password);
                    User user = null;
                    user = await _userRepository.GetFirstOrDefaultAsync(o => o.Email == model.UserName, default(CancellationToken), includes);
                    
                    foreach(var item in user.UserRoles)
                    {
                        if (item.RoleId == 1)
                        {
                            IsAdmin = true;
                        }
                    }

                    foreach (var item in user.UserRoles)
                    {
                        if (item.RoleId == 1)
                        {
                            rolename = rolename + "super_admin,";
                        }
                        else if (item.RoleId == 2)
                        {
                            rolename = rolename + "company_owner,";
                        }
                        else
                        {
                            rolename = rolename + "user,";
                        }
                    }

                    if (IsAdmin)
                    {
                        if (user.Password == DecrptedPassword)
                        {
                            //FormsAuthentication.SetAuthCookie(user.Name + "|" + user.Id, false);
                            var authTicket = new FormsAuthenticationTicket(
                                                  1,
                                                  user.Name + "|" + user.Id,  //user id
                                                  DateTime.Now,
                                                  DateTime.Now.AddMinutes(180),  // expiry
                                                  model.RememberMe,  //true to remember
                                                  rolename, //roles 
                                                  "/"
                                                );

                            //encrypt the ticket and add it to a cookie
                            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                            Response.Cookies.Add(cookie);


                            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Default");
                            }
                        }
                        else
                        {
                            ViewBag.Message = MyCard.Web.Resources.ErrorMessages.UserNamePasswordNotMatch;
                        }
                    }
                    else
                    {
                        ViewBag.Message = MyCard.Web.Resources.ErrorMessages.LoginForAdminOnly;
                    }
                    return View();
                }
                catch
                {
                    ViewBag.Message = MyCard.Web.Resources.ErrorMessages.UserNamePasswordNotMatch;
                    return View();
                }
            }
            else
            {
                return View();
            }
        }

        // GET: ChangePassword
        [Authorize]
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: ChangePassword
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            string EncryptedPassword = string.Empty;
            string EncryptedOldPassword = string.Empty;

            EncryptedOldPassword = MyCard.Helper.EncryptionHelper.EncryptString(model.OldPassword);
            ViewBag.Message = string.Empty;

            int UserID = 0;
            //Get User ID From Cookie
            if (HttpContext.Request.IsAuthenticated)
            {
                UserID = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (ModelState.IsValid)
            {
                User user = new User();
                user = await _userRepository.GetFirstOrDefaultAsync(o => o.Id == UserID);
                
                if (user!=null)
                {
                    if(user.Password==EncryptedOldPassword)
                    {
                        EncryptedPassword = MyCard.Helper.EncryptionHelper.EncryptString(model.Password);
                        user.Password = EncryptedPassword;
                        _userRepository.SetModified(user);

                        await _userRepository.UnitOfWork.CommitAsync();
                    }
                    else
                    {
                        ViewBag.Message = MyCard.Web.Resources.ErrorMessages.OldPasswordNotCorrect;
                        return View();
                    }
                }
                
                return RedirectToAction("ChangePasswordThanks");
            }
            return View();
        }

        // GET: Admin ChangePasswordThanks
        [Authorize]
        public ActionResult ChangePasswordThanks()
        {
            return View();
        }

        [Authorize]
        public ActionResult CardShared()
        {

            return View();
        }

        public ActionResult CompaniesListForDropDown()
        {
            var companies = _companyRepository.GetAllElements().Select(x=> new { x.Id, x.Name}).ToList();
            
            return Json(companies, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CompanyYears(int companyId)
        {
            int companyCreationYear = 0;

            Company company = new Company();
            if(companyId==0)
            {
                company = _companyRepository.GetAllElements().OrderBy(o => o.CreationDate).FirstOrDefault();
                if(company!=null)
                {
                    companyCreationYear = company.CreationDate.Year;
                }
            }
            else
            {
                company = _companyRepository.GetElementById(companyId);
                if (company != null)
                {
                    companyCreationYear = company.CreationDate.Year;
                }
            }
            
            
            IList<int> years = new List<int>();
            for (int i = DateTime.UtcNow.Year; i >= companyCreationYear; i--)
            {
                years.Add(i);
            }
            return Json(years, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CardShareChart(int? companyId, int year)
        {
            //Card Shares Chart Data
            IList<int> userlist = null;
            
            IList<ContactShareActivity> cardShares = null;
            List<int> cardShareCount = new List<int>();
            
            if(companyId==null || companyId == 0)
            {
                userlist = _userRepository.GetFilteredElements(o => o.Active==1).Select(t => t.Id).ToList();
            }
            else
            {
                userlist = _userRepository.GetFilteredElements(o => o.CompanyId == companyId && o.Active == 1).Select(t => t.Id).ToList();
            }
            

            cardShares = _contactShareActivityRepository.GetFilteredElements(o => o.CreationDate.Year == year && userlist.Contains(o.SenderId)).ToList();

            for (int i = 1; i <= 12; i++)
            {
                cardShareCount.Add(cardShares.Count(o => o.CreationDate.Month == i));
            }
            //}

            return Json(new { datalist = cardShareCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SharedCardCount(int? companyId, int year)
        {
            //Card Shares Chart Data
            //User user = null;
            IList<int> userlist = null;
            
            int cardSharesCount = 0;

            if (companyId==0 || companyId == null)
            {
                userlist = _userRepository.GetFilteredElements(o => o.Active == 1).Select(t => t.Id).ToList();
            }
            else
            {
                userlist = _userRepository.GetFilteredElements(o => o.CompanyId == companyId && o.Active == 1).Select(t => t.Id).ToList();
            }
            //userlist = _userRepository.GetFilteredElements(o => o.CompanyId == companyId).Select(t => t.Id).ToList();

            cardSharesCount = _contactShareActivityRepository.GetFilteredElements(o => o.CreationDate.Year == year && userlist.Contains(o.SenderId)).Count();
            
            return Json(cardSharesCount, JsonRequestBehavior.AllowGet);

        }

        //////Revenue/////
        [Authorize]
        public ActionResult CompanyRevenue()
        {

            return View();
        }

        public ActionResult RevenueChart(int? companyId, int year)
        {
            //Card Shares Chart Data
            IList<Decimal> revenueList = new List<Decimal>();

            IList<PurchaseOrder> poList = null;
            //List<int> cardShareCount = new List<int>();

            if (companyId == null || companyId == 0)
            {
                poList = _purchaseOrderRepository.GetFilteredElements(o => o.Approved == 1 && o.CreationDate.Year==year).ToList();
            }
            else
            {
                poList = _purchaseOrderRepository.GetFilteredElements(o => o.Approved == 1 && o.CreationDate.Year == year && o.CompanyId== companyId).ToList();
            }


            //revenueList = _contactShareActivityRepository.GetFilteredElements(o => o.CreationDate.Year == year && userlist.Contains(o.SenderId)).ToList();
            
            for (int i = 1; i <= 12; i++)
            {
                if(poList.Count(o => o.CreationDate.Month == i)>0)
                {
                    revenueList.Add(poList.Count(o => o.CreationDate.Month == i) > 0 ? poList.Where(o => o.CreationDate.Month == i).Sum(o => o.OrderTotal) : 0);
                }
                else
                {
                    revenueList.Add(0);
                }
                
            }
            //}

            return Json(new { datalist = revenueList }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]

        public ActionResult SaveEmailTemplate(EmailTemplateViewModel model)
        {
            if (model != null && model.ID != 0)
            {
                EmailTemplate template = _emailTemplateRepository.GetElementById(model.ID);
                template.Template = model.EmailTemplate;
                template.Subject = model.Subject;
                template.LastUpdateDate = DateTime.UtcNow;
                _emailTemplateRepository.SetModified(template);
                _emailTemplateRepository.UnitOfWork.Commit();
            }
            return RedirectToAction("EmailComposer", new { id = model.ID });
        }

        public ActionResult EmailComposer(int? id)
        {
            int templateId = 0;
            List<EmailTemplate> templates = _emailTemplateRepository.GetAllElements().ToList();
            EmailTemplateViewModel viewModel = new EmailTemplateViewModel();
            List<SelectListItem> selectList = new List<SelectListItem>();
            if (id == null || id.Value == 0)
            {
                templateId = 0;
            }
            else
            {
                templateId = id.Value;
                viewModel.EmailTemplate = templates.First(t => t.Id == id.Value).Template;
                viewModel.Subject = templates.First(t => t.Id == id.Value).Subject;
                viewModel.ID = id.Value;
            }
            selectList.Add(new SelectListItem
            {
                Value = "0",
                Text = "Select",
                Selected = templateId == 0? true:false
            });
            templates.ForEach(template =>
            {
                selectList.Add(new SelectListItem
                {
                    Value = template.Id.ToString(),
                    Text = template.Name,
                    Selected = templateId == template.Id ? true:false
                });   
            });
            viewModel.EmailTemplates = selectList;
            return View(viewModel);
        }


        public ActionResult TotalRevenue(int? companyId, int year)
        {
            //Total revenue for the year
            decimal totalRevenue = 0;

            if (companyId == null || companyId == 0)
            {
                totalRevenue = _purchaseOrderRepository.GetFilteredElements(o => o.Approved == 1 && o.CreationDate.Year == year).Sum(t => t.OrderTotal);
            }
            else
            {
                totalRevenue = _purchaseOrderRepository.GetFilteredElements(o => o.Approved == 1 && o.CreationDate.Year == year && o.CompanyId==companyId).Sum(t => t.OrderTotal);
            }

            return Json(totalRevenue, JsonRequestBehavior.AllowGet);

        }
        ////////////

        [Authorize]
        public ActionResult ChangePricing()
        {
            LicenseTypeModel model = null;
            IList<LicenseTypeModel> modelList = new List<LicenseTypeModel>();

            IList<LicenseType> licenseTypeList = new List<LicenseType>();
            licenseTypeList = _licenseTypeRepository.GetAllElements().ToList();

            if(licenseTypeList!=null)
            {
                foreach(LicenseType item in licenseTypeList)
                {
                    model = new LicenseTypeModel();
                    model.Id = item.Id;
                    model.LicenseTypeName = item.LicenseTypeName;
                    model.Price = item.Price;
                    modelList.Add(model);
                }
            }

            return View(modelList);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ChangePricing(List<LicenseTypeModel> modelList)
        {
            if (ModelState.IsValid)
            {
                LicenseType licenseType = null;

                foreach(LicenseTypeModel item in modelList)
                {
                    licenseType = await _licenseTypeRepository.GetFirstOrDefaultAsync(x => x.Id == item.Id);
                    if(licenseType.Price != item.Price)
                    {
                        licenseType.Price = item.Price;
                        _licenseTypeRepository.SetModified(licenseType);

                        await _licenseTypeRepository.UnitOfWork.CommitAsync();
                    }
                }
                
                return RedirectToAction("Index", "Default");
            }
            return View(modelList);
        }

        //Change Descriptions
        [Authorize]
        public ActionResult ChangeDescription()
        {
            LicenseDescriptionModel model = null;
            IList<LicenseDescriptionModel> modelList = new List<LicenseDescriptionModel>();

            IList<LicenseDescription> licenseDescriptionList = new List<LicenseDescription>();
            licenseDescriptionList = _licenseDescriptionRepository.GetAllElements().ToList();

            if (licenseDescriptionList != null)
            {
                foreach (LicenseDescription item in licenseDescriptionList)
                {
                    model = new LicenseDescriptionModel();
                    model.Id = item.Id;
                    model.LicenseName = item.LicenseName;
                    model.Description = item.Description;
                    modelList.Add(model);
                }
            }

            return View(modelList);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> ChangeDescription(List<LicenseDescriptionModel> modelList)
        {
            if (ModelState.IsValid)
            {
                LicenseDescription licenseDescription = null;

                foreach (LicenseDescriptionModel item in modelList)
                {
                    licenseDescription = await _licenseDescriptionRepository.GetFirstOrDefaultAsync(x => x.Id == item.Id);
                    if (licenseDescription.Description != item.Description)
                    {
                        licenseDescription.Description = item.Description;
                        _licenseDescriptionRepository.SetModified(licenseDescription);

                        await _licenseDescriptionRepository.UnitOfWork.CommitAsync();
                    }
                }

                return RedirectToAction("Index", "Default");
            }
            return View(modelList);
        }

        // GET: My Files
        public ActionResult MyCardFiles()
        {

            MyCardFileViewModel model = new MyCardFileViewModel();

            MyCardFile myCardFile = _myCardFileRepository.GetAllElements().OrderByDescending(x => x.Id).FirstOrDefault();

            if (myCardFile!=null)
            {
                model.FileName = myCardFile.FileName;
                model.FileContent = myCardFile.FileContent;
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> MyCardFiles(HttpPostedFileBase file)
        {

            if (file != null && file.ContentLength > 0)
            {
                string ext = Path.GetExtension(file.FileName);
                //if (String.IsNullOrEmpty(ext) || ( !ext.Equals(".pdf", StringComparison.OrdinalIgnoreCase) && !ext.Equals(".docx", StringComparison.OrdinalIgnoreCase)))
                if (String.IsNullOrEmpty(ext) || !ext.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    //ViewBag.Message = "File is not pdf/doc format";
                    //return View();
                    return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.POUploadFileTypeError }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    byte[] uploadedFile = new byte[file.InputStream.Length];
                    //file.InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                    using (BinaryReader br = new BinaryReader(file.InputStream))
                    {
                        uploadedFile = br.ReadBytes(file.ContentLength);
                    }

                    MyCardFile myCardFile = null;

                    myCardFile = _myCardFileRepository.GetAllElements().OrderByDescending(x => x.Id).FirstOrDefault();

                    if(myCardFile != null)
                    {
                        myCardFile.FileContent = uploadedFile;
                        myCardFile.FileName = file.FileName;

                        _myCardFileRepository.SetModified(myCardFile);
                    }
                    else
                    {
                        myCardFile = new MyCardFile();

                        myCardFile.FileContent = uploadedFile;
                        myCardFile.FileName = file.FileName;

                        _myCardFileRepository.Add(myCardFile);
                    }


                    
                    await _myCardFileRepository.UnitOfWork.CommitAsync();

                    //return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
                    //return View();
                }


            }
            //ALL STRINGS IN RESOURCE FILES

            //return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.PurchaseUploadPOFileNotSelectedError }, JsonRequestBehavior.AllowGet);
            
            return RedirectToAction("MyCardFiles", "Default");
        }

        [HttpGet]
        public FileResult DownloadMyCardFile()
        {

            byte[] bytes = null;
            string fileName = string.Empty, contentType = string.Empty;

            //contentType = "application/pdf";
            //fileName = "order";

            MyCardFile myCardFile = null;

            myCardFile = _myCardFileRepository.GetAllElements().OrderByDescending(x => x.Id).FirstOrDefault();

            if (myCardFile != null)
            {
                bytes = (byte[])myCardFile.FileContent;
                contentType = "application/pdf";
                fileName = myCardFile.FileName + ".pdf";
                return File(bytes, contentType, fileName);
            }
            else
            {
                return null;
            }

        }

        [HttpGet]
        public ActionResult RegisterCompany()
        {
            RegistrationViewModel model = new RegistrationViewModel();
            
            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterCompany(RegistrationViewModel model)
        {
            ViewBag.PageType = "Register";
            if (ModelState.IsValid)
            {
                int existingCompanyCount = 0;
                string searchString = string.Empty;

                searchString = model.DomainName.ToUpper();
                string EncryptedCompanyID = string.Empty;
                int CompanyID = 0;
                string EncodedCompanyID = string.Empty;


                if (!String.IsNullOrEmpty(model.EmailID))
                {
                    existingCompanyCount = _companyRepository.GetFilteredElements(o => o.Email.ToUpper().Equals(model.EmailID)).Count();
                }
                if (existingCompanyCount > 0)
                {
                    return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.EmailAlreadyExists }, JsonRequestBehavior.AllowGet);
                }

                Company existingCompany = null;
                ///////
                if (!String.IsNullOrEmpty(searchString))
                {
                    //existingCompanyCount = _companyRepository.GetFilteredElements(o => o.Domain.ToUpper().Equals(searchString)).Count();
                    existingCompany = _companyRepository.GetFilteredElements(o => o.Domain.ToUpper().Equals(searchString)).FirstOrDefault();
                }

                if (existingCompany != null)
                {
                    //Email Template
                    string emailBody = string.Empty;
                    string emailSubject = string.Empty;
                    EmailTemplate emailTemplate = null;
                    //emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.Name == "Not Corporate Account").FirstOrDefault();
                    emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 6).FirstOrDefault();


                    if (emailTemplate != null)
                    {
                        emailBody = emailTemplate.Template;
                        emailSubject = emailTemplate.Subject;
                    }
                    StringBuilder sbEmail = new StringBuilder(emailBody);
                    sbEmail.Replace("[CustomerName]", model.CustomerName);
                    sbEmail.Replace("[CustomerEmail]", model.EmailID);
                    sbEmail.Replace("[CorporateEmail]", existingCompany.Email);

                    HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
                    {
                        MailHelper mail = new MailHelper();
                        string subject = emailSubject;// MyCard.Web.Resources.CaptionsAll.EmailPasswordCreationSubjectMSO;
                        //string body = String.Format(@Resources.CaptionsAll.PasswordCreationEmailBody, model.Name, domainName, EncodedCompanyID);
                        string body = sbEmail.ToString();//String.Format(@Resources.CaptionsAll.PasswordCreationEmailBodyMSO, model.Name, domainName);
                        await mail.SendAsync(model.EmailID, subject, body, String.Empty);

                    });
                    return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.DomainAlreadyRegistered }, JsonRequestBehavior.AllowGet);
                }


                try
                {
                    //Saving Company Record Start

                    Company company = new Company();
                    company.Name = model.Name;
                    company.Domain = model.DomainName;
                    company.CustomerName = model.CustomerName;
                    company.Email = model.EmailID;
                    company.BusinessTitle = model.BusinessTitle;
                    company.Industry = model.Industry;
                    company.City = model.City;
                    company.Country = model.Country;
                    company.CustomerType = model.CustomerType;
                    company.ServerIp = model.ServerIP;

                    User user = new User();
                    user.Name = model.CustomerName;
                    user.Email = model.EmailID;
                    user.Active = 1;

                    if (company.Users == null)
                    {
                        company.Users = new List<User>();
                    }
                    user.Company = company;
                    company.Users.Add(user);

                    //Assigning Role
                    UserRole userRole = new UserRole();
                    userRole.RoleId = 2;//Company Owner

                    if (user.UserRoles == null)
                    {
                        user.UserRoles = new List<UserRole>();
                    }
                    user.UserRoles.Add(userRole);
                    //
                    _companyRepository.Add(company);


                    await _companyRepository.UnitOfWork.CommitAsync();

                    //Saving Company Record End
                    string domainName = string.Empty;
                    Uri url = HttpContext.Request.Url;
                    domainName = url.AbsoluteUri.Replace(url.PathAndQuery, string.Empty);

                    CompanyID = company.Id;

                    //Sending Email Code Start
                    TempData["FirstName"] = model.Name;
                    //Sending Password Creation Email
                    //EncryptedCompanyID = MyCard.Helper.EncryptionHelper.EncryptString(Convert.ToString(CompanyID));+ "|" + System.DateTime.UtcNow
                    EncryptedCompanyID = MyCard.Helper.EncryptionHelper.EncryptString(Convert.ToString(CompanyID + "|" + System.DateTime.UtcNow));
                    EncodedCompanyID = HttpUtility.UrlEncode(EncryptedCompanyID);


                    //Email Template
                    string emailBody = string.Empty;
                    string emailSubject = string.Empty;
                    EmailTemplate emailTemplate = null;
                    //emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.Name == "Registration").FirstOrDefault();
                    emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 10).FirstOrDefault();


                    if (emailTemplate != null)
                    {
                        emailBody = emailTemplate.Template;
                        emailSubject = emailTemplate.Subject;
                    }
                    StringBuilder sbEmail = new StringBuilder(emailBody);
                    sbEmail.Replace("[CompanyName]", model.Name);

                    
                    string sLink = "<a href ='" + domainName + "/CompanyDashboard/CreatePassword/?param=" + EncodedCompanyID + "'>" + domainName + "/CompanyDashboard/CreatePassword/?param=" + EncodedCompanyID + "</a>";
                    sbEmail.Replace("[CreatePasswordLink]", sLink);


                    HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
                    {
                        MailHelper mail = new MailHelper();
                        string subject = emailSubject;// MyCard.Web.Resources.CaptionsAll.EmailPasswordCreationSubjectMSO;
                                                        //string body = String.Format(@Resources.CaptionsAll.PasswordCreationEmailBody, model.Name, domainName, EncodedCompanyID);
                        string body = sbEmail.ToString();//String.Format(@Resources.CaptionsAll.PasswordCreationEmailBodyMSO, model.Name, domainName);
                        await mail.SendAsync(model.EmailID, subject, body, String.Empty);

                    });

                    //  Send "Success"
                    return Json(new { success = true, responseText = MyCard.Web.Resources.CaptionsAll.RegistrationThankyouMessage }, JsonRequestBehavior.AllowGet);
                    


                    //return RedirectToAction("RegistrationFormThanks", "Registration");

                }
                catch (Exception ex)
                {
                    //  Send "false"
                    //return Json(new { success = false, responseText = ex.Message });
                    return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.ExceptionError });

                    //ViewBag.ErrorMessage = ex.Message;
                    //return View();
                }


            }
            else
            {
                return View();
            }

        }


        [HttpGet]
        public JsonResult GetJsonCity(string countryName)
        {
            var list = new List<DropDownListViewModel>();
            try
            {
                //var httpClient = new HttpClient();
                //httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //HttpResponseMessage response;
                //response = httpClient.GetAsync("http://localhost:59866/api/" + "cityDetails/GetAllCityDetails?stateId=" + stateId + "").Result;
                //response.EnsureSuccessStatusCode();
                //List<City> cityList = response.Content.ReadAsAsync<List<City>>().Result;

                IList<City> cityList = _cityRepository.GetFilteredElements(x => x.CountryName == countryName).OrderBy(x => x.CityName).ToList();

                if (!object.Equals(cityList, null))
                {
                    var cities = cityList.ToList();
                    foreach (var item in cities)
                    {
                        list.Add(new DropDownListViewModel
                        {
                            Value = item.CityName,
                            Text = item.CityName
                        });
                    }
                }
            }
            catch (HttpException ex)
            {
                throw new HttpException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Json(new SelectList(list, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetJsonCountry()
        {
            var list = new List<DropDownListViewModel>();
            try
            {
                var counytryList = _cityRepository.GetAllElements().Select(x => new { x.CountryName }).OrderBy(x => x.CountryName).ToList().Distinct().ToList();

                if (!object.Equals(counytryList, null))
                {
                    var countries = counytryList.ToList();
                    foreach (var item in countries)
                    {
                        list.Add(new DropDownListViewModel
                        {
                            Value = item.CountryName,
                            Text = item.CountryName
                        });
                    }
                }
            }
            catch (HttpException ex)
            {
                throw new HttpException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Json(new SelectList(list, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetJsonIndustry()
        {
            var list = new List<DropDownListViewModel>();
            try
            {
                IList<Industry> IndustryList = _industryRepository.GetAllElements().OrderBy(x => x.IndustryName).ToList();

                if (!object.Equals(IndustryList, null))
                {
                    var industries = IndustryList.ToList();
                    foreach (var item in industries)
                    {
                        list.Add(new DropDownListViewModel
                        {
                            Value = item.IndustryName,
                            Text = item.IndustryName
                        });
                    }
                }
            }
            catch (HttpException ex)
            {
                throw new HttpException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return Json(new SelectList(list, "Value", "Text"), JsonRequestBehavior.AllowGet);
        }

    }
        
}