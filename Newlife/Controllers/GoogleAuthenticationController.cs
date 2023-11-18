using Newlife.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Newlife.Controllers
{
    public class GoogleAuthenticationController : Controller
    {
        NewlifeDBContext _db = new NewlifeDBContext();
        // GET: GoogleAuthentication
        public ActionResult SignUp()
        {
            var redirectUrl = Url.Action("Callback", "GoogleAuthentication", null, protocol: Request.Url.Scheme);
            return Redirect(GetAuthorizationUrl(redirectUrl));
        }

        public ActionResult Callback(string code)

        {
            var redirectUrl = Url.Action("Callback", "GoogleAuthentication", null, protocol: Request.Url.Scheme);
            var tokenResponse = GetAccessToken(code, redirectUrl);

            if (tokenResponse != null && !string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                var userInfo = GetUserInfo(tokenResponse.AccessToken);
              

                string getmailid = userInfo.email;
                var isEmailAlreadyExists = _db.Userinfo.Any(x => x.Email == getmailid);
                if (!isEmailAlreadyExists)
                {
                    var user = new patient_profile
                    {
                        First_Name = userInfo.given_name,
                        Last_Name = userInfo.family_name,
                        Email = userInfo.email,
                        PictureUrl = userInfo.picture
                    };
                    _db.Userinfo.Add(user);
                    _db.SaveChanges();
                    SendSignUpVerificationEmail(getmailid);

                }
                var userName = userInfo.given_name;
                var picurl = userInfo.picture;
                var credentails = _db.Userinfo.Where(model => model.Email == getmailid).FirstOrDefault();
                HttpCookie cooskie = new HttpCookie("UserInfo");
                cooskie.Values["Email"] = Convert.ToString(userName);
                cooskie.Values["idUser"] = Convert.ToString(credentails.id);
                cooskie.Values["UserPic"] = picurl;
                cooskie.Expires = DateTime.Now.AddMonths(2);
                Response.Cookies.Add(cooskie);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index","Home");
            }
        }

        private dynamic GetUserInfo(string accessToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://www.googleapis.com/oauth2/v2/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = client.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                dynamic userInfo = JsonConvert.DeserializeObject(content);
                return userInfo;
            }
            else
            {
                throw new Exception("Failed to retrieve user information.");
            }
        }

        private string GetAuthorizationUrl(string redirectUrl)
        {
            var state = Guid.NewGuid().ToString("N");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["response_type"] = "code";
            queryString["client_id"] = "";
            queryString["redirect_uri"] = redirectUrl;
            queryString["scope"] = "openid email profile";
            queryString["state"] = state;

            var authorizationEndpoint = "https://accounts.google.com/o/oauth2/v2/auth";

            return string.Format("{0}?{1}", authorizationEndpoint, queryString);
        }
        private TokenResponse GetAccessToken(string code, string redirectUrl)
        {
            var tokenEndpoint = "https://www.googleapis.com/oauth2/v4/token";

            var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

            request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        { "code", code },
        { "client_id", "" },
        { "client_secret", "" },
        { "redirect_uri", redirectUrl },
        { "grant_type", "authorization_code" }
    });

            var response = client.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<TokenResponse>(responseContent);
            }

            return null;
        }

        public ActionResult Logout()
        {
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("UserInfo"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["UserInfo"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }


        [NonAction]
        public void SendSignUpVerificationEmail(string emailid)
        {
            var fromEmail = new MailAddress("@gmail.com", "Newlife");
            var toEmail = new MailAddress(emailid);
            var fromEmailPassword = ""; // Replace with actual password

            string subject = "";
            string body = "";

            subject = "Your account is successfully created!";
            body = "<br/>We are excited to tell you that your Account Created Successfully ! <br/>" +
                " Connected With Our Family and Enjoy Our Hospitality at your Door Step <br/>" +
                "Stay Safe & Healthy <br/>" +
                ". Thank From Newlife" +
                " <br/>";
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);

        }

        private class TokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }

            [JsonProperty("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonProperty("token_type")]
            public string TokenType { get; set; }

            [JsonProperty("refresh_token")]
            public string RefreshToken { get; set; }

            [JsonProperty("id_token")]
            public string IdToken { get; set; }
        }
        private class UserInfo
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("given_name")]
            public string FirstName { get; set; }

            [JsonProperty("family_name")]
            public string LastName { get; set; }

            [JsonProperty("picture")]
            public string PictureUrl { get; set; }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Index");
            }
        }


        public ActionResult MyAction()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Index");
            // Add your code here to handle the authorized user
        }

    }
}