using Newlife.Filters;
using Newlife.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
namespace Newlife.Controllers
{
    public class AdminController : Controller
    {
        NewlifeDBContext db = new NewlifeDBContext();
        // GET: Admin
        public ActionResult Index()
        {
            var adminInCookie = Request.Cookies["AdminInfo"];
           // var doclist = db.DocDetail.ToList();
            DoctorDataViewModel mv = new DoctorDataViewModel();
            mv.docdetails = db.DocDetail.OrderBy(i => Guid.NewGuid()).Take(4).ToList();
            ViewBag.Doctotal = db.DocDetail.ToList().Count();
            ViewBag.Usertotal = db.Userinfo.ToList().Count();
            ViewBag.Reviewtotal = db.reviewdetails.ToList().Count();
            mv.userDetails = db.Userinfo.ToList();
            if (adminInCookie != null)
            {
                return View(mv);
            }
            else
            {
                return RedirectToAction("AdminLogin");
            }

        }

        [HttpPost]        
        public ActionResult AdminLogin(AdminDetails model)
        {
            var data = db.Admininfo.Where(s => s.Username.Equals(model.Username) && s.Password.Equals(model.Password) && s.Status.Equals("1")).ToList();
            if (data.Count() > 0)
            {
                HttpCookie cooskie = new HttpCookie("AdminInfo");
                cooskie.Values["AdminID"] = Convert.ToString(data.FirstOrDefault().Id);
                cooskie.Values["AdminEmail"] = Convert.ToString(data.FirstOrDefault().Username);
                cooskie.Expires = DateTime.Now.AddMonths(5);
                Response.Cookies.Add(cooskie);
                TempData["Message"] = "Welcome Back Mr."+data.FirstOrDefault().Username;
                TempData["Type"] = "success";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = "Invalid Login Details Try Again !";
                TempData["Type"] = "error";
                return View();
            }
           
        }

        public ActionResult LogoutAdmin()
        {
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("AdminInfo"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["AdminInfo"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            return RedirectToAction("AdminLogin");
        }

        public ActionResult AdminLogin()
        {
            return View();
        }
        public ActionResult AddAdmin()
        {
            return View();
        }
        public string GenerateRandomPassword(int length)
        {
            const string allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()_-+=[{]};:<>|./?";
            var randNum = new Random();
            var password = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                password.Append(allowedChars[randNum.Next(0, allowedChars.Length)]);
            }

            return password.ToString();
        }
        public ActionResult AddDoctors()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddDoctors(HttpPostedFileBase file,DoctorDetails doc)
        {
            
                if (file != null && file.ContentLength > 0)
                {
                    try
                    {
                        string path = Path.Combine(Server.MapPath("~/Content/Images"),
                                                   Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        string filename = file.FileName;
                        ViewBag.Message = "File uploaded successfully";
                        doc.ProfilePicture = "Content/Images/" + filename;
                        db.DocDetail.Add(doc);
                        db.SaveChanges();

                        Doctor_Profile docpro = new Doctor_Profile();
                        var docpassword = GenerateRandomPassword(8);
                         docpro.DoctorID = doc.DoctorID;
                        docpro.DoctorName = doc.DoctorName;
                        docpro.DoctorPassword = docpassword;
                        db.DocProfile.Add(docpro);
                        db.SaveChanges();
                        TempData["Message"] = "Doctor Details Add Sucessfully !";
                        TempData["Type"] = "success";
                }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR:" + ex.Message.ToString();
                        TempData["Message"] = "Error Try Again !";
                        TempData["Type"] = "error";
                }
                }
                else
                {
                try
                {                   
                    db.DocDetail.Add(doc);
                    db.SaveChanges();
                    Doctor_Profile docpro = new Doctor_Profile();
                    var docpassword = GenerateRandomPassword(8);
                    docpro.DoctorID = doc.DoctorID;
                    docpro.DoctorName = doc.DoctorName;
                    docpro.DoctorPassword = docpassword;
                    db.DocProfile.Add(docpro);
                    db.SaveChanges();
                    TempData["Message"] = "Doctor Details Add Sucessfully !";
                    TempData["Type"] = "success";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                    TempData["Message"] = "Error Try Again !";
                    TempData["Type"] = "error";
                }
               
                }
            
            
            return View();
        }
        [HttpGet]
        public ActionResult DeleteDoc(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorDetails doc = db.DocDetail.Find(id);
            if (doc == null)
            {
                return HttpNotFound();
            }
            string currentimg = Request.MapPath("~/" + doc.ProfilePicture);
            db.Entry(doc).State = EntityState.Deleted;
            if (db.SaveChanges() > 0)
            {
                if (System.IO.File.Exists(currentimg))
                {
                    System.IO.File.Delete(currentimg);
                }
                TempData["sucessmsg"] = "Deleted Sucessfully Done !";
                return RedirectToAction("DoctorList", "Admin");
            }
            return View();
        }

        public ActionResult UpdateDoctorDetails(int id)
        {
            var getdata = db.DocDetail.Where(x => x.Id == id).FirstOrDefault();
            TempData["imgpath"] = getdata.ProfilePicture;
            return View(getdata);
        }


        [HttpPost]
        public ActionResult UpdateDoctorDetails(HttpPostedFileBase file, DoctorDetails model)
        {
            if (ModelState.IsValid)
            {
                var doctor = db.DocDetail.Find(model.Id);

                // Update properties from the model
                doctor.DoctorName = model.DoctorName;
                doctor.DoctorID = model.DoctorID;
                doctor.Specialization = model.Specialization;
                doctor.Email = model.Email;
                doctor.Bio = model.Bio;
                doctor.Location = model.Location;
                doctor.Education = model.Education;
                doctor.Clinic_Charge = model.Clinic_Charge;
                doctor.Phone = model.Phone;
                doctor.Experience = model.Experience;
                doctor.Active = model.Active;
                // Save the profile picture if there is one
                if (file != null && file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), fileName);
                    file.SaveAs(path);
                    doctor.ProfilePicture = "Content/Images/" + fileName;
                }

                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("DoctorList");

            }          

            return View(model);
        }

        public ActionResult DoctorList()
        {
            var getdata = db.DocDetail.ToList().OrderByDescending(x => x.Id);
            return View(getdata);
        }       

        // POST: Admin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Admin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Admin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
