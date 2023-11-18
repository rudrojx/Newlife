using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Newlife.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Octokit;
using Microsoft.Owin.Security.Cookies;

namespace Newlife.Controllers
{
    public class GitHubController : Controller
    {
        NewlifeDBContext db = new NewlifeDBContext();
        private const string GitHubClientId = "";
        private const string GitHubClientSecret = "";

        // GET: GitHub
        public ActionResult SignIn()
        {
            var redirectUrl = Url.Action("GitHubCallbackAsync", "GitHub", null, Request.Url.Scheme);
            var githubAuthUrl = $"https://github.com/login/oauth/authorize?client_id={GitHubClientId}&redirect_uri={redirectUrl}&scope=user";

            return Redirect(githubAuthUrl);
        }

        public async Task<ActionResult> GitHubCallbackAsync(string code)
        {
            var github = new GitHubClient(new ProductHeaderValue("Newlife"));
            var token = await github.Oauth.CreateAccessToken(new OauthTokenRequest(GitHubClientId, GitHubClientSecret, code));
            github.Credentials = new Credentials(token.AccessToken);
          
            var user = await github.User.Current();
            string username = user.Name;
            string usermail = user.Email;
            string uniqueid = Convert.ToString(user.Id);
            var isIDAlreadyExists = db.Userinfo.Any(x => x.PictureUrl == uniqueid);
            if (!isIDAlreadyExists)
            {
                var users = new patient_profile
                {
                    First_Name = username,
                    Email = usermail,
                    PictureUrl = uniqueid,

                };
                db.Userinfo.Add(users);
                db.SaveChanges();
            }
            var credentails = db.Userinfo.Where(model => model.PictureUrl == uniqueid).FirstOrDefault();
            HttpCookie cooskie = new HttpCookie("UserInfo");
            cooskie.Values["Email"] = username;
            cooskie.Values["idUser"] = Convert.ToString(credentails.id);
           // cooskie.Values["UserPic"] = picurl;
            cooskie.Expires = DateTime.Now.AddMonths(2);
            Response.Cookies.Add(cooskie);
            

            return RedirectToAction("Index", "Home");


        }         

     
    }
}