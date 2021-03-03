
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.Security.Principal;
using Quartz;
using MyCard.Web.Models;

namespace MyCard.Web
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            GlobalFilters.Filters.Add(new RequireHttpsAttribute());
            JobScheduler.Start();
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
           
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie =
                Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket =
                                            FormsAuthentication.Decrypt(authCookie.Value);

                string[] roles = authTicket.UserData.Split(new Char[] { ',' });

                string UserId = authTicket.Name.Split('|')[1];

                GenericPrincipal userPrincipal =
                                    new GenericPrincipal(new GenericIdentity(authTicket.Name), roles);

                if (authTicket.UserData.Contains("super_admin"))
                {                      
                    Context.User = userPrincipal;
                    return;
                }
                
                string cmskey = authTicket.Name.Split('|')[4];

                List<LoggedUser> listUsers = (List<LoggedUser>) Application["LoggedUser"];
                if(listUsers== null)
                {
                    listUsers = new List<LoggedUser>();
                }

                var user = listUsers.Find(k => k.UserId == UserId);
                if (user == null)
                {
                    user = new LoggedUser() { UserId = UserId, LastAccessTime = DateTime.Now, CMSKey = cmskey };
                    listUsers.Add(user);
                    Application["LoggedUser"] = listUsers;
                }
                user.LastAccessTime = DateTime.Now;
                                
                Context.User = userPrincipal;

                if (cmskey != user.CMSKey)
                {
                    HttpContext.Current.RewritePath("/CompanyDashboard/Logout");               
                }
            }
        }
        
        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("X-Powered-By");
        }

        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }

        //protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        //{
        //    if (FormsAuthentication.CookiesSupported == true)
        //    {
        //        if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
        //        {
        //            try
        //            {
        //                //let us take out the username now                
        //                string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
        //                string roles = string.Empty;
        //                int UserID = Convert.ToInt32(username.Split('|')[1]);

        //                //using (userDbEntities entities = new userDbEntities())
        //                //{
        //                //    User user = entities.Users.SingleOrDefault(u => u.username == username);

        //                //    roles = user.Roles;
        //                //}
        //                //let us extract the roles from our own custom cookie
        //                //User user = null;

        //                User user = _userRepository.GetFilteredElements(o => o.Id == UserID).FirstOrDefault();//.UserRoles.Select(u=>u.Role).Any(r=>r.rol);

        //                //roles = user.UserRoles == null ? new string[] { } :  user.UserRoles.Select(u => u.Role).Select(u => u.Name).ToArray();
        //                roles = (user.UserRoles == null) ? string.Empty: user.UserRoles.Select(u => u.Role).Select(u => u.Name).FirstOrDefault();
        //                //roles = "super_admin";

        //                //Let us set the Pricipal with our user specific details
        //                HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(
        //                  new System.Security.Principal.GenericIdentity(username, "Forms"), roles.Split(';'));
        //            }
        //            catch (Exception)
        //            {
        //                //somehting went wrong
        //            }
        //        }
        //    }
        //}
    }

}