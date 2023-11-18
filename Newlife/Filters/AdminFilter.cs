using Newlife.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Newlife.Filters
{
    public class AdminFilter : AuthorizeAttribute 
    {
        NewlifeDBContext db = new NewlifeDBContext();
      

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            // Check if the user is authenticated and has the necessary role or permission
            if (!httpContext.User.Identity.IsAuthenticated || !IsAdmin(httpContext.User.Identity.Name))
            {
                // Redirect the user to the login page or show an unauthorized error
                httpContext.Response.StatusCode = 401; // Unauthorized

                return false;
            }

            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Controller.TempData["Message"] = "Invalid Login Details Try Again !";
            filterContext.Controller.TempData["Type"] = "error";
            // Redirect the user to the login page or show an unauthorized error
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                { "controller", "Admin" }, // Replace "Account" with your login controller name
                { "action", "AdminLogin" } // Replace "Login" with your login action name

                });

        }

        private bool IsAdmin(string username)
        {
            // Implement your logic to check if the user is an admin based on their username or any other criteria
            // You can use your existing database query or any other method to determine admin status
            var data = db.Admininfo.FirstOrDefault(s => s.Username == username && s.Status == "1");
            return data != null;
        }
    }
}