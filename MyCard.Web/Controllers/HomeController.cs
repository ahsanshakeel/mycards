using Microsoft.Graph;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using MyCard.Web.Models;
using MyCard.Web.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using MyCard.Web.Resources;
using System.Linq.Expressions;
using MyCard.Domain;
using System.Threading;
using System.Web.Security;
using System.Security.Claims;
using MyCard.Web.TokenStorage;
using Microsoft.Owin.Security.Cookies;
using System.Security.Cryptography;
using System.Text;

namespace MyCard.Web.Controllers
{
    
    public class HomeController : Controller
    {
        private IUserRepository _userRepository;
        private ICompanyRepository _companyRepository;
        private IPurchaseOrderRepository _purchaseOrderRepository;
        private ICityRepository _cityRepository;
        private IIndustryRepository _industryRepository;
        private ILicenseDescriptionRepository _licenseDescriptionRepository;
        GraphService graphService = new GraphService();

        public HomeController( IUserRepository userRepository, IPurchaseOrderRepository purchaseOrderRepository, ICityRepository cityRepository, IIndustryRepository industryRepository, ICompanyRepository companyRepository, ILicenseDescriptionRepository licenseDescriptionRepository)
        {
            if (userRepository == (IUserRepository)null)
                throw new ArgumentNullException("userRepository");
            _userRepository = userRepository;



            if (purchaseOrderRepository == (ICompanyRepository)null)
                throw new ArgumentNullException("purchaseOrderRepository");
            _purchaseOrderRepository = purchaseOrderRepository;

            if (companyRepository == (ICompanyRepository)null)
                throw new ArgumentNullException("companyRepository");
            _companyRepository = companyRepository;

            if (cityRepository == (ICityRepository)null)
                throw new ArgumentNullException("cityRepository");
            _cityRepository = cityRepository;

            if (industryRepository == (IIndustryRepository)null)
                throw new ArgumentNullException("industryRepository");
            _industryRepository = industryRepository;

            if (licenseDescriptionRepository == (ILicenseDescriptionRepository)null)
                throw new ArgumentNullException("licenseDescriptionRepository");
            _licenseDescriptionRepository = licenseDescriptionRepository;
        }
        // GET: Home
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("MSOLoginRedirect", "Home");
            }
            else
            {
                return RedirectToAction("Login", "CompanyDashboard");
            }
            
        }
       
        public ActionResult authresponse()
        {
            return View();
        }
        // GET: Microsoft Office 365 Login
        [AllowAnonymous]
        public void MSOLogin()
        {
            this.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

            //if (!Request.IsAuthenticated)
            //{
            // Signal OWIN to send an authorization request to Azure.
            HttpContext.GetOwinContext().Authentication.Challenge(
                  new AuthenticationProperties { RedirectUri = "/" },
                  OpenIdConnectAuthenticationDefaults.AuthenticationType);
            //}
        }

        //public async Task<ActionResult> RegisterUser()
        //{

        //    return RedirectToAction("", "");
        //}

        [AllowAnonymous]
        public async Task<ActionResult> MSOLoginRedirect()
        {
            string encryptedCompanyID = string.Empty;
            string encodedCompanyID = string.Empty;

            string rolename = string.Empty;

            //GenerateRandomCryptographicKey cmsKey
            string cmsKey = string.Empty;
            cmsKey = GenerateRandomKey(12);

            try
            {

                GraphServiceClient graphClient = SDKHelper.GetAuthenticatedClient();

                string useremail = await graphService.GetMyEmailAddress(graphClient);

                MyCard.Domain.User user = null;

                MyCard.Domain.Company company= null;
                company = await _companyRepository.GetFirstOrDefaultAsync(o => o.Email == useremail);

                Expression<Func<MyCard.Domain.User, object>>[] includes =
                {
                        x => x.UserRoles
                };

                if (company != null)
                {
                    user = await _userRepository.GetFirstOrDefaultAsync(o => o.Email == useremail, default(CancellationToken), includes);

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

                    if (rolename == "company_owner")
                    {
                        
                        var authTicket = new FormsAuthenticationTicket(
                                                      1,
                                                      user.Name + "|" + user.Id + "|" + user.CompanyId + "|" + encodedCompanyID + "|" + cmsKey,  //user id
                                                      DateTime.Now,
                                                      DateTime.Now.AddDays(3),  // expiry
                                                      false,  //true to remember
                                                      rolename, //roles 
                                                      "/"
                                                    );

                        //encrypt the ticket and add it to a cookie
                        HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                        Response.Cookies.Add(cookie);

                        //if(company.CustomerType=="Freemium")
                        //{
                        //    return RedirectToAction("Index", "CompanyDashboard");
                        //}

                        //Update Cms_key here
                        company = await _companyRepository.GetFirstOrDefaultAsync(o => o.Id == user.CompanyId);

                        company.CmsAccessKey = cmsKey;


                        _companyRepository.SetModified(company);
                        await _companyRepository.UnitOfWork.CommitAsync();

                        PurchaseOrder poUpload = _purchaseOrderRepository.GetFilteredElements(x => x.CompanyId == user.CompanyId).OrderByDescending(x => x.Id).FirstOrDefault();

                        if (poUpload != null)
                        {
                            if (poUpload.OrderFile == null || poUpload.FileName == null)
                            {
                                return RedirectToAction("UploadPO", "CompanyDashboard");
                            }
                            else
                            {
                                return RedirectToAction("Index", "CompanyDashboard");
                            }

                        }
                        else
                        {
                            return RedirectToAction("Purchase", "CompanyDashboard");
                        }
                        
                    }
                    else
                    {
                        //ViewBag.Message = MyCard.Web.Resources.ErrorMessages.UserNamePasswordNotMatch;
                        return RedirectToAction("Login", "CompanyDashboard");
                    }
                    
                }
                else
                {
                    //Get Organisation Data
                    //IGraphServiceOrganizationCollectionPage organization = await graphClient.Organization.Request().GetAsync();

                    GraphServiceClient graphClient1 = SDKHelper.GetAuthenticatedClient();
                    IGraphServiceOrganizationCollectionPage organization = await graphService.GetOrganisationInfo(graphClient1);

                    if (organization.Count == 0)
                    {
                        return View("~/Views/Shared/GoToLogin.cshtml");
                    }

                    //Commenting Below as the Freemium is now being disabled.
                    //Adding new requirement of Freemium and Premium
                    //return RedirectToAction("CompanyLicense", "Home");


                    GraphServiceClient graphClient2 = SDKHelper.GetAuthenticatedClient();

                    // Get the current user's email address. 
                    //Microsoft.Graph.User me = await graphClient.Me.Request().Select("mail, userPrincipalName, displayName, jobTitle").GetAsync();
                    Microsoft.Graph.User me = await graphService.GetUserInfo(graphClient2);

                    //me.Mail ?? me.UserPrincipalName;
                    string email = me.Mail ?? me.UserPrincipalName;
                    string displayName = me.DisplayName;
                    string companyName = organization.CurrentPage[0].DisplayName;
                    string website = organization.CurrentPage[0].VerifiedDomains.ElementAt(0).Name; //me.MySite;
                    string jobTitle = me.JobTitle;

                        RegistrationViewModel model = new RegistrationViewModel();
                        model.EmailID = email;
                        model.CustomerName = displayName;
                        model.DomainName = website;
                        model.Name = companyName;
                        model.BusinessTitle = jobTitle;

                        ViewBag.PageType = "Register";

                        return View("~/Views/Registration/Register.cshtml", model);                    
                }
                
            }
            catch
            {
                return RedirectToAction("Login", "CompanyDashboard");
            }


        }

        public ActionResult CMSRegister()
        {
            RegistrationViewModel model = new RegistrationViewModel();
            model.EmailID = "";
            model.CustomerName = "";
            model.DomainName = "mydomain.com";
            model.Name = "";
            model.BusinessTitle = "";
            model.AccountType = 2;

            ViewBag.PageType = "Register";

            return View("~/Views/Registration/CMSRegister.cshtml", model);
        }

        public ActionResult CompanyLicense()
        {
            CompanyLicenseViewModel model = new CompanyLicenseViewModel();


            LicenseDescriptionModel ldModel = null;
            IList<LicenseDescriptionModel> modelList = new List<LicenseDescriptionModel>();

            IList<LicenseDescription> licenseDescriptions = _licenseDescriptionRepository.GetAllElements().ToList();

            foreach(LicenseDescription item in licenseDescriptions)
            {
                ldModel = new LicenseDescriptionModel();
                ldModel.LicenseName = item.LicenseName;
                ldModel.Description = item.Description;
                modelList.Add(ldModel);
            }

            model.CompanyLicenseType = "Freemium";
            model.LicenseDescriptions = modelList;

            ViewBag.PageType = "Register";
            return View(model);
            
        }


        //public async Task<ActionResult> GoToRegister(CompanyLicenseViewModel cModel)
        public async Task<ActionResult> GoToRegister()
        {
            GraphServiceClient graphClient1 = SDKHelper.GetAuthenticatedClient();
            IGraphServiceOrganizationCollectionPage organization = await graphService.GetOrganisationInfo(graphClient1);

            if (organization.Count == 0)
            {
                return View("~/Views/Shared/GoToLogin.cshtml");
            }

            GraphServiceClient graphClient2 = SDKHelper.GetAuthenticatedClient();

            // Get the current user's email address. 
            //Microsoft.Graph.User me = await graphClient.Me.Request().Select("mail, userPrincipalName, displayName, jobTitle").GetAsync();
            Microsoft.Graph.User me = await graphService.GetUserInfo(graphClient2);

            //me.Mail ?? me.UserPrincipalName;
            string email = me.Mail ?? me.UserPrincipalName;
            string displayName = me.DisplayName;
            string companyName = organization.CurrentPage[0].DisplayName;
            string website = organization.CurrentPage[0].VerifiedDomains.ElementAt(0).Name; //me.MySite;
            string jobTitle = me.JobTitle;

            RegistrationViewModel model = new RegistrationViewModel();
            model.EmailID = email;
            model.CustomerName = displayName;
            model.DomainName = website;
            model.Name = companyName;
            model.BusinessTitle = jobTitle;
            //model.CustomerType = cModel.CompanyLicenseType;
            model.CustomerType = "Premium";

            ViewBag.PageType = "Register";

            //ViewBag.citieslist = _cityRepository.GetAllElements().Select(x => new SelectListItem
            //{
            //    Value = x.CityName,
            //    Text = x.CityName
            //});


            ViewBag.countrieslist = _cityRepository.GetAllElements().Select(x=> new { x.CountryName }).OrderBy(x=>x.CountryName).ToList().Distinct().Select(x => new SelectListItem
            {
                Value = x.CountryName,
                Text = x.CountryName
            });

            ViewBag.industrieslist = _industryRepository.GetAllElements().OrderBy(x=>x.IndustryName).ToList().Select(x => new SelectListItem
            {
                Value = x.IndustryName,
                Text = x.IndustryName
            });

            return View("~/Views/Registration/Register.cshtml", model);



        }


        public void SignOut()
        {
            if (Request.IsAuthenticated)
            {
                // Get the user's token cache and clear it.
                string userObjectId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;

                SessionTokenCache tokenCache = new SessionTokenCache(userObjectId, HttpContext);
                HttpContext.GetOwinContext().Authentication.SignOut(OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
            }

            // Send an OpenID Connect sign-out request. 
            HttpContext.GetOwinContext().Authentication.SignOut(
              CookieAuthenticationDefaults.AuthenticationType);
            //Response.Redirect("/");
        }

        //Randomn Key Generation
        public string GenerateRandomCryptographicKey(int keyLength)
        {
            RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[keyLength];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
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
    }
}