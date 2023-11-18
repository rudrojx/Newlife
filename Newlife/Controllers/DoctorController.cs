using Newlife.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Newlife.Controllers
{
    public class DoctorController : Controller
    {
        NewlifeDBContext db = new NewlifeDBContext();
        // GET: Doctor
        public ActionResult Index()
        {
            var docInCookie = Request.Cookies["DocLogInfo"];
            string id = docInCookie["DoctorID"].ToString();
            string docname = docInCookie["DoctorName"].ToString();
            var getdata = db.Clinicinfo.Where(x => x.DoctorID == id).ToList();
            DoctorDataViewModel mv = new DoctorDataViewModel();
            mv.cliniclist = getdata;
            mv.reviewlists = db.reviewdetails.Where(x => x.DoctorID == id).ToList();
            ViewBag.ClinicTotal = db.Clinicinfo.Where(x => x.DoctorID == id).ToList().Count();
            ViewBag.AppontimentTotal = db.Appointmenlists.Where(x => x.DoctorName == docname).ToList().Count();
            ViewBag.CustomerReview = db.reviewdetails.Where(x => x.DoctorID == id).ToList().Count();
            if (docInCookie != null)
            {
                return View(mv);
            }
            else
            {
                return RedirectToAction("DoctorLogin");
            }

        }

        public ActionResult AppointmentBookingList()
        {
            var docInCookie = Request.Cookies["DocLogInfo"];
            string docname = docInCookie["DoctorName"].ToString();
            var getappointmentlist = db.Appointmenlists.Where(x => x.DoctorName == docname).ToList();
            return View(getappointmentlist);
        }

        [HttpGet]
        public ActionResult AddAppointmentSlot()
        {
            // Get the current year
            int year = DateTime.Now.Year;

            // Create a list to store the dates with days for every month
            Dictionary<string, List<string>> datesWithDaysForMonth = new Dictionary<string, List<string>>();

            // Loop through each month of the year
            for (int month = 1; month <= 12; month++)
            {
                // Get the number of days in the month
                int daysInMonth = DateTime.DaysInMonth(year, month);

                // Create a list to store the dates with days for the current month
                List<string> datesWithDays = new List<string>();

                // Loop through each day of the month
                for (int day = 1; day <= daysInMonth; day++)
                {
                    // Create a DateTime object for the current date
                    DateTime currentDate = new DateTime(year, month, day);

                    // Add the date with day to the list for the current month
                    datesWithDays.Add(currentDate.ToString("MMMM dd, yyyy (dddd)"));
                }

                // Add the list of dates with days for the current month to the dictionary
                datesWithDaysForMonth.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month), datesWithDays);
            }

            // Pass the dictionary of dates with days for every month to the view
            return View(datesWithDaysForMonth);
        }

        [HttpPost]
        public ActionResult AddAppointmentSlot(AppointmentSlots ap)
        {
            var docInCookie = Request.Cookies["DocLogInfo"];
            string id = docInCookie["DoctorID"].ToString();
            string docname = docInCookie["DoctorName"].ToString();
            if(docInCookie != null)
            {
                try
                {
                    ap.DoctorID = id;
                    ap.DoctorName = docname;
                    db.Appslotinfo.Add(ap);
                    db.SaveChanges();
                    TempData["Message"] = "Appointments Details Add Sucessfully !";
                    TempData["Type"] = "success";
                }
                catch(Exception ex)
                {
                   
                    TempData["Message"] = "ERROR:" + ex.Message.ToString();
                    TempData["Type"] = "error";
                }
                return RedirectToAction("AppointmentList");
            }
            else
            {
                return RedirectToAction("DoctorLogin");
            }
            
        }
        public ActionResult DeleteAppointmentslot(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppointmentSlots doc = db.Appslotinfo.Find(id);
            if (doc == null)
            {
                return HttpNotFound();
            }

            db.Entry(doc).State = EntityState.Deleted;
            if (db.SaveChanges() > 0)
            {
                TempData["Message"] = "Deleted Sucessfully !";
                TempData["Type"] = "success";
                return RedirectToAction("AppointmentList", "Doctor");
            }
            else
            {
                TempData["Message"] = "Try again !";
                TempData["Type"] = "error";

            }
            return RedirectToAction("AppointmentList");
        }
        public ActionResult AppointmentList()
        {
            var docInCookie = Request.Cookies["DocLogInfo"];
            string id = docInCookie["DoctorID"].ToString();
            var getappointmentlist = db.Appslotinfo.Where(x=>x.DoctorID == id).ToList();
            return View(getappointmentlist);
        }

        public ActionResult UpdateAppointmentDetails(int id)
        {
            var getdata = db.Appslotinfo.Where(x => x.Id == id).FirstOrDefault();
            return View(getdata);
        }

        [HttpPost]
        public ActionResult UpdateAppointmentDetails(AppointmentSlots model)
        {

            if (ModelState.IsValid)
            {
                var doctor = db.Appslotinfo.Find(model.Id);

                // Update properties from the model
                doctor.DoctorName = model.DoctorName;
                doctor.DoctorID = model.DoctorID;                
                doctor.ClinicName = model.ClinicName;
                doctor.Date = model.Date;
                doctor.Time = model.Time;
                doctor.AvailableSlots = model.AvailableSlots;
                doctor.IsAvailable = model.IsAvailable;
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("AppointmentList");

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteClinic(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clinic_Details doc = db.Clinicinfo.Find(id);
            if (doc == null)
            {
                return HttpNotFound();
            }
           
            db.Entry(doc).State = EntityState.Deleted;
            if (db.SaveChanges() > 0)
            {
                TempData["Message"] = "Deleted Sucessfully !";
                TempData["Type"] = "success";
                return RedirectToAction("ClinicList", "Doctor");
            }
            else
            {
                TempData["Message"] = "Try again !";
                TempData["Type"] = "error";
                
            }
            return RedirectToAction("ClinicList");
        }
        public ActionResult UpdateClinicDetails(int id)
        {
            var getdata = db.Clinicinfo.Where(x => x.Id == id).FirstOrDefault();
            return View(getdata);
        }
        [HttpPost]
        public ActionResult UpdateClinicDetails(Clinic_Details model)
        {
            if (ModelState.IsValid)
            {
                var doctor = db.Clinicinfo.Find(model.Id);

                // Update properties from the model
                doctor.DoctorName = model.DoctorName;
                doctor.DoctorID = model.DoctorID;
                doctor.DoctorSpeciality = model.DoctorSpeciality;
                doctor.Doctor_Availability = model.Doctor_Availability;
                doctor.ClinicFee = model.ClinicFee;
                doctor.Location = model.Location;
                doctor.ClinicName = model.ClinicName;
                doctor.Clinic_Address = model.Clinic_Address;
                doctor.Clinic_Timing = model.Clinic_Timing;
              
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("ClinicList");

            }
            return View(model);
        }


        public ActionResult ClinicList()
        {
            var docInCookie = Request.Cookies["DocLogInfo"];
            string id = docInCookie["DoctorID"].ToString();
            var getdata = db.Clinicinfo.Where(x =>x.DoctorID == id).ToList();
            return View(getdata);           
        }

        [HttpPost]
        public ActionResult DoctorLogin(Doctor_Profile model)
        {
            var data = db.DocProfile.Where(s => s.DoctorID.Equals(model.DoctorID) && s.DoctorPassword.Equals(model.DoctorPassword)).ToList();
            if (data.Count() > 0)
            {
                HttpCookie cooskie = new HttpCookie("DocLogInfo");
                cooskie.Values["DoctorID"] = Convert.ToString(data.FirstOrDefault().DoctorID);
                cooskie.Values["DoctorName"] = Convert.ToString(data.FirstOrDefault().DoctorName);
                cooskie.Expires = DateTime.Now.AddMonths(5);
                Response.Cookies.Add(cooskie);
                TempData["Message"] = "Welcome Back Dr." + data.FirstOrDefault().DoctorName;
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

        public ActionResult LogoutDoctor()
        {
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("DocLogInfo"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["DocLogInfo"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            return RedirectToAction("DoctorLogin");
        }

        public ActionResult DoctorLogin()
        {
            return View();
        }

        public ActionResult AddClinic()
        {
            var docInCookie = Request.Cookies["DocLogInfo"];
            string id = docInCookie["DoctorID"].ToString();
            if (docInCookie != null)
            {
                var getdoc = db.DocDetail.Where(x => x.DoctorID == id).FirstOrDefault();               
                return View(getdoc);
            }
            else
            {
                return RedirectToAction("DoctorLogin");
            }

        }

        [HttpPost]
        public ActionResult AddClinic(Clinic_Details cd)
        {
            var docInCookie = Request.Cookies["DocLogInfo"];
            string id = docInCookie["DoctorID"].ToString();
            var getdoc = db.DocDetail.Where(x => x.DoctorID == id).FirstOrDefault();
            try
            {              
                db.Clinicinfo.Add(cd);
                db.SaveChanges();
                TempData["Message"] = "Clinic Details Add Sucessfully !";
                TempData["Type"] = "success";
            }
            catch (Exception ex)
            {
                ViewBag.Message = "ERROR:" + ex.Message.ToString();
                TempData["Message"] = "Error Try Again !";
                TempData["Type"] = "error";
            }
            return View(getdoc);
        }
    }
}