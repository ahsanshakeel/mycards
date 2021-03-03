using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyCard.Web.Models;
using System.Net.Mail;
using MyCard.Domain;
using System.Threading.Tasks;
using MyCard.Helper;
using System.Web.Security;
using System.Linq.Expressions;
using System.Threading;
using System.IO;
using System.Web.Hosting;
using MyCard.Web.Attributes;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using CaptchaMvc.HtmlHelpers;

namespace MyCard.Web.Controllers
{
    [ErrorHandlerAttribute]
    public class RegistrationController : Controller
    {
        private ICompanyRepository _companyRepository;
        private IUserRepository _userRepository;
        private IInquiryRepository _inquiryRepository;
        private IEmailTemplateRepository _emailTemplateRepository;
        private ICityRepository _cityRepository;
        private IIndustryRepository _industryRepository;
        public RegistrationController(ICompanyRepository companyRepository, IUserRepository userRepository, 
            IInquiryRepository inquiryRepository, ICityRepository cityRepository, IIndustryRepository industryRepository,
            IEmailTemplateRepository emailTemplateRepository)
        {
            if (companyRepository == (ICompanyRepository)null)
                throw new ArgumentNullException("companyRepository");
            _companyRepository = companyRepository;

            if (userRepository == (IUserRepository)null)
                throw new ArgumentNullException("userRepository");
            _userRepository = userRepository;

            if (inquiryRepository == (IInquiryRepository)null)
                throw new ArgumentNullException("inquiryRepository");
            _inquiryRepository = inquiryRepository;

            if (emailTemplateRepository == (IEmailTemplateRepository)null)
                throw new ArgumentNullException("emailTemplateRepository");
            _emailTemplateRepository = emailTemplateRepository;

            if (cityRepository == (ICityRepository)null)
                throw new ArgumentNullException("cityRepository");
            _cityRepository = cityRepository;

            if (industryRepository == (IIndustryRepository)null)
                throw new ArgumentNullException("industryRepository");
            _industryRepository = industryRepository;

        }
        // GET: Registration
        public ActionResult Index()
        {
            //return RedirectToAction("RegistrationForm");
            ViewBag.PageType = "Register";
            return View();
        }

        public ActionResult GetCaptcha()
        {
            return View("Captcha");
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegistrationViewModel model, string mode)
        {
            try
            {

                ViewBag.PageType = "Register";

            
            //if(System.Configuration.ConfigurationManager.AppSettings["domains2bar"].ToString().Contains (model.EmailID.Split('@')[1]))
            //{
            //    return Json(new { success = false, responseText = "Please use a corporate email account to register" });
            //}

            bool bcaptcha = this.IsCaptchaValid("Validate your captcha");

            if (!string.IsNullOrEmpty(mode))
            {
                bcaptcha = true;
            }

            if (ModelState.IsValid && bcaptcha)
            {

                int existingCompanyCount = 0;
                string searchString = string.Empty;
                string emailBody = string.Empty;
                string emailSubject = string.Empty;
                EmailTemplate emailTemplate = null;
                StringBuilder sbEmail;

                MailAddress emailAddress = new MailAddress(model.EmailID);

                string emailUserName = emailAddress.User;
                string emailDomain = emailAddress.Host;

                if (model.DomainName == "mydomain.com")
                {
                    model.DomainName = emailDomain;
                }

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
                    emailBody = string.Empty;
                    emailSubject = string.Empty;
                    emailTemplate = null;
                    //emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.Name == "Not Corporate Account").FirstOrDefault();
                    emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 6).FirstOrDefault();


                    if (emailTemplate!=null)
                    {
                        emailBody = emailTemplate.Template;
                        emailSubject = emailTemplate.Subject;
                    }
                    sbEmail = new StringBuilder(emailBody);
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
                    company.AccountType = model.AccountType;
                    company.CmsAccessKey = "";
                    company.CpPassword = "";
                    company.PassResetCode = "";

                    User user = new User();
                    user.Name = model.CustomerName;
                    user.Email = model.EmailID;
                    user.Active = 1;
                    user.OutlookProfilePic = "";
                    user.RememberToken = "";

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


                    
                    //emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.Name == "Registration").FirstOrDefault();
                    if(model.AccountType == 2)
                    {
                        emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 10).FirstOrDefault();
                    }
                    else
                    {
                        emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 1).FirstOrDefault();
                    }
                    //emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 1).FirstOrDefault();


                    if (emailTemplate!=null)
                    {
                        emailBody = emailTemplate.Template;
                        emailSubject = emailTemplate.Subject;
                    }
                    sbEmail = new StringBuilder(emailBody);
                    sbEmail.Replace("[CompanyName]", model.Name);

                    if (model.AccountType == 2)
                    {
                        string sLink = "<a href ='" + domainName + "/CompanyDashboard/CreatePassword/?param=" + EncodedCompanyID + "'>" + domainName + "/CompanyDashboard/CreatePassword/?param=" + EncodedCompanyID + "</a>";
                        sbEmail.Replace("[CreatePasswordLink]", sLink);
                    }


                    if(model.CustomerType=="Freemium")
                    {
                        return Json(new { success = true, responseText = MyCard.Web.Resources.CaptionsAll.RegisterSuccessMessageFree1 }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
                        {
                            MailHelper mail = new MailHelper();
                            string subject = emailSubject;// MyCard.Web.Resources.CaptionsAll.EmailPasswordCreationSubjectMSO;
                                                          //string body = String.Format(@Resources.CaptionsAll.PasswordCreationEmailBody, model.Name, domainName, EncodedCompanyID);
                            string body = sbEmail.ToString();//String.Format(@Resources.CaptionsAll.PasswordCreationEmailBodyMSO, model.Name, domainName);
                            //await mail.SendAsync(model.EmailID, subject, body, String.Empty);
                            await mail.SendAsyncBCC(model.EmailID, subject, body, String.Empty);

                        });

                        //  Send "Success"
                        return Json(new { success = true, responseText = MyCard.Web.Resources.CaptionsAll.RegistrationThankyouMessage }, JsonRequestBehavior.AllowGet);
                    }

                    
                    //return RedirectToAction("RegistrationFormThanks", "Registration");

            }
            else
            {
                return Json(new { success = false, responseText = "CAPTCHA verification failed." });
            }
            }
            catch (Exception ex)
            {
                //  Send "false"
                //return Json(new { success = false, responseText = ex.Message });
                return Json(new { success = false, responseText = ex.StackTrace + ":" + ex.Message });

                //ViewBag.ErrorMessage = ex.Message;
                //return View();
            }

        }
        [HttpPost]
        public JsonResult Send_Email(string Name, string Email, string CompanyName)
        {
            string emailBody = string.Empty;
            string emailSubject = string.Empty;
            MailAddress emailAddress = new MailAddress(Email);
            emailBody = "Hello and welcome! in a myCards™ Enterprise. Here is Bank 2 Bank Information" + "<br/><br/> Name  :" + Name + "<br/><br/>Comapnr Name :" + CompanyName + "<br/><br/> Email:" + Email;
            emailSubject = "Customer Company Information";
            HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
            {
                MailHelper mail = new MailHelper();
                string subject = emailSubject;
                string body = emailBody;
                await mail.SendAsyncBCC(Email, subject, body, String.Empty);

            });
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        // GET: Inquiry
        public async Task<ActionResult> Inquiry(InquiryViewModel model)
        {
            try
            {
                
                Inquiry inquiry = new Inquiry();

                inquiry.BusinessName = model.BusinessName;
                inquiry.CustomerName = model.CustomerName;
                inquiry.Email = model.Email;
                _inquiryRepository.Add(inquiry);


                await _inquiryRepository.UnitOfWork.CommitAsync();

                //Email Template
                string emailBody = string.Empty;
                string emailSubject = string.Empty;
                EmailTemplate emailTemplate = null;
                //emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.Name == "NOT using Office 365").FirstOrDefault();
                emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 5).FirstOrDefault();


                if (emailTemplate != null)
                {
                    emailBody = emailTemplate.Template;
                    emailSubject = emailTemplate.Subject;
                }
                StringBuilder sbEmail = new StringBuilder(emailBody);
                sbEmail.Replace("[CustomerName]", model.CustomerName);

                HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
                {
                    MailHelper mail = new MailHelper();
                    string subject = emailSubject;// MyCard.Web.Resources.CaptionsAll.EmailPasswordCreationSubjectMSO;
                    string body = sbEmail.ToString();//String.Format(@Resources.CaptionsAll.PasswordCreationEmailBodyMSO, model.Name, domainName);
                    await mail.SendAsync(model.Email, subject, body, String.Empty);

                });


                return Json(new { success = true, responseText = MyCard.Web.Resources.CaptionsAll.RegistrationThankyouMessage }, JsonRequestBehavior.AllowGet);
                

            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.Message });
                
            }
        }

        // GET: RegistrationFormThankscocontr
        public ActionResult RegistrationFormThanks()
        {
            ViewBag.FirstName = TempData["FirstName"];

            return View();
        }

        //Logout
        //[HttpPost]

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

                IList<City> cityList = _cityRepository.GetFilteredElements(x => x.CountryName == countryName).OrderBy(x=>x.CityName).ToList();

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