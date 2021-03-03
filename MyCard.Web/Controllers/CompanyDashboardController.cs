using MyCard.Web.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using MyCard.Domain;
using MyCard.Web.Models;
using MyCard.BoundedContext;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;
using System.Web.Hosting;
using MyCard.Helper;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System.Web.UI;
using System.Text;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using MyCard.Web.Security;
using System.Security.Claims;
using MyCard.Web.TokenStorage;
using Microsoft.Owin.Security.Cookies;
using System.Net.Http;
using System.Security.Cryptography;
using CaptchaMvc.HtmlHelpers;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using MySql.Data.MySqlClient;

namespace MyCard.Web.Controllers
{
    [ErrorHandlerAttribute]
    [Authorize(Roles = "company_owner")]
    public class CompanyDashboardController : Controller
    {
        private ILicenseTypeRepository _licenseTypeRepository; 
        private IUserRepository _userRepository;
        private IPurchaseOrderRepository _purchaseOrderRepository;
        private IPaytabRepository _paytabRepository;
        private ICompanyRepository _companyRepository;
        private IContactShareActivityRepository _contactShareActivityRepository;
        private IEmailTemplateRepository _emailTemplateRepository;
        private IMyCardFileRepository _myCardFileRepository;
        private IPasswordHistoryRepository _passwordHistoryRepository;

        public CompanyDashboardController(ILicenseTypeRepository licenseTypeRepository, IUserRepository userRepository, IPurchaseOrderRepository purchaseOrderRepository, IPaytabRepository paytabRepository,
            ICompanyRepository companyRepository, IContactShareActivityRepository contactShareActivityRepository, 
            IEmailTemplateRepository emailTemplateRepository, IMyCardFileRepository myCardFileRepository, IPasswordHistoryRepository passwordHistoryRepository)
        {
            if (companyRepository == (ICompanyRepository)null)
                throw new ArgumentNullException("companyRepository");
            _companyRepository = companyRepository;

            if (purchaseOrderRepository == (IPurchaseOrderRepository)null)
                throw new ArgumentNullException("purchaseOrderRepository");
            _purchaseOrderRepository = purchaseOrderRepository;

            if (paytabRepository == (IPaytabRepository)null)
                throw new ArgumentNullException("paytabRepository");
            _paytabRepository = paytabRepository;

            if (userRepository == (IUserRepository)null)
                throw new ArgumentNullException("userRepository");
            _userRepository = userRepository;

            if (licenseTypeRepository == (ILicenseTypeRepository)null)
                throw new ArgumentNullException("licenseTypeRepository");
            _licenseTypeRepository = licenseTypeRepository;

            if (contactShareActivityRepository == (IContactShareActivityRepository)null)
                throw new ArgumentNullException("contactShareActivityRepository");
            _contactShareActivityRepository = contactShareActivityRepository;

            if (emailTemplateRepository == (IEmailTemplateRepository)null)
                throw new ArgumentNullException("emailTemplateRepository");
            _emailTemplateRepository = emailTemplateRepository;

            if (myCardFileRepository == (IMyCardFileRepository)null)
                throw new ArgumentNullException("myCardFileRepository");
            _myCardFileRepository = myCardFileRepository;

            if (passwordHistoryRepository == (IPasswordHistoryRepository)null)
                throw new ArgumentNullException("passwordHistoryRepository");
            _passwordHistoryRepository = passwordHistoryRepository;
        }
        // GET: Dashboard
        public async Task<ActionResult> Index()
        {
            int userId = 0;
            int companyId = 0;
            string companyName = string.Empty;
            IList<int> userlist = null;
            int cardSharesCount = 0;
            string approved = string.Empty;

            CompanyDashboardViewModel companyViewModel = new CompanyDashboardViewModel();

            if (HttpContext.Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (userId > 0)
            {
                User user = new User();
                Company company = new Company();
                user = _userRepository.GetElementById(userId);

                if (user != null)
                {
                    companyId = user.CompanyId;
                                        
                    company = _companyRepository.GetFilteredElements(o => o.Id == companyId).FirstOrDefault();
                    companyName = company.Name;

                    companyViewModel.CustomerName = company.CustomerName;
                    companyViewModel.Name = company.Name;
                    companyViewModel.DomainName = company.Domain;
                    companyViewModel.EmailID = company.Email;
                    companyViewModel.BusinessTitle = company.BusinessTitle;
                    companyViewModel.MemberSince = company.CreationDate.ToString("dd MMMM yyyy");
                    ViewBag.FileName = company.LogoFileName;
                    ViewBag.CustomerType = company.CustomerType;
                    approved = company.Approved=="Yes" ? "Yes": "No";
                }

                userlist = _userRepository.GetFilteredElements(o => o.CompanyId == companyId).Select(t => t.Id).ToList();

                cardSharesCount = _contactShareActivityRepository.GetFilteredElements(o => userlist.Contains(o.SenderId)).Count();
                companyViewModel.TotalCardsShared = cardSharesCount;


            }

            PurchaseOrder po;
            po = _purchaseOrderRepository.GetFilteredElements(o => o.CompanyId == companyId).OrderByDescending(o => o.CreationDate).FirstOrDefault();
            if (po!=null)
            {
                companyViewModel.LastPurchaseDate = po.CreationDate.ToString("dd MMMM yyyy");
            }
            else
            {
                companyViewModel.LastPurchaseDate = string.Empty;
            }
            
            var poTotalAmount = _purchaseOrderRepository.GetFilteredElements(o => o.CompanyId == companyId && o.Approved==1).Sum(o => o.LicensesOrdered);
            companyViewModel.TotalPurchase = poTotalAmount;
            ViewBag.TotalPurchase = poTotalAmount;

            ViewBag.Designer = "No";

            PurchaseOrder poDesigner = _purchaseOrderRepository.GetFilteredElements(x => x.CompanyId == companyId && x.Approved == 1).FirstOrDefault();
            if (poDesigner != null)
            {
                ViewBag.Designer = "Yes";
            }
            else
            {
                if(approved=="Yes")
                {
                    ViewBag.Designer = "Yes";
                }
            }

            PurchaseOrder poAddLicense = _purchaseOrderRepository.GetFilteredElements(x => x.CompanyId == companyId).FirstOrDefault();
            if (poAddLicense != null)
            {
                ViewBag.AddLicense = "Yes";
            }

            //For Links to be white or dull
            PurchaseOrder poLatestPurchase = _purchaseOrderRepository.GetFilteredElements(x => x.CompanyId == companyId).OrderByDescending(o => o.Id).FirstOrDefault();
            if (poLatestPurchase != null)
            {
                if (poLatestPurchase.Approved == 1)
                    ViewBag.LatestPurchasedApproved = "Yes";
                else
                    ViewBag.LatestPurchasedApproved = "No";
            }
            else
            {
                ViewBag.LatestPurchasedApproved = "No";
            }

            ViewBag.CompanyName = companyName;

            ViewBag.myCardCMS = "No";
            
            Company companyCMS = await _companyRepository.GetFirstOrDefaultAsync(x => x.Id == companyId && (x.AccountType == 2 || x.AccountType == 3));
            if (companyCMS != null)
            {
                ViewBag.myCardCMS = "Yes";
                
            }


            


            return View(companyViewModel);
        }

        public ActionResult CardShareChart(int year)
        {
            //Card Shares Chart Data
            User user = null;
            IList<int> userlist = null;
            int userId = 0;
            int companyId = 0;
            IList<ContactShareActivity> cardShares = null;
            List<int> cardShareCount = new List<int>();
            if (HttpContext.Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (userId > 0)
            {
                user = _userRepository.GetElementById(userId);

                if (user != null)
                {
                    companyId = user.CompanyId;
                }
                userlist = _userRepository.GetFilteredElements(o => o.CompanyId == companyId).Select(t=>t.Id).ToList();

                cardShares = _contactShareActivityRepository.GetFilteredElements(o => o.CreationDate.Year == year && userlist.Contains(o.SenderId)).ToList();

                for (int i = 1; i <= 12; i++)
                {
                    cardShareCount.Add(cardShares.Count(o => o.CreationDate.Month == i));
                }
            }

            return Json(new { datalist = cardShareCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CompaniesChart(int year)
        {
            MyCardUnitOfWork db = new MyCardUnitOfWork();

            int userId = 0;
            int companyId = 0;
            IList<int> userlist = null;
            IList<ContactShareActivity> cardShares = null;
            List<int> companyCount = new List<int>();
            if (HttpContext.Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (userId > 0)
            {
                User user = new User();
                Company company = new Company();
                user = _userRepository.GetElementById(userId);

                if (user != null)
                {
                    companyId = user.CompanyId;
                }

                userlist = _userRepository.GetFilteredElements(o => o.CompanyId == companyId).Select(t => t.Id).ToList();

                cardShares = _contactShareActivityRepository.GetFilteredElements(o => o.CreationDate.Year == year && userlist.Contains(o.SenderId))
                    .OrderBy(t=>t.CreationDate).ToList();
                
                for (int i = 1; i <= 12; i++)
                {
                    List<ContactShareActivity> monthCardShares = cardShares.Where(o => o.CreationDate.Month == i).ToList();
                    List<int> companyIds = new List<int>();
                    monthCardShares.ForEach(monthCardShare =>
                    {
                        User recUser = null;
                        recUser = _userRepository.GetElementById(monthCardShare.ReceiverId);

                        if (recUser!=null)
                        {
                            int sharedCompanyId = recUser.CompanyId;
                            if (!companyIds.Contains(sharedCompanyId))
                            {
                                companyIds.Add(sharedCompanyId);
                            }
                        }

                        
                    });
                    companyCount.Add(companyIds.Count);
                }
            }
            
            return Json(new { datalist = companyCount }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CompanyYears()
        {
            int userId = 0;
            int companyId = 0;
            int companyCreationYear = 0;


            if (HttpContext.Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (userId > 0)
            {
                User user = new User();
                
                user = _userRepository.GetElementById(userId);

                if (user != null)
                {
                    companyId = user.CompanyId;
                    Company company = new Company();
                    company = _companyRepository.GetElementById(companyId);
                    companyCreationYear = company.CreationDate.Year;
                }
            }
            

            IList<int> years = new List<int>();
            for (int i= DateTime.UtcNow.Year; i>= companyCreationYear; i--)
            {
                years.Add(i);
            }
            return Json(years, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SharedCardCount(int year)
        {
            //Card Shares Chart Data
            User user = null;
            IList<int> userlist = null;
            int userId = 0;
            int companyId = 0;
            int cardSharesCount = 0;

            if (HttpContext.Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (userId > 0)
            {
                user = _userRepository.GetElementById(userId);

                if (user != null)
                {
                    companyId = user.CompanyId;
                }
                userlist = _userRepository.GetFilteredElements(o => o.CompanyId == companyId).Select(t => t.Id).ToList();

                cardSharesCount = _contactShareActivityRepository.GetFilteredElements(o => o.CreationDate.Year == year && userlist.Contains(o.SenderId)).Count();
            }

            return Json(cardSharesCount, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CompaniesCount(int year)
        {
            //Card Shares Chart Data
            User user = null;
            IList<int> userlist = null;
            int userId = 0;
            int companyId = 0;
            int companiestotal = 0;
            //int cardSharesCount = 0;

            if (HttpContext.Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (userId > 0)
            {
                user = _userRepository.GetElementById(userId);

                if (user != null)
                {
                    companyId = user.CompanyId;
                }
                userlist = _userRepository.GetFilteredElements(o => o.CompanyId == companyId).Select(t => t.Id).ToList();

                List<ContactShareActivity> cardSharesCount = _contactShareActivityRepository.GetFilteredElements(o => o.CreationDate.Year == year && userlist.Contains(o.SenderId)).ToList();

                List<int> companyIds = new List<int>();
                foreach(ContactShareActivity item in cardSharesCount)
                {
                    User recUser = null;

                    recUser = _userRepository.GetElementById(item.ReceiverId);
                    if (recUser!=null)
                    {
                        int sharedCompanyId = recUser.CompanyId;

                        if (!companyIds.Contains(sharedCompanyId))
                        {
                            companyIds.Add(sharedCompanyId);
                        }
                    }
                    
                }

                companiestotal = companyIds.Count();

            }

            return Json(companiestotal, JsonRequestBehavior.AllowGet);

        }
        
        // GET: Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string emailAddress)
        {
            //var href = Request.Url.Authority;
            //string domain;
            //string n = HttpUtility.UrlDecode("D2Y%2bH1%2fTJqW%2b80tOAAEm3A%3d%3d");
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////WHAT IS THE PURPOSE OF THESE TWO LINES BELOW///////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            string y = "MHrz6Gk3lAa0PsxexTspAA==";
            string m = MyCard.Helper.EncryptionHelper.DecryptString(y);
            
            if (!string.IsNullOrEmpty(emailAddress))
            {
                ViewBag.ActivationLinkMessage = MyCard.Web.Resources.CaptionsAll.AcivationLinkMessage1;
            }
            else
            {
                ViewBag.ActivationLinkMessage = string.Empty;
            }


            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        public ActionResult LoginPremise(string returnUrl, string emailAddress)
        {
            //var href = Request.Url.Authority;
            //string domain;
            //string n = HttpUtility.UrlDecode("D2Y%2bH1%2fTJqW%2b80tOAAEm3A%3d%3d");
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////WHAT IS THE PURPOSE OF THESE TWO LINES BELOW///////////////////////////////////////////
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            string y = "MHrz6Gk3lAa0PsxexTspAA==";
            string m = MyCard.Helper.EncryptionHelper.DecryptString(y);

            if (!string.IsNullOrEmpty(emailAddress))
            {
                ViewBag.ActivationLinkMessage = MyCard.Web.Resources.CaptionsAll.AcivationLinkMessage1;
            }
            else
            {
                ViewBag.ActivationLinkMessage = string.Empty;
            }


            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //Randomn Key Generation
        private string GenerateRandomKey(int length)
        {
            const string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var res = new StringBuilder(length);
            using (var rng = new RNGCryptoServiceProvider())
            {
                int count = (int)Math.Ceiling(Math.Log(alphabet.Length, 2) / 8.0);
                //Debug.Assert(count <= sizeof(uint));
                int offset = BitConverter.IsLittleEndian ? 0 : sizeof(uint) - count;
                int max = (int)(Math.Pow(2, count * 8) / alphabet.Length) * alphabet.Length;
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (res.Length < length)
                {
                    rng.GetBytes(uintBuffer, offset, count);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    if (num < max)
                    {
                        res.Append(alphabet[(int)(num % alphabet.Length)]);
                    }
                }
            }

            return res.ToString();
        }


        public string GenerateRandomCryptographicKey(int keyLength)
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }


        // POST: Login
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> LoginPremise(LoginViewModel model, string returnUrl)
        {
            string DecrptedPassword = string.Empty;
            string rolename = string.Empty;

            string encryptedCompanyID = string.Empty;
            string encodedCompanyID = string.Empty;
            //GenerateRandomCryptographicKey cmsKey
            string cmsKey = string.Empty;
            cmsKey = GenerateRandomKey(12);


            if (ModelState.IsValid)
            {
                try
                {
                    Expression<Func<User, object>>[] includes =
                    {
                        x => x.UserRoles
                    };

                    DecrptedPassword = MyCard.Helper.EncryptionHelper.EncryptString(model.Password);
                    User user = new User();
                    user = await _userRepository.GetFirstOrDefaultAsync(o => o.Email == model.UserName, default(CancellationToken), includes);


                    encryptedCompanyID = MyCard.Helper.EncryptionHelper.EncryptString(Convert.ToString(user.CompanyId));
                    encodedCompanyID = HttpUtility.UrlEncode(encryptedCompanyID);

                    foreach (var item in user.UserRoles)
                    {
                        if (item.RoleId == 1)
                        {
                            rolename = "super_admin";
                        }
                        else if (item.RoleId == 2)
                        {
                            rolename = "company_owner";
                        }
                        else
                        {
                            rolename = "user";
                        }
                    }

                    if(rolename == "company_owner")
                    {
                        if (user.Password == DecrptedPassword)
                        {

                            #region check failed attemps

                            List<FailedUser> listFailures = (List<FailedUser>)this.HttpContext.Application["FailedUser"];
                            if (listFailures != null)
                            {
                                try
                                {
                                    FailedUser faileduser = listFailures.Find(k => k.UserId == model.UserName);

                                    bool bValid = this.IsCaptchaValid("Validate your captcha");
                                    if (faileduser != null && faileduser.LastAccessTime > DateTime.Now.AddMinutes(-60) && faileduser.Count > 2 && !bValid)
                                    {
                                        ViewBag.Message = "Your account is temporarily locked. Please try again in an hour.";
                                        return View();
                                    }

                                    if (bValid)
                                    {
                                        listFailures.Remove(faileduser);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string s = ex.Message;
                                }
                            }

                            #endregion
                            
                            #region already logged in checking
                            List<LoggedUser> listUsers = (List<LoggedUser>)this.HttpContext.Application["LoggedUser"];
                            if (listUsers == null)
                            {
                                listUsers = new List<LoggedUser>();
                            }

                            string UserId = user.Id.ToString();
                            var loggeduser = listUsers.Find(k => k.UserId == UserId);
                            if (loggeduser == null)
                            {
                                loggeduser = new LoggedUser() { UserId = UserId, LastAccessTime = DateTime.Now, CMSKey = cmsKey };
                                listUsers.Add(loggeduser);
                                this.HttpContext.Application["LoggedUser"] = listUsers;
                            }
                            else
                            {
                                loggeduser.LastAccessTime = DateTime.Now;
                                loggeduser.CMSKey = cmsKey;
                            }
                            #endregion
                                                        
                            //FormsAuthentication.SetAuthCookie(user.Name + "|" + user.Id, false);

                            var authTicket = new FormsAuthenticationTicket(
                                                      1,
                                                      user.Name + "|" + user.Id + "|" + user.CompanyId + "|" + encodedCompanyID + "|" + cmsKey ,  //user id
                                                      DateTime.Now,
                                                      DateTime.Now.AddMinutes(15),  // expiry
                                                      model.RememberMe,  //true to remember
                                                      rolename, //roles 
                                                      "/"
                                                    );
                            string encTicket = FormsAuthentication.Encrypt(authTicket);

                            //Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket) { Expires = authTicket.Expiration });
                            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket) { Expires = authTicket.Expiration, Secure = true });
                            //Update Cms_key here

                            Company company = null;
                            company = await _companyRepository.GetFirstOrDefaultAsync(o => o.Id == user.CompanyId);

                            company.CmsAccessKey = cmsKey;


                            _companyRepository.SetModified(company);
                            await _companyRepository.UnitOfWork.CommitAsync();

                            //Check if there is latest transaction is unfinished
                            //ViewBag.Designer = "No";
                            PurchaseOrder poUpload = _purchaseOrderRepository.GetFilteredElements(x => x.CompanyId == user.CompanyId).OrderByDescending(x => x.Id).FirstOrDefault();

                            if (poUpload != null)
                            {
                                if(poUpload.OrderFile==null || poUpload.FileName==null)
                                {
                                    return RedirectToAction("Purchase", "CompanyDashboard");
                                }
                            }
                            else
                            {
                                return RedirectToAction("Purchase", "CompanyDashboard");
                            }
                            //


                            if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                                && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "CompanyDashboard");
                            }


                        }
                        else
                        {
                            ViewBag.Message = MyCard.Web.Resources.ErrorMessages.UserNamePasswordNotMatch;
                            UpdateFailAttempts(model.UserName);

                            #region check failed attemps

                            List<FailedUser> listFailures = (List<FailedUser>)this.HttpContext.Application["FailedUser"];
                            if (listFailures != null)
                            {
                                FailedUser faileduser = listFailures.Find(k => k.UserId == model.UserName);

                                if (faileduser != null && faileduser.LastAccessTime > DateTime.Now.AddMinutes(-60) && faileduser.Count > 2)
                                {
                                    ViewBag.Message = "Your account is temporarily locked. Please try again in an hour.";
                                    return View();
                                }
                            }

                            #endregion

                        }
                        return View();
                    }
                    else
                    {
                        ViewBag.Message = MyCard.Web.Resources.ErrorMessages.UserNamePasswordNotMatch;
                        return View();
                    }
                    
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

        private void UpdateFailAttempts(string UserName)
        {
            List<FailedUser> listUsers = (List<FailedUser>)this.HttpContext.Application["FailedUser"];
            if (listUsers == null)
            {
                listUsers = new List<FailedUser>();
            }

            string UserId = UserName;
            var faileduser = listUsers.Find(k => k.UserId == UserId);
            if (faileduser == null)
            {
                faileduser = new FailedUser() { UserId = UserId, LastAccessTime = DateTime.Now, Count =1 };
                listUsers.Add(faileduser);
                this.HttpContext.Application["FailedUser"] = listUsers;
            }
            else
            {
                if (faileduser.LastAccessTime < DateTime.Now.AddMinutes(-60))
                {
                    faileduser.LastAccessTime = DateTime.Now;
                    faileduser.Count = 1;
                }
                else
                {
                    faileduser.LastAccessTime = DateTime.Now;
                    faileduser.Count += 1;
                }                
            }

        }

        //// GET: ChangePassword
        //public ActionResult ChangePassword()
        //{
        //    return View();
        //}

        //// POST: ChangePassword
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        //{
        //    string EncryptedPassword = string.Empty;
        //    string EncryptedOldPassword = string.Empty;

        //    EncryptedOldPassword = MyCard.Helper.EncryptionHelper.EncryptString(model.OldPassword);
        //    ViewBag.Message = string.Empty;

        //    int UserID = 0;
        //    //Get User ID From Cookie
        //    if (HttpContext.Request.IsAuthenticated)
        //    {
        //        UserID = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        User user = new User();
        //        user = await _userRepository.GetFirstOrDefaultAsync(o => o.Id == UserID);

        //        if (user != null)
        //        {
        //            if (user.Password == EncryptedOldPassword)
        //            {
        //                EncryptedPassword = MyCard.Helper.EncryptionHelper.EncryptString(model.Password);
        //                user.Password = EncryptedPassword;
        //                _userRepository.SetModified(user);

        //                await _userRepository.UnitOfWork.CommitAsync();
        //            }
        //            else
        //            {
        //                ViewBag.Message = MyCard.Web.Resources.ErrorMessages.OldPasswordNotCorrect;
        //                return View();
        //            }
        //        }

        //        return RedirectToAction("ChangePasswordThanks");
        //    }
        //    return View();
        //}

        //// GET: Admin ChangePasswordThanks
        //public ActionResult ChangePasswordThanks()
        //{
        //    return View();
        //}

        // GET: CreatePassword
        [AllowAnonymous]
        public async Task<ActionResult> CreatePassword(string param)
        {
            int CompanyID = 0;
            string decrpytedParam = string.Empty;
            DateTime linkGeneratedAt = DateTime.UtcNow;
            //string DecodedCompanyID;
            //DecodedCompanyID = HttpUtility.UrlDecode(param);
            if (string.IsNullOrEmpty(param))
            {
                return View();
            }
            else
            {
                decrpytedParam = MyCard.Helper.EncryptionHelper.DecryptString(param);
                CompanyID = Convert.ToInt32(decrpytedParam.Split('|')[0]);
                linkGeneratedAt = Convert.ToDateTime(decrpytedParam.Split('|')[1]).ToUniversalTime();

                ///
                User user = new User();
                user = await _userRepository.GetFirstOrDefaultAsync(o => o.CompanyId == CompanyID);

                if (String.IsNullOrEmpty(user.Password) && linkGeneratedAt.AddHours(24) >= DateTime.UtcNow)
                {
                    /////
                    TempData["CompanyID"] = CompanyID;
                    return View();
                }
                else
                {
                    return RedirectToAction("Login");
                }


            }

        }

        // POST: CreatePassword
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> CreatePassword(CreatePasswordViewModel model)
        {
            int CompanyID = 0;
            CompanyID = Convert.ToInt32(TempData["CompanyID"]);
            string EncryptedPassword = string.Empty;

            if (ModelState.IsValid)
            {
                User user = new User();
                user = await _userRepository.GetFirstOrDefaultAsync(o => o.CompanyId == CompanyID);

                EncryptedPassword = MyCard.Helper.EncryptionHelper.EncryptString(model.Password);
                user.Password = EncryptedPassword;
                _userRepository.SetModified(user);

                await _userRepository.UnitOfWork.CommitAsync();

                return RedirectToAction("Login");
            }
            return View();
        }
        // GET: CreatePassword Exit
        [AllowAnonymous]
        public ActionResult CreatePasswordExit()
        {
            return View();
        }

        // GET: CreatePassword Thanks
        [AllowAnonymous]
        public ActionResult CreatePasswordThanks()
        {
            return View();
        }


        public async Task<ActionResult> Logout()
        {
            //For MSO365 Login
            //if (Request.IsAuthenticated)
            //{
            //    // Get the user's token cache and clear it.
            //    string userObjectId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;

            //    SessionTokenCache tokenCache = new SessionTokenCache(userObjectId, HttpContext);
            //    HttpContext.GetOwinContext().Authentication.SignOut(OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
            //}
            int accountType = 0;
            Company company = null;
            string userName = "";
            if(User != null)
            {
                int companyId = Convert.ToInt32(User.Identity.Name.Split('|')[2]);
                userName = User.Identity.Name.Split('|')[1];
                company = await _companyRepository.GetFirstOrDefaultAsync(o => o.Id == companyId);
            }
            

            if (company != null)
            {
                accountType = company.AccountType;
                company.CmsAccessKey = null;

                _companyRepository.SetModified(company);
                await _companyRepository.UnitOfWork.CommitAsync();
            }
            

            
            
            if (accountType == 2)
            {
                System.Web.HttpContext.Current.Session.Clear();
                System.Web.HttpContext.Current.Session.Abandon();
                FormsAuthentication.SignOut();
            }
                

            ///

            //if (accountType == 1 || accountType == 3)
            else 
            {
                if (Request.IsAuthenticated)
                {
                    try
                    {                     //Get the user's token cache and clear it.
                        string userObjectId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;

                        SessionTokenCache tokenCache = new SessionTokenCache(userObjectId, HttpContext);
                        HttpContext.GetOwinContext().Authentication.SignOut(OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
                    }
                    catch { }

                }

                // Send an OpenID Connect sign-out request. 
                //HttpContext.GetOwinContext().Authentication.SignOut(
                //  CookieAuthenticationDefaults.AuthenticationType);
                ///End

                System.Web.HttpContext.Current.Session.Clear();
                System.Web.HttpContext.Current.Session.Abandon();
                FormsAuthentication.SignOut();
            }

            #region already logged in checking

            List<LoggedUser> listUsers = (List<LoggedUser>)this.HttpContext.Application["LoggedUser"];
            if (listUsers != null)
            {
                LoggedUser loggeduser = listUsers.Find(k => k.UserId == userName);
                if (loggeduser != null)
                {
                    listUsers.Remove(loggeduser);
                    //loggeduser.LastAccessTime = DateTime.Now.AddMinutes(-30);
                }
            }

            #endregion


            ///
            return RedirectToAction("Login", "CompanyDashboard");
        }

        //// GET: ResendLink
        //[AllowAnonymous]
        //public ActionResult ResendLink()
        //{
        //    return View();
        //}

        //// POST: ResendLink
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[AllowAnonymous]
        //public async Task<ActionResult> ResendLink(ResendLinkViewModel model)
        //{
        //    ViewBag.Message = string.Empty;
        //    string EncryptedCompanyID = string.Empty;
        //    string EncodedCompanyID = string.Empty;
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            User user = null;
        //            string domainName = string.Empty;
        //            string companyName = string.Empty;
        //            Uri url = HttpContext.Request.Url;
        //            domainName = url.AbsoluteUri.Replace(url.PathAndQuery, string.Empty);

        //            user = await _userRepository.GetFirstOrDefaultAsync(o => o.Email == model.Email);
        //            if (user != null)
        //            {
        //                if(!string.IsNullOrEmpty(user.Password))
        //                {
        //                    ViewBag.ErrorMessageAlreadyActivated = MyCard.Web.Resources.ErrorMessages.AlreadyActivated;
        //                    return View();
        //                }

        //                //EncryptedUserID = MyCard.Helper.EncryptionHelper.EncryptString(Convert.ToString(user.Id ));
        //                EncryptedCompanyID = MyCard.Helper.EncryptionHelper.EncryptString(Convert.ToString(user.CompanyId + "|" + System.DateTime.UtcNow));
        //                EncodedCompanyID = HttpUtility.UrlEncode(EncryptedCompanyID);

        //                Company company = null;
        //                company = _companyRepository.GetElementById(user.CompanyId);
        //                if (company != null)
        //                {
        //                    companyName = company.Name;
        //                }

        //                if (!String.IsNullOrEmpty(company.Email))
        //                {
        //                    HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
        //                    {
        //                        MailHelper mail = new MailHelper();
        //                        string subject = MyCard.Web.Resources.CaptionsAll.EmailPasswordCreationSubject;
        //                        string body = String.Format(@Resources.CaptionsAll.PasswordCreationEmailBody, companyName, domainName, EncodedCompanyID);
        //                        await mail.SendAsync(company.Email, subject, body, String.Empty);

        //                    });
        //                    ViewBag.Message = MyCard.Web.Resources.ErrorMessages.EmailSentSuccess;
        //                }

        //            }
        //            else
        //            {
        //                ViewBag.ErrorMessage = MyCard.Web.Resources.ErrorMessages.EmailNotExistErrorMessage;
        //            }
        //        }

        //        catch
        //        {
        //            ViewBag.ErrorMessage = MyCard.Web.Resources.ErrorMessages.ExceptionError;
        //        }

        //        return View();
        //    }
        //    else
        //    {
        //        return View();
        //    }

        //}


        // GET: ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            ViewBag.Message = string.Empty;
            string EncryptedUserID = string.Empty;
            string EncodedUserID = string.Empty;
            string roleName = string.Empty;

            if(!this.IsCaptchaValid("Validate your captcha"))
            {
                return Json(new { success = false, responseText = "CAPTCHA verification failed." });
            }

            if (ModelState.IsValid)
            {
                try
                {
                    User user = null;
                    User user1 = null;
                    string domainName = string.Empty;
                    string companyName = string.Empty;
                    Uri url = HttpContext.Request.Url;
                    domainName = url.AbsoluteUri.Replace(url.PathAndQuery, string.Empty);

                    user1 = await _userRepository.GetFirstOrDefaultAsync(o => o.Email == model.Email);
                    if (user1 != null)
                    {
                        user = await _userRepository.GetFirstOrDefaultAsync(o => o.Email == model.Email, default(CancellationToken), o => o.UserRoles);

                        foreach (var item in user.UserRoles)
                        {
                            if (item.RoleId == 1)
                            {
                                roleName = "super_admin";
                            }
                            else if (item.RoleId == 2)
                            {
                                roleName = "company_owner";
                            }
                            else
                            {
                                roleName = "user";
                            }
                        }

                        if (roleName != "company_owner")
                        {
                            //ViewBag.ErrorMessage = MyCard.Web.Resources.ErrorMessages.EmailNotExistErrorMessage;
                            return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.EmailNotExistErrorMessage }, JsonRequestBehavior.AllowGet);
                            //return View();
                        }

                        if (string.IsNullOrEmpty(user.Password))
                        {
                            //ViewBag.ErrorMessageNotActivated = MyCard.Web.Resources.ErrorMessages.NotActivatedMessage1;
                            return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.NotActivatedMessage1 }, JsonRequestBehavior.AllowGet);
                            //return View();
                        }

                        //EncryptedUserID = MyCard.Helper.EncryptionHelper.EncryptString(Convert.ToString(user.Id ));
                        EncryptedUserID = MyCard.Helper.EncryptionHelper.EncryptString(Convert.ToString(user.Id + "|" + System.DateTime.UtcNow));
                        EncodedUserID = HttpUtility.UrlEncode(EncryptedUserID);


                        Company company = null;
                        company = _companyRepository.GetElementById(user.CompanyId);
                        if (company != null)
                        {
                            companyName = company.Name;
                        }

                        if (!String.IsNullOrEmpty(user.Email))
                        {
                            HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
                            {
                                MailHelper mail = new MailHelper();
                                string subject = MyCard.Web.Resources.CaptionsAll.EmailPasswordResetSubject;
                                string body = String.Format(@Resources.CaptionsAll.EmailPasswordResetBody, companyName, domainName, EncodedUserID);
                                await mail.SendAsync(model.Email, subject, body, String.Empty);

                            });
                            //ViewBag.Message = MyCard.Web.Resources.ErrorMessages.EmailSentSuccess;
                            return Json(new { success = true, responseText = MyCard.Web.Resources.ErrorMessages.EmailSentSuccess }, JsonRequestBehavior.AllowGet);
                        }


                    }
                    else
                    {
                        //ViewBag.ErrorMessage = MyCard.Web.Resources.ErrorMessages.EmailNotExistErrorMessage;
                        return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.EmailNotExistErrorMessage }, JsonRequestBehavior.AllowGet);
                    }



                }

                catch
                {
                    //ViewBag.ErrorMessage = MyCard.Web.Resources.ErrorMessages.ExceptionError;
                    return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.ExceptionError }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.EmailNotExistErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.EmailNotExistErrorMessage }, JsonRequestBehavior.AllowGet);
            }

        }

        // GET: ResetPassword
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(string param)
        {
            int UserID = 0;
            string decrpytedParam = string.Empty;
            DateTime linkGeneratedAt = DateTime.UtcNow;
            //string DecodedUserID;
            //DecodedUserID = HttpUtility.UrlDecode(param);

            decrpytedParam = MyCard.Helper.EncryptionHelper.DecryptString(param);
            UserID = Convert.ToInt32(decrpytedParam.Split('|')[0]);
            linkGeneratedAt = Convert.ToDateTime(decrpytedParam.Split('|')[1]).ToUniversalTime();
            
            if (linkGeneratedAt.AddHours(1) <= DateTime.UtcNow)
            {
                return RedirectToAction("ResetPasswordExpired", "CompanyDashboard");
            }
            else
            {
                TempData["UserID"] = UserID;
                return View();
            }

        }

        // POST: ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(PasswordResetViewModel model)
        {
            int UserID = 0;
            UserID = Convert.ToInt32(TempData["UserID"]);
            string EncryptedPassword = MyCard.Helper.EncryptionHelper.EncryptString(model.Password);

            PasswordHistory hist = await _passwordHistoryRepository.GetFirstOrDefaultAsync(o => o.UserId == UserID && o.Password == EncryptedPassword);
            
            if(hist != null)
            {
                TempData["Error"] = "Please choose a different password than your old ones.";
                TempData["UserID"] = UserID;
                return View();
            }

            if (ModelState.IsValid)
            {
                User user = new User();
                user = await _userRepository.GetFirstOrDefaultAsync(o => o.Id == UserID);
                
                user.Password = EncryptedPassword;
                _userRepository.SetModified(user);

                await _userRepository.UnitOfWork.CommitAsync();

                PasswordHistory histNew = new PasswordHistory() { UserId = user.Id, Password = EncryptedPassword, CreationDate = DateTime.Now, LastUpdateDate = DateTime.Now };
                _passwordHistoryRepository.Add(histNew);
                await _passwordHistoryRepository.UnitOfWork.CommitAsync();

                return RedirectToAction("ResetPasswordThanks");
            }

            TempData["UserID"] = UserID;
            return View();
        }

        // GET: ResetPassword Thanks
        [AllowAnonymous]
        public ActionResult ResetPasswordThanks()
        {
            return View();
        }

        // GET: ResetPassword Thanks
        [AllowAnonymous]
        public ActionResult ResetPasswordExpired()
        {
            return View();
        }

        ////for getting employees list page wise
        //public ActionResult EmployeeListing()
        //{
        //    EmployeeListingViewModel listingModel = new EmployeeListingViewModel();
        //    EmployeeListViewModel model;
        //    IList<EmployeeListViewModel> modelList = new List<EmployeeListViewModel>();
        //    int pageSize = 10;


        //    // Finding the company id through authenticated user
        //    int companyid = 0;
        //    int UserID = 0;
        //    if (HttpContext.Request.IsAuthenticated)
        //    {
        //        UserID = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
        //    }

        //    if (UserID > 0)
        //    {
        //        User user = null;
        //        Company company = new Company();
        //        user = _userRepository.GetElementById(UserID);

        //        if (user != null)
        //        {
        //            companyid = user.CompanyId;
        //        }
        //    }


        //    ////////////////////////

        //    listingModel.CompanyID = companyid;

        //    IList<User> EmployeeList = new List<User>();
        //    //int totalRows = _userRepository.GetFilteredElements(x => x.CompanyId == listingModel.CompanyID && x.Active==1).Count();
        //    int totalRows = 0;
        //    IList<int> userIds = new List<int>();
        //    EmployeeList = _userRepository.GetFilteredElements(x => x.CompanyId == listingModel.CompanyID && x.Active == 1, x=>x.UserRoles).ToList();

        //    foreach(User item in EmployeeList)
        //    {
        //        foreach(UserRole ur in item.UserRoles)
        //        {
        //            if(ur.RoleId==3)
        //            {
        //                userIds.Add(ur.UserId);
        //                totalRows++;
        //            }
        //        }
        //    }

        //    double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(pageSize));
        //    listingModel.PageCount = (int)Math.Ceiling(pageCount);

        //    if (listingModel.CurrentPageIndex == 0)
        //    {
        //        listingModel.CurrentPageIndex = 0;
        //    }
        //    listingModel.PageSize = pageSize;

        //    IList<User> users = null;
        //    //users = _userRepository.GetPagedElements(listingModel.CurrentPageIndex, listingModel.PageSize, o => o.Id, x => x.CompanyId == listingModel.CompanyID && x.Active==1, false).ToList();
        //    users = _userRepository.GetPagedElements(listingModel.CurrentPageIndex, listingModel.PageSize, o => o.Id, x => x.CompanyId == listingModel.CompanyID && x.Active == 1 && userIds.Contains(x.Id), false).ToList();
        //    //users = null;
        //    if (users != null)
        //    {
        //        foreach (User item in users)
        //        {
        //            model = new EmployeeListViewModel();
        //            model.ID = item.Id;
        //            model.EmployeeName = item.Name;
        //            model.EmailID = item.Email;
        //            model.JoiningDate = item.CreationDate.ToString("d-MMM-yyyy");
        //            model.Status = item.Active == 1 ? "Activated" : "Deactivated";
        //            modelList.Add(model);
        //        }
        //    }


        //    listingModel.EmployeesList = modelList;
        //    return View(listingModel);
        //}

        //[HttpPost]
        //public ActionResult EmployeeListing(EmployeeListingViewModel employeeListingModel)
        //{
        //    int totalRows = 0;
        //    if (employeeListingModel.DateTo != null)
        //    {
        //        employeeListingModel.DateTo = employeeListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
        //    }

        //    ///////////Filtering Records, Setting Filter
        //    Expression<Func<User, bool>> filter = null;
        //    if (!String.IsNullOrEmpty(employeeListingModel.SearchString) && (employeeListingModel.DateFrom == null || employeeListingModel.DateTo == null))
        //    {
        //        filter = x => x.CompanyId == employeeListingModel.CompanyID && x.Active==1 && (x.Name.Contains(employeeListingModel.SearchString) || x.Email.Contains(employeeListingModel.SearchString));
        //    }

        //    if (employeeListingModel.DateFrom != null && employeeListingModel.DateTo != null && String.IsNullOrEmpty(employeeListingModel.SearchString))
        //    {
        //        employeeListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
        //        filter = x => x.CompanyId == employeeListingModel.CompanyID && x.Active==1 && x.CreationDate >= employeeListingModel.DateFrom.Value && x.CreationDate <= employeeListingModel.DateTo.Value;
        //    }

        //    if (employeeListingModel.DateFrom != null && employeeListingModel.DateTo != null && !String.IsNullOrEmpty(employeeListingModel.SearchString))
        //    {
        //        employeeListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
        //        filter = x => x.CompanyId == employeeListingModel.CompanyID && x.Active==1 && (x.Name.Contains(employeeListingModel.SearchString) || x.Email.Contains(employeeListingModel.SearchString)
        //        && (x.CreationDate >= employeeListingModel.DateFrom.Value && x.CreationDate <= employeeListingModel.DateTo.Value));
        //    }
        //    /////////Filtering Records
        //    IList<User> userList = new List<User>();
        //    if (filter == null)
        //    {
        //        userList = _userRepository.GetFilteredElements(x => x.CompanyId == employeeListingModel.CompanyID && x.Active==1, x=>x.UserRoles).ToList();
        //    }
        //    else
        //    {
        //        userList = _userRepository.GetFilteredElements(filter, x=>x.UserRoles).ToList();
        //    }

        //    IList<int> userIds = new List<int>();

        //    if (userList != null)
        //    {
        //        //totalRows = userList.Count();
        //        foreach (User item in userList)
        //        {
        //            foreach (UserRole ur in item.UserRoles)
        //            {
        //                if (ur.RoleId == 3)
        //                {
        //                    userIds.Add(ur.UserId);
        //                    totalRows++;
        //                }
        //            }
        //        }
        //    }

        //    List<EmployeeListViewModel> modelList = new List<EmployeeListViewModel>();
        //    EmployeeListViewModel model = null;

        //    IList<User> users = null;

        //    double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(employeeListingModel.PageSize));
        //    employeeListingModel.PageCount = (int)Math.Ceiling(pageCount);
        //    //employeeListingModel.CompanyID = companyid;
        //    if (filter == null)
        //    {
        //        users = _userRepository.GetPagedElements(employeeListingModel.CurrentPageIndex, employeeListingModel.PageSize, o => o.Id, x => x.CompanyId == employeeListingModel.CompanyID && userIds.Contains(x.Id), false).ToList();
        //    }
        //    else
        //    {
        //        users = _userRepository.GetPagedElements(employeeListingModel.CurrentPageIndex, employeeListingModel.PageSize, o => o.Id, x => x.CompanyId == employeeListingModel.CompanyID && userIds.Contains(x.Id), false).ToList();
        //    }
        //    //users = null;
        //    if (users != null)
        //    {
        //        foreach (var item in users)
        //        {
        //            model = new EmployeeListViewModel();
        //            model.ID = item.Id;
        //            model.EmployeeName = item.Name;
        //            model.EmailID = item.Email;
        //            model.JoiningDate = item.CreationDate.ToString("d-MMM-yyyy");
        //            model.Status = item.Active == 1 ? "Activated" : "Deactivated";
        //            modelList.Add(model);
        //        }
        //    }

        //    employeeListingModel.EmployeesList = modelList;

        //    return View(employeeListingModel);
        //}

        ////EmployeesListing
        //public ActionResult EmployeesListing()
        //{
        //    EmployeeListingViewModel listingModel = new EmployeeListingViewModel();
        //    EmployeeListViewModel model;
        //    IList<EmployeeListViewModel> modelList = new List<EmployeeListViewModel>();
        //    int pageSize = 10;


        //    // Finding the company id through authenticated user
        //    int companyid = 0;
        //    int UserID = 0;
        //    if (HttpContext.Request.IsAuthenticated)
        //    {
        //        UserID = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
        //    }

        //    if (UserID > 0)
        //    {
        //        User user = null;
        //        Company company = new Company();
        //        user = _userRepository.GetElementById(UserID);

        //        if (user != null)
        //        {
        //            companyid = user.CompanyId;
        //        }
        //    }


        //    ////////////////////////

        //    listingModel.CompanyID = companyid;

        //    IList<User> EmployeeList = new List<User>();
        //    //int totalRows = _userRepository.GetFilteredElements(x => x.CompanyId == listingModel.CompanyID && x.Active==1).Count();
        //    int totalRows = 0;
        //    IList<int> userIds = new List<int>();
        //    EmployeeList = _userRepository.GetFilteredElements(x => x.CompanyId == listingModel.CompanyID && x.Active == 1, x => x.UserRoles).ToList();

        //    foreach (User item in EmployeeList)
        //    {
        //        foreach (UserRole ur in item.UserRoles)
        //        {
        //            if (ur.RoleId == 3)
        //            {
        //                userIds.Add(ur.UserId);
        //                totalRows++;
        //            }
        //        }
        //    }

        //    double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(pageSize));
        //    listingModel.PageCount = (int)Math.Ceiling(pageCount);

        //    if (listingModel.CurrentPageIndex == 0)
        //    {
        //        listingModel.CurrentPageIndex = 0;
        //    }
        //    listingModel.PageSize = pageSize;

        //    IList<User> users = null;
        //    //users = _userRepository.GetPagedElements(listingModel.CurrentPageIndex, listingModel.PageSize, o => o.Id, x => x.CompanyId == listingModel.CompanyID && x.Active==1, false).ToList();
        //    users = _userRepository.GetPagedElements(listingModel.CurrentPageIndex, listingModel.PageSize, o => o.Id, x => x.CompanyId == listingModel.CompanyID && x.Active == 1 && userIds.Contains(x.Id), false).ToList();
        //    //users = null;
        //    if (users != null)
        //    {
        //        foreach (User item in users)
        //        {
        //            model = new EmployeeListViewModel();
        //            model.ID = item.Id;
        //            model.EmployeeName = item.Name;
        //            model.EmailID = item.Email;
        //            model.JoiningDate = item.CreationDate.ToString("d-MMM-yyyy");
        //            model.Status = item.Active == 1 ? "Activated" : "Deactivated";
        //            modelList.Add(model);
        //        }
        //    }


        //    listingModel.EmployeesList = modelList;
        //    return View(listingModel);
        //}

        //[HttpPost]
        //public ActionResult EmployeesListing(EmployeeListingViewModel employeeListingModel)
        //{
        //    int totalRows = 0;
        //    if (employeeListingModel.DateTo != null)
        //    {
        //        employeeListingModel.DateTo = employeeListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
        //    }

        //    ///////////Filtering Records, Setting Filter
        //    Expression<Func<User, bool>> filter = null;
        //    if (!String.IsNullOrEmpty(employeeListingModel.SearchString) && (employeeListingModel.DateFrom == null || employeeListingModel.DateTo == null))
        //    {
        //        filter = x => x.CompanyId == employeeListingModel.CompanyID && x.Active == 1 && (x.Name.Contains(employeeListingModel.SearchString) || x.Email.Contains(employeeListingModel.SearchString));
        //    }

        //    if (employeeListingModel.DateFrom != null && employeeListingModel.DateTo != null && String.IsNullOrEmpty(employeeListingModel.SearchString))
        //    {
        //        employeeListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
        //        filter = x => x.CompanyId == employeeListingModel.CompanyID && x.Active == 1 && x.CreationDate >= employeeListingModel.DateFrom.Value && x.CreationDate <= employeeListingModel.DateTo.Value;
        //    }

        //    if (employeeListingModel.DateFrom != null && employeeListingModel.DateTo != null && !String.IsNullOrEmpty(employeeListingModel.SearchString))
        //    {
        //        employeeListingModel.DateTo.Value.AddHours(23).AddMinutes(59);
        //        filter = x => x.CompanyId == employeeListingModel.CompanyID && x.Active == 1 && (x.Name.Contains(employeeListingModel.SearchString) || x.Email.Contains(employeeListingModel.SearchString)
        //        && (x.CreationDate >= employeeListingModel.DateFrom.Value && x.CreationDate <= employeeListingModel.DateTo.Value));
        //    }
        //    /////////Filtering Records
        //    IList<User> userList = new List<User>();
        //    if (filter == null)
        //    {
        //        userList = _userRepository.GetFilteredElements(x => x.CompanyId == employeeListingModel.CompanyID && x.Active == 1, x => x.UserRoles).ToList();
        //    }
        //    else
        //    {
        //        userList = _userRepository.GetFilteredElements(filter, x => x.UserRoles).ToList();
        //    }

        //    IList<int> userIds = new List<int>();

        //    if (userList != null)
        //    {
        //        //totalRows = userList.Count();
        //        foreach (User item in userList)
        //        {
        //            foreach (UserRole ur in item.UserRoles)
        //            {
        //                if (ur.RoleId == 3)
        //                {
        //                    userIds.Add(ur.UserId);
        //                    totalRows++;
        //                }
        //            }
        //        }
        //    }

        //    List<EmployeeListViewModel> modelList = new List<EmployeeListViewModel>();
        //    EmployeeListViewModel model = null;

        //    IList<User> users = null;

        //    double pageCount = (double)((decimal)totalRows / Convert.ToDecimal(employeeListingModel.PageSize));
        //    employeeListingModel.PageCount = (int)Math.Ceiling(pageCount);
        //    //employeeListingModel.CompanyID = companyid;
        //    if (filter == null)
        //    {
        //        users = _userRepository.GetPagedElements(employeeListingModel.CurrentPageIndex, employeeListingModel.PageSize, o => o.Id, x => x.CompanyId == employeeListingModel.CompanyID && userIds.Contains(x.Id), false).ToList();
        //    }
        //    else
        //    {
        //        users = _userRepository.GetPagedElements(employeeListingModel.CurrentPageIndex, employeeListingModel.PageSize, o => o.Id, x => x.CompanyId == employeeListingModel.CompanyID && userIds.Contains(x.Id), false).ToList();
        //    }
        //    //users = null;
        //    if (users != null)
        //    {
        //        foreach (var item in users)
        //        {
        //            model = new EmployeeListViewModel();
        //            model.ID = item.Id;
        //            model.EmployeeName = item.Name;
        //            model.EmailID = item.Email;
        //            model.JoiningDate = item.CreationDate.ToString("d-MMM-yyyy");
        //            model.Status = item.Active == 1 ? "Activated" : "Deactivated";
        //            modelList.Add(model);
        //        }
        //    }

        //    employeeListingModel.EmployeesList = modelList;

        //    return PartialView("_PartialEmployees", employeeListingModel);
        //}
        ////EmployeesListing


        [HttpPost]
        public async Task<JsonResult> DeactivateEmployee(int status, int userid)
        {
            //Update PO now
            User user = null;

            user = _userRepository.GetElementById(userid);
            user.Active = status;

            _userRepository.SetModified(user);
            await _userRepository.UnitOfWork.CommitAsync();
            return Json(null);
        }

        public FileResult GetCompanyLogoImage()
        {
            Company company = null;
            byte[] bytes = null;
            string fileName = string.Empty, contentType = string.Empty;

            //contentType = "application/pdf";
            //fileName = "order";

            int companyId = 0;
            int userId = 0;
            //Get User ID From Cookie
            CompanyEditViewModel model = new CompanyEditViewModel();
            if (HttpContext.Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (userId > 0)
            {
                User user = new User();
                user = _userRepository.GetFilteredElements(o => o.Id == userId).FirstOrDefault();

                if (user != null)
                {
                    companyId = user.CompanyId;
                }
            }

            company = _companyRepository.GetFilteredElements(o => o.Id == companyId).SingleOrDefault();

            if (company != null)
            {
                bytes = (byte[])company.Logo;
                contentType = company.LogoContentType;
                fileName = company.LogoFileName;
            }
            if (fileName != null && fileName != "")
                return File(bytes, contentType, fileName);
            else
                return null;
        }

        // GET: CompanyDashboard/EditCompany
        public async Task<ActionResult> EditCompany()
        {
            int CompanyID = 0;
            int UserID = 0;
            //Get User ID From Cookie
            CompanyEditViewModel model = new CompanyEditViewModel();
            if (HttpContext.Request.IsAuthenticated)
            {
                UserID = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (UserID > 0)
            {
                User user = new User();
                Company company = new Company();
                user = await _userRepository.GetFirstOrDefaultAsync(o => o.Id == UserID);

                if (user != null)
                {
                    CompanyID = user.CompanyId;
                    
                    company = await _companyRepository.GetFirstOrDefaultAsync(o => o.Id == CompanyID);
                }
                if (company != null)
                {
                    model.CustomerName = string.IsNullOrEmpty(company.CustomerName) ? string.Empty : company.CustomerName;
                    model.Name = string.IsNullOrEmpty(company.Name) ? string.Empty : company.Name;
                    model.DomainName = string.IsNullOrEmpty(company.Domain) ? string.Empty : company.Domain;
                    model.EmailID = string.IsNullOrEmpty(company.Email) ? string.Empty : company.Email;
                    model.Phone = string.IsNullOrEmpty(company.Phone) ? string.Empty : company.Phone;
                    model.Address = string.IsNullOrEmpty(company.Address) ? string.Empty : company.Address;
                    model.City = string.IsNullOrEmpty(company.City) ? string.Empty : company.City;
                    model.State = string.IsNullOrEmpty(company.State) ? string.Empty : company.State;
                    model.Zip = string.IsNullOrEmpty(company.Zip) ? string.Empty : company.Zip;
                    model.CompanyLogo = company.Logo;
                    ViewBag.FileName = company.LogoFileName;
                }

                var poTotalAmount = _purchaseOrderRepository.GetFilteredElements(o => o.CompanyId == user.CompanyId && o.Approved == 1).Sum(o => o.LicensesOrdered);
                ViewBag.TotalPurchase = poTotalAmount;

                ViewBag.Designer = "No";

                PurchaseOrder poDesigner = _purchaseOrderRepository.GetFilteredElements(x => x.CompanyId == user.CompanyId && x.Approved == 1).FirstOrDefault();
                if (poDesigner != null)
                {
                    ViewBag.Designer = "Yes";
                }

                PurchaseOrder poAddLicense = _purchaseOrderRepository.GetFilteredElements(x => x.CompanyId == user.CompanyId).FirstOrDefault();
                if (poAddLicense != null)
                {
                    ViewBag.AddLicense = "Yes";
                }
                                
            }

            ViewBag.myCardCMS = "No";
            ViewBag.myCardCMSEdit = "No";

            Company companyCMS = await _companyRepository.GetFirstOrDefaultAsync(x => x.Id == CompanyID && (x.AccountType == 2 || x.AccountType == 3));
            if (companyCMS != null)
            {
                ViewBag.myCardCMS = "Yes";
                if (companyCMS.AccountType == 2)
                {
                    ViewBag.myCardCMSEdit = "Yes";
                }
            }


            return View(model);
        }

        // POST: CompanyDashboard/EditCompany
        [HttpPost]
        public async Task<ActionResult> EditCompany(CompanyEditViewModel model, HttpPostedFileBase file)
        {

            int CompanyID = 0;
            int UserID = 0;
            
            //Get User ID From Cookie
            if (file != null)
            {
                if (file.ContentType.ToLower() != "image/jpg" &&
                    file.ContentType.ToLower() != "image/jpeg" &&
                    file.ContentType.ToLower() != "image/pjpeg" &&
                    file.ContentType.ToLower() != "image/gif" &&
                    file.ContentType.ToLower() != "image/x-png" &&
                    file.ContentType.ToLower() != "image/png")
                {
                    ModelState.AddModelError("ImageUpload", "Please choose either a JPG or PNG image");
                }
            }

            


            if (HttpContext.Request.IsAuthenticated)
            {
                UserID = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (UserID > 0)
            {
                User user = new User();
                Company company = new Company();
                user = await _userRepository.GetFirstOrDefaultAsync(o => o.Id == UserID);

                if (user != null)
                {
                    CompanyID = user.CompanyId;

                    company = await _companyRepository.GetFirstOrDefaultAsync(o => o.Id == CompanyID);
                }

                ////For Add License and Designer Menu
                var poTotalAmount = _purchaseOrderRepository.GetFilteredElements(o => o.CompanyId == user.CompanyId && o.Approved == 1).Sum(o => o.LicensesOrdered);
                ViewBag.TotalPurchase = poTotalAmount;

                ViewBag.Designer = "No";

                PurchaseOrder poDesigner = _purchaseOrderRepository.GetFilteredElements(x => x.CompanyId == user.CompanyId && x.Approved == 1).FirstOrDefault();
                if (poDesigner != null)
                {
                    ViewBag.Designer = "Yes";
                }

                PurchaseOrder poAddLicense = _purchaseOrderRepository.GetFilteredElements(x => x.CompanyId == user.CompanyId).FirstOrDefault();
                if (poAddLicense != null)
                {
                    ViewBag.AddLicense = "Yes";
                }

                ViewBag.myCardCMS = "No";
                ViewBag.myCardCMSEdit = "No";

                Company companyCMS = await _companyRepository.GetFirstOrDefaultAsync(x => x.Id == CompanyID && (x.AccountType == 2 || x.AccountType == 3));
                if (companyCMS != null)
                {
                    ViewBag.myCardCMS = "Yes";
                    if (companyCMS.AccountType == 2)
                    {
                        ViewBag.myCardCMSEdit = "Yes";
                    }
                    
                }

                ////
                

                if (company != null)
                {
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            //Saving uploaded file
                            if (file != null && file.ContentLength > 0)
                            {
                                string ext = Path.GetExtension(file.FileName);
                                //if (String.IsNullOrEmpty(ext) || (!ext.Equals(".png", StringComparison.OrdinalIgnoreCase)))
                                //{
                                //    ViewBag.Message = "File is not png format";
                                //    return View();
                                //}
                                //else
                                //{
                                byte[] uploadedFile = new byte[file.InputStream.Length];
                                //file.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                                using (BinaryReader br = new BinaryReader(file.InputStream))
                                {
                                    uploadedFile = br.ReadBytes(file.ContentLength);
                                }
                                company.Logo = uploadedFile;
                                company.LogoFileName = file.FileName;
                                company.LogoContentType = file.ContentType;
                                model.CompanyLogo = uploadedFile;
                                //}
                            }

                            /////////////
                            ////company.CustomerName = model.CustomerName;
                            ////company.Name = model.Name;
                            ////company.Domain = model.DomainName;
                            ////company.Email = model.EmailID;
                            //company.Phone = model.Phone;
                            //company.Address = model.Address;
                            //company.City = model.City;
                            //company.State = model.State;
                            //company.Zip = model.Zip;

                            _companyRepository.SetModified(company);

                            await _companyRepository.UnitOfWork.CommitAsync();

                            ViewBag.Message = MyCard.Web.Resources.CaptionsAll.CompanyDetailsUpdated;
                            //return View();
                            return RedirectToAction("Index", "CompanyDashboard");
                        }
                        catch
                        {
                            ViewBag.ErrorMessage = MyCard.Web.Resources.ErrorMessages.ExceptionError;
                            return View();
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
            }

            return View(model);
        }
        //[HttpPost]
        public JsonResult LicensePrice(int LicenseId)
        {
            LicenseType licenseType = new LicenseType();
            licenseType = _licenseTypeRepository.GetElementById(LicenseId);

            LicenseTypeModel model = new LicenseTypeModel();
            if (licenseType != null)
            {
                model.Id = licenseType.Id;
                model.LicenseTypeName = licenseType.LicenseTypeName;
                model.Price = licenseType.Price;
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public class bodyClass
        {
            public string profile_id {get; set;}
            public string tran_type {get; set;}
            public string tran_class {get; set;}
            public string cart_description {get; set;}
            public string cart_id {get; set;}            
            public string cart_currency { get; set; }      
            public string cart_amount {get; set;}      
            public string callback { get; set; }         
            public string Return {get; set;}            
        }

        public async Task<ActionResult> PayTimeAPiAsync(string totalCosts, string totallicenses, string LicenseTypeId)
        {
            int CompanyID = 0;
            int orderID = 0;
            int UserID = 0;
            int maxOrderID = 0;
            string CompanyName = string.Empty;
            string UserEmail = string.Empty;
            string attachmentName = string.Empty;
            string customerName = string.Empty;
            string customerTitle = string.Empty;
            string qoutationDate = DateTime.UtcNow.Date.ToString("d-MMM-yyyy");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("authorization", "S2JNLWNHMB-HZTBRG2ZNK-TTGR2ZWWTB");
            JObject obj = JObject.FromObject(new bodyClass()
            {
                profile_id = Convert.ToString(50108),
                tran_type = "sale",
                tran_class = "ecom",
                cart_description = "Transaction For Purchaseing" + totallicenses + "License",
                cart_id = "4244b9fd-c7e9-4f16-8d3c-4fe7bf6c48ca",
                cart_currency = "USD",
                cart_amount = totalCosts,
                callback = "https://portal.mycards.com/CompanyDashboard/Responsepage",
                Return = "https://portal.mycards.com/CompanyDashboard/Responsepage",
            });
            HttpContent content = new StringContent(obj.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://secure.paytabs.com/payment/request", content);

            if (response.IsSuccessStatusCode == true)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var model = JsonConvert.DeserializeObject<PayTab_ResponseModel>(responseString);
                int poCount = _purchaseOrderRepository.GetAllElements().Count();
                maxOrderID = poCount;
                maxOrderID++;
                string OrderNumber = string.Empty;
                OrderNumber = "PayTab-" + maxOrderID.ToString() + "-" + DateTime.UtcNow.Year.ToString();
                PurchaseOrder purchaseOrder = new PurchaseOrder();
                purchaseOrder.OrderNumber = OrderNumber;
                purchaseOrder.LicenseId = Convert.ToInt32(LicenseTypeId);
                purchaseOrder.CompanyId = CompanyID;
                purchaseOrder.LicensesOrdered = Convert.ToInt32(totallicenses);
                purchaseOrder.OrderTotal = Convert.ToDecimal(totalCosts);
                purchaseOrder.TermsAccepted = true;
                purchaseOrder.BillingAddress = "";
                purchaseOrder.ApprovalDate = DateTime.UtcNow;
                _purchaseOrderRepository.Add(purchaseOrder);
                await _purchaseOrderRepository.UnitOfWork.CommitAsync();
                if (HttpContext.Request.IsAuthenticated)
                {
                    UserID = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
                }
                if (UserID > 0)
                {
                    User user = new User();
                    user = await _userRepository.GetFirstOrDefaultAsync(o => o.Id == UserID);

                    if (user != null)
                    {
                        CompanyID = user.CompanyId;
                        UserEmail = user.Email;
                        var company = await _companyRepository.GetFirstOrDefaultAsync(o => o.Id == CompanyID);
                        CompanyName = company.Name;
                        customerName = company.CustomerName;
                        orderID = purchaseOrder.Id;

                        string MyConnection2 = "datasource=mycarddb.mysql.database.azure.com;port=3306;database=mycards;uid=mycardAdmin@mycarddb;password=@Card123";
                        string Query = "insert into paytaborder(callback,cart_amount,cart_currency,cart_description,cart_id,Company_Name,Payment_Date,customer_Name,redirect_url,ReturnPage,tran_ref,tran_type,User_Email,User_ID,Company_ID,PurchaseOrder_Id)" + "values('" + model.callback + "','" + model.cart_amount + "','" + model.cart_currency + "','" + model.cart_description + "','" + model.cart_id + "','" + company.Name + "','" + DateTime.UtcNow + "','" + company.CustomerName + "','" + model.redirect_url + "','" + model.ReturnPage + "','" + model.tran_ref + "','" + model.tran_type + "','" +  user.Email + "','" + UserID + "','" + user.CompanyId + "','" + purchaseOrder.Id + "');";
                        MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                        MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                        MySqlDataReader MyReader2;
                        MyConn2.Open();
                        MyReader2 = MyCommand2.ExecuteReader();
                        while (MyReader2.Read())
                        {
                        }
                        MyConn2.Close();
                    }
                }
                var returnurl = model.redirect_url;
                ViewBag.messagestatus = "1";
                return Json(returnurl, JsonRequestBehavior.AllowGet);

            }
            else
            {
                ViewBag.messagestatus = "0";
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Responsepage()
        {
            return View();
        }
        // GET: PurchaseLicense
        public async Task<ActionResult> Purchase()
        {
            IList<LicenseType> licneseTypeList = null;
            decimal price = 0;
            int NoOfLicense = 0;

            int companyId = 0;
            int userId = 0;
            if (HttpContext.Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (userId > 0)
            {
                User user = null;
                user = _userRepository.GetElementById(userId);

                if (user != null)
                {
                    companyId = user.CompanyId;
                }
            }

            ViewBag.Designer = "No";
            PurchaseOrder po = await _purchaseOrderRepository.GetFirstOrDefaultAsync(x => x.CompanyId == companyId && x.Approved == 1);
            if(po != null)
            {
                ViewBag.Designer = "Yes";
            }


            Company company = await _companyRepository.GetFirstOrDefaultAsync(x => x.Id == companyId && (x.AccountType == 2 || x.AccountType == 3));
            if (company != null)
            {
                ViewBag.myCardCMS = "Yes";
            }

            

            //licneseTypeList = _licenseTypeRepository.GetFilteredElements(o => o.LicenseTypeName == "Purchase").ToList();
            //if (licneseTypeList != null)
            //{
            //    foreach (LicenseType item in licneseTypeList)
            //    {
            //        int from = (item.From == null) ? 0 : Convert.ToInt32(item.From);
            //        int to = (item.To == null) ? Int32.MaxValue : Convert.ToInt32(item.To);
            //        if (NoOfLicense >= from && NoOfLicense <= to)
            //        {
            //            price = item.Price;
            //        }
            //    }

            //}
            PurchaseLicenseViewModel model = new PurchaseLicenseViewModel();
            IList<LicenseTypeModel> licenseTypeList = new List<LicenseTypeModel>();
            LicenseTypeModel licenseType;
            licneseTypeList = _licenseTypeRepository.GetAllElements().ToList();
            if (licneseTypeList != null)
            {
                foreach (LicenseType item in licneseTypeList)
                {
                    licenseType = new LicenseTypeModel();
                    licenseType.Id = item.Id;
                    licenseType.LicenseTypeName = item.LicenseTypeName;
                    licenseType.Price = item.Price;

                    licenseTypeList.Add(licenseType);
                }

            }

            //Adding below so that the radio button be selected.
            if (licenseTypeList.Count==1)
            {
                LicenseType licenseTypeNew = _licenseTypeRepository.GetAllElements().FirstOrDefault();
                model.LicenseTypeId = licenseTypeNew.Id;
                //model.NoOfLicense = 0;
            }
            

            model.LicenseTypes = licenseTypeList;

            ViewBag.Price = price;

            ViewBag.PageType = "Purchase";
            return View(model);
        }

        // POST: PurchaseLicense
        [HttpPost]
        public async Task<ActionResult> Purchase(PurchaseLicenseViewModel model)
        {
            decimal totalCost = 0;
            decimal price = 0;
            int licenseTypeId = 0;
            ViewBag.PageType = "Purchase";
            string CompanyName = string.Empty;
            int NoOfLicenses = 0;
            int companyAccountType = 0;

            int companyId = 0;
            int userId = 0;
            
            if (HttpContext.Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (userId > 0)
            {
                User user = null;
                user = _userRepository.GetElementById(userId);

                if (user != null)
                {
                    companyId = user.CompanyId;
                }
            }

            ViewBag.Designer = "No";
            ViewBag.myCardCMS = "No";
            PurchaseOrder poDesigner = await _purchaseOrderRepository.GetFirstOrDefaultAsync(x => x.CompanyId == companyId && x.Approved == 1);
            if (poDesigner != null)
            {
                ViewBag.Designer = "Yes";
            }

            Company company = await _companyRepository.GetFirstOrDefaultAsync(x => x.Id == companyId );
            if (company != null)
            {
                companyAccountType = company.AccountType;
                if (company.AccountType==2 || company.AccountType == 3)
                {
                    ViewBag.myCardCMS = "Yes";
                }
            }

            if (ModelState.IsValid)
            {
                //totalCost = price * model.NoOfLicense;

                int CompanyID = 0;
                int UserID = 0;
                int orderID = 0;
                int maxOrderID = 0;
                string UserEmail = string.Empty;
                string attachmentName = string.Empty;
                string customerName = string.Empty;
                string customerTitle = string.Empty;
                string qoutationDate = DateTime.UtcNow.Date.ToString("d-MMM-yyyy");

                NoOfLicenses = model.NoOfLicense;

                //Get User ID From Cookie
                if (HttpContext.Request.IsAuthenticated)
                {
                    UserID = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
                }

                if (UserID > 0)
                {
                    User user = new User();
                    //Company company = new Company();
                    user = await _userRepository.GetFirstOrDefaultAsync(o => o.Id == UserID);

                    if (user != null)
                    {
                        CompanyID = user.CompanyId;
                        UserEmail = user.Email;

                        company = await _companyRepository.GetFirstOrDefaultAsync(o => o.Id == CompanyID);
                        CompanyName = company.Name;
                        customerName = company.CustomerName;
                        customerTitle = company.BusinessTitle;
                    }
                }

                IList<LicenseType> licneseTypeList = null;

                licneseTypeList = _licenseTypeRepository.GetFilteredElements(o => o.Id == model.LicenseTypeId).ToList();

                if (licneseTypeList != null)
                {
                    foreach (LicenseType item in licneseTypeList)
                    {
                        totalCost = model.NoOfLicense * item.Price;
                        price = item.Price;
                        licenseTypeId = item.Id;
                    }

                }
                else
                {
                    return Json(new { success = false, responseText = MyCard.Web.Resources.CaptionsAll.RegistrationThankyouMessage }, JsonRequestBehavior.AllowGet);
                }

                IList<PurchaseOrder> po = null;
                po = _purchaseOrderRepository.GetFilteredElements(o => o.CompanyId == CompanyID).ToList();
                if (po != null)
                {
                    int poCount = _purchaseOrderRepository.GetAllElements().Count();
                    maxOrderID = poCount;
                }
                maxOrderID++;
                string OrderNumber = string.Empty;
                //OrderNumber = "MYC-" + CompanyID.ToString() + "-" + maxOrderID.ToString("0000");
                OrderNumber = "MCQ-" + maxOrderID.ToString() + "-" + DateTime.UtcNow.Year.ToString();
                //Code to Add Purchase Order Record
                PurchaseOrder purchaseOrder = new PurchaseOrder();
                purchaseOrder.OrderNumber = OrderNumber;
                purchaseOrder.LicenseId = licenseTypeId;// Convert.ToInt32(TempData["LicenseTypeID"]);
                purchaseOrder.CompanyId = CompanyID;
                purchaseOrder.LicensesOrdered = model.NoOfLicense;// Convert.ToInt32(TempData["NoOfLicenses"]);
                purchaseOrder.OrderTotal = totalCost;//Convert.ToDecimal(TempData["TotalCost"]);
                purchaseOrder.TermsAccepted = true;
                purchaseOrder.BillingAddress = "";//TempData["BillingAddress"].ToString();
                purchaseOrder.ApprovalDate = DateTime.UtcNow;

                _purchaseOrderRepository.Add(purchaseOrder);

                await _purchaseOrderRepository.UnitOfWork.CommitAsync();

                orderID = purchaseOrder.Id;

                //Update Order Number
                purchaseOrder.OrderNumber = "MCQ-" + orderID + "-" + DateTime.UtcNow.Year.ToString();
                await _purchaseOrderRepository.UnitOfWork.CommitAsync();

                TempData["orderID"] = orderID;
                attachmentName = "myCards-Quotation-" + purchaseOrder.OrderNumber + ".pdf";


                //Email Template
                string emailBody = string.Empty;
                string emailSubject = string.Empty;
                EmailTemplate emailTemplate = null;
                //emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.Name == "Qoutation").FirstOrDefault();
                emailTemplate = _emailTemplateRepository.GetFilteredElements(x => x.TemplateId == 2).FirstOrDefault();


                if (emailTemplate != null)
                {
                    emailBody = emailTemplate.Template;
                    emailSubject = emailTemplate.Subject;
                }
                StringBuilder sbEmail = new StringBuilder(emailBody);
                sbEmail.Replace("[CompanyName]", CompanyName);

                //Email Template

                string licenseTypeName = string.Empty;

                var licenseTypes = _licenseTypeRepository.GetElementById(model.LicenseTypeId);

                if(licenseTypes!=null)
                {
                    licenseTypeName = licenseTypes.LicenseTypeName;
                }


                //Code for Creating Qoutation and sending this by email

                using (StringWriter sw = new StringWriter())
                {
                    using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                    {
                        string companyName = CompanyName;
                        string orderNo = purchaseOrder.OrderNumber;

                        StringBuilder sb = new StringBuilder();



                        //sb.Append("<header class='clearfix'>");
                        //sb.Append("<img src='~/img/admin/register-logo.png' />");
                        sb.Append("<h1>INVOICE</h1>");
                        //sb.Append("<br /><table width = '100%' cellspacing = '0' cellpadding = '2' >");
                        //sb.Append("<tr><td align = 'center' style = 'background-color: #18B5F0' colspan = '2'><b> Qoutation </b></ td></ tr >< tr >< td colspan = '2' ></ td ></ tr >" +
                        //    "< tr >< td >< b > Qoutation No:</ b >{ 0}</ td >< td >< b > Date: </ b >{ 1}</ td ></ tr >" +
                        //    "  < tr >< td colspan = '2' >< b > Company Name:</ b > { 2} </ td ></ tr >  </ table >< br />" +
                        //    " < table border = '1' >< tr >< th > License Type </ th >< th > Number of Licenses</ th >< th > Price Per License</ th >< th > Total Price </ th >" +
                        //    "< tr >< td >{ 6}</ td >< td >{ 3}</ td >< td >{ 4}</ td >< td >{ 5}</ td ></ tr ></ table > ");

                        //sb.Append(String.Format(MyCard.Web.Resources.CaptionsAll.QoutationTemplate, orderNo, DateTime.Now, companyName, model.NoOfLicense, price, totalCost, licenseTypeName));
                        StringReader sr = new StringReader(sb.ToString());

                        //Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                        Document pdfDoc = new Document(PageSize.A4, 30f, 30f, 50f, 0f);

#pragma warning disable CS0612 // Type or member is obsolete
#pragma warning disable CS0612 // Type or member is obsolete
                        HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
#pragma warning restore CS0612 // Type or member is obsolete
#pragma warning restore CS0612 // Type or member is obsolete
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                            pdfDoc.Open();
                            //pdfDoc.SetMargins(20f, 20f, 20f, 20f);
                            //htmlparser.Parse(sr);
                            try
                            {
                                

                                string imageURL = Server.MapPath("..") + "/img/logo.png";

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

                                //Registering Font
                                string fontpath = Server.MapPath("..") + "/css/Gotham/";

                                BaseFont customfont = BaseFont.CreateFont(fontpath + "Gotham-Book.otf", BaseFont.CP1252, BaseFont.EMBEDDED);

                                //iTextSharp.text.Font font = new iTextSharp.text.Font(customfont, 16, Font.BOLD , iTextSharp.text.BaseColor.BLUE);
                                iTextSharp.text.Font mainHeadingfont = new iTextSharp.text.Font(customfont, 16, Font.NORMAL, new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml("#2ea0da")));

                                Font normalFont = new Font(customfont, 11, Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                                Font normalFontWhite = new Font(customfont, 11, Font.NORMAL, iTextSharp.text.BaseColor.WHITE);
                                Font normalFont12 = new Font(customfont, 12, Font.NORMAL, iTextSharp.text.BaseColor.BLACK);
                                Font normalFont12LightBlue = new Font(customfont, 12, Font.NORMAL, new iTextSharp.text.BaseColor(System.Drawing.ColorTranslator.FromHtml("#2ea0da")));
                                
                                Font boldFont10 = new Font(customfont, 10, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                                Font boldFont11 = new Font(customfont, 11, Font.BOLD, iTextSharp.text.BaseColor.BLACK);
                                Font boldFont11UnderLine = new Font(customfont, 11, Font.BOLD | Font.UNDERLINE, iTextSharp.text.BaseColor.BLACK);

                                //Font lightblue = new Font(Font.FontFamily.COURIER, 9f, Font.NORMAL, new Color(43, 145, 175));
                                //Registering Font
                                Paragraph pInvoice = new Paragraph("Quotation", mainHeadingfont);
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

                                Paragraph pCompany = new Paragraph(companyName, normalFont12);
                                pCompany.Alignment = Element.ALIGN_LEFT;
                                pdfDoc.Add(pCompany);

                                pdfDoc.Add(Chunk.NEWLINE);

                                Paragraph pQno = new Paragraph("Quotation no: " + orderNo, normalFont12);
                                pQno.Alignment = Element.ALIGN_LEFT;
                                pdfDoc.Add(pQno);

                                //pdfDoc.Add(Chunk.NEWLINE);
                                
                                PdfPTable table = new PdfPTable(6);
                                table.WidthPercentage = 100;
                                table.SpacingBefore = 10f;
                                //PdfPHeaderCell hCell1 = new PdfPHeaderCell(new Phrase("Row 1 , Col 1, Col 2 and col 3"));

                                //float[] widths = new float[] { 4f, 2f, 2f, 2f, 1f, 1f };
                                //table.SetWidths(widths);

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


                                cell = new PdfPCell(new Phrase("myCards Enterprise Software as a Service (Saas)", normalFont));
                                cell.MinimumHeight = 40f;
                                cell.Colspan = 2;
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                table.AddCell(cell);

                                cell = new PdfPCell(new Phrase(licenseTypeName, normalFont));
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                table.AddCell(cell);

                                cell = new PdfPCell(new Phrase(NoOfLicenses.ToString()));
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

                                //cell = new PdfPCell(new Phrase("myCards APP", normalFont));
                                //cell.MinimumHeight = 40f;
                                //cell.Colspan = 2;
                                //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                //table.AddCell(cell);

                                //cell = new PdfPCell(new Phrase(""));
                                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                //table.AddCell(cell);

                                //cell = new PdfPCell(new Phrase(""));
                                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                //table.AddCell(cell);

                                //cell = new PdfPCell(new Phrase(""));
                                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                //table.AddCell(cell);

                                //cell = new PdfPCell(new Phrase(""));
                                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                //table.AddCell(cell);

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

                                Decimal totalOrderCost = totalCost + (totalCost * 5/100);

                                cell = new PdfPCell(new Phrase(String.Format("{0:n}", totalOrderCost)));
                                cell.BackgroundColor = new BaseColor(System.Drawing.Color.LightGray);
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                table.AddCell(cell);
                                                                
                                pdfDoc.Add(table);

                                pdfDoc.Add(Chunk.NEWLINE);
                                pdfDoc.Add(Chunk.NEWLINE);


                                Paragraph tc = new Paragraph("Terms & Conditions", boldFont11UnderLine);
                                tc.Alignment = Element.ALIGN_LEFT;
                                pdfDoc.Add(tc);

                                Paragraph tcLine1 = new Paragraph("    •  Quotation is valid for 30 days", normalFont);
                                tcLine1.Alignment = Element.ALIGN_LEFT;
                                //tcLine1.SpacingBefore = 20f;
                                pdfDoc.Add(tcLine1);

                                Paragraph tcLine2 = new Paragraph("    •  Access to myCardsTM Designer will be granted within 48 hours of successful ", normalFont);
                                tcLine2.Alignment = Element.ALIGN_LEFT;
                                //tcLine2.SpacingBefore = 20f;
                                pdfDoc.Add(tcLine2);

                                Paragraph tcLine21 = new Paragraph("        Purchase Order (PO) upload & verification and e-invoice dispatch", normalFont);
                                tcLine21.Alignment = Element.ALIGN_LEFT;
                                //tcLine2.SpacingBefore = 20f;
                                pdfDoc.Add(tcLine21);

                                Paragraph tcLine3 = new Paragraph("    •  This quotation is subject to the General Terms & Conditions attached", normalFont);
                                tcLine3.Alignment = Element.ALIGN_LEFT;
                                //tcLine3.SpacingBefore = 20f;
                                pdfDoc.Add(tcLine3);
                                
                                string imageSecondURL = Server.MapPath("..") + "/img/mail-bottom.png";

                                iTextSharp.text.Image jpgSecond = iTextSharp.text.Image.GetInstance(imageSecondURL);

                                //Resize image depend upon your need
                                //jpg.ScaleToFit(80, 70);

                                //Give space before image
                                // jpg.SpacingBefore = 10f;

                                //Give some space after the image
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
                                pdfDoc.Add(Chunk.NEWLINE);
                                pdfDoc.Add(Chunk.NEWLINE);
                                pdfDoc.Add(Chunk.NEWLINE);

                                Paragraph br = new Paragraph("Best Regards,", normalFont12);
                                br.Alignment = Element.ALIGN_LEFT;
                                pdfDoc.Add(br);

                                pdfDoc.Add(Chunk.NEWLINE);

                                Paragraph mst = new Paragraph("myCards Sales Team", normalFont12);
                                mst.Alignment = Element.ALIGN_LEFT;
                                pdfDoc.Add(mst);

                                Paragraph emailString = new Paragraph("sales@mycards.com", normalFont12LightBlue);
                                emailString.Alignment = Element.ALIGN_LEFT;
                                pdfDoc.Add(emailString);

                            }

                            catch (Exception ex)
                            { }

                            finally
                            {
                                pdfDoc.Close();
                            }

                            pdfDoc.Close();
                            byte[] bytes = memoryStream.ToArray();
                            memoryStream.Close();

                            //UserEmail = "jehangir.ali@gmail.com";


                            //Attaching Proposal saved in the DB
                            byte[] byteData = null;
                            string poFileName = string.Empty;

                            MyCardFile myCardFile = null;

                            myCardFile = _myCardFileRepository.GetFilteredElements(y=>y.Id== companyAccountType).OrderByDescending(x => x.Id).FirstOrDefault();

                            if (myCardFile != null)
                            {
                                byteData = (byte[])myCardFile.FileContent;
                                poFileName = myCardFile.FileName;

                            }



                            //Sending email now
                            if (!String.IsNullOrEmpty(UserEmail))
                            {
                                HostingEnvironment.QueueBackgroundWorkItem(async cancellationToken =>
                                {
                                    MailHelper mail = new MailHelper();
                                    string subject = emailSubject;// MyCard.Web.Resources.CaptionsAll.QoutationEmailSubject;
                                    string body = sbEmail.ToString();// String.Format(MyCard.Web.Resources.CaptionsAll.QoutationEmailBody, CompanyName);
                                    //await mail.SendAsyncAttach(UserEmail, subject, body, bytes, attachmentName);
                                    if (string.IsNullOrEmpty(poFileName))
                                    {
                                        await mail.SendAsyncAttach(UserEmail, subject, body, bytes, attachmentName);
                                    }
                                    else
                                    {
                                        await mail.SendAsyncAttachPO(UserEmail, subject, body, bytes, attachmentName, byteData, poFileName);
                                    }
                                });
                            }
                        }
                    }
                }
                return Json(new { success = true, responseText = MyCard.Web.Resources.CaptionsAll.RegistrationThankyouMessage }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(new { success = false, responseText = MyCard.Web.Resources.CaptionsAll.RegistrationThankyouMessage }, JsonRequestBehavior.AllowGet);
                //return View();
            }

            //return View();
        }

        // GET: UploadPO
        public async Task<ActionResult> UploadPO()
        {
            int orderID = 0;

            orderID = Convert.ToInt32(TempData["orderID"]);

            TempData["order"] = orderID;
            //ViewBag.Message = TempData["SuccessMessage"];
            ViewBag.PageType = "PurchaseUpload";

            int companyId = 0;
            int userId = 0;
            
            if (HttpContext.Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (userId > 0)
            {
                User user = null;
                user = _userRepository.GetElementById(userId);

                if (user != null)
                {
                    companyId = user.CompanyId;
                }
            }

            ViewBag.myCardCMS = "No";
            Company company = await _companyRepository.GetFirstOrDefaultAsync(x => x.Id == companyId && (x.AccountType == 2 || x.AccountType == 3));
            if (company != null)
            {
                ViewBag.myCardCMS = "Yes";
            }

            PurchaseOrder poUpload = _purchaseOrderRepository.GetFilteredElements(x => x.CompanyId == companyId).OrderByDescending(x => x.Id).FirstOrDefault();
            
            if (poUpload!=null)
            {
                orderID = poUpload.Id;
            }

            if (orderID == 0)
            {
                return RedirectToAction("PurchaseLicense", "Purchase");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            int orderID = 0;
            ViewBag.PageType = "PurchaseUpload";
            orderID = Convert.ToInt32(TempData["order"]);
            TempData["orderID"] = orderID;

            int companyId = 0;
            int userId = 0;

            if (HttpContext.Request.IsAuthenticated)
            {
                userId = Convert.ToInt32(HttpContext.User.Identity.Name.Split('|')[1]);
            }

            if (userId > 0)
            {
                User user = null;
                user = _userRepository.GetElementById(userId);

                if (user != null)
                {
                    companyId = user.CompanyId;
                }
            }

            ViewBag.myCardCMS = "No";
            Company company = await _companyRepository.GetFirstOrDefaultAsync(x => x.Id == companyId && (x.AccountType == 2 || x.AccountType == 3));
            if (company != null)
            {
                ViewBag.myCardCMS = "Yes";
            }

            PurchaseOrder poUpload = _purchaseOrderRepository.GetFilteredElements(x => x.CompanyId == companyId).OrderByDescending(x => x.Id).FirstOrDefault();

            if (poUpload != null)
            {
                orderID = poUpload.Id;
            }


            try
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

                        PurchaseOrder purchaseOrder = null;

                        purchaseOrder = await _purchaseOrderRepository.GetFirstOrDefaultAsync(o => o.Id == orderID);

                        purchaseOrder.OrderFile = uploadedFile;
                        purchaseOrder.FileName = purchaseOrder.OrderNumber;
                        purchaseOrder.ContentType = file.ContentType;

                        _purchaseOrderRepository.SetModified(purchaseOrder);
                        await _purchaseOrderRepository.UnitOfWork.CommitAsync();

                        return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
                    }


                }
                //ALL STRINGS IN RESOURCE FILES

                return Json(new { success = false, responseText = MyCard.Web.Resources.ErrorMessages.PurchaseUploadPOFileNotSelectedError }, JsonRequestBehavior.AllowGet);
                //TempData["SuccessMessage"] = MyCard.Web.Resources.ErrorMessages.POUploadSuccess;
                //return RedirectToAction("UploadPO");
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
                //ViewBag.ErrorMessage = MyCard.Web.Resources.ErrorMessages.ExceptionError;
                //return View();
            }
        }

        public async Task<ActionResult> SendHardCopy(bool sendCopy)
        {
            try
            {
                if (sendCopy)
                {
                    int orderID = 0;
                    orderID = Convert.ToInt32(TempData["orderID"]);
                    PurchaseOrder purchaseOrder = null;

                    purchaseOrder = await _purchaseOrderRepository.GetFirstOrDefaultAsync(o => o.Id == orderID);
                    purchaseOrder.SendInvoice = true;

                    _purchaseOrderRepository.SetModified(purchaseOrder);
                    await _purchaseOrderRepository.UnitOfWork.CommitAsync();

                    return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    return Json(new { success = true, responseText = "" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }


    }


}
