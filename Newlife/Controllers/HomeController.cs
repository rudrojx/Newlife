using Newlife.Filters;
using Newlife.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Newlife.Controllers
{
    
    public class HomeController : Controller
    {
        NewlifeDBContext db = new NewlifeDBContext();
       // DoctorDataViewModel db2 = new DoctorDataViewModel();


        public ActionResult Index()
        {
            
                return View();
            
          
            
        }
        public bool UserCheck()
        {            
            var usersInCookie = Request.Cookies["UserInfo"];            
            if(usersInCookie == null)
            {              
                return false;
            }
            else
            {
                return true;
            }
        }
        public ActionResult ConfirmationCode()
        {
            return View();
        }

        public ActionResult Error_404()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DoctorReview(Review_Details model)
        {
            var usersInCookie = Request.Cookies["UserInfo"];
            if (usersInCookie == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    var pid = usersInCookie["idUser"].ToString();
                    model.PatientID = pid;
                    db.reviewdetails.Add(model);
                    if(db.SaveChanges() > 0)
                    {
                        TempData["Message"] = "Thank u for your valuable feedback";
                        TempData["Type"] = "success";
                        return RedirectToAction("DocView", new { id = model.DoctorID });
                    }
                    else
                    {
                        TempData["Message"] = "Error Saving Data Try Again";
                        TempData["Type"] = "error";
                        return RedirectToAction("DocView", new { id = model.DoctorID });
                    }
                }catch(Exception ex)
                {
                    TempData["Message"] = ex;
                    TempData["Type"] = "error";
                    return RedirectToAction("DocView", new { id = model.DoctorID });
                }
            }
                
        }
        public JsonResult GetTimesForDate(string date)
        {          

            // Get the times for the selected date from your data source
            // For example, you could query your database using Entity Framework
            var times = db.Appslotinfo.Where(t => t.Date == date).ToList();

            // Convert the times to a list of strings
            var timeStrings = times.Select(t => t.Time).ToList();
            var checkbooking = times.Any(x => x.AvailableSlots > 0 && x.IsAvailable == "1");

            if (checkbooking)
            {
                // Enable the book now button
                ViewBag.ButtonText = "Book Appointment";
                ViewBag.ButtonDisabled = false;
            }
            else
            {
                // Disable the book now button
                ViewBag.ButtonText = "Full Booked";
                ViewBag.ButtonDisabled = true;
            }


            // Return the times as JSON
            return Json(timeStrings, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Getclinicinfos(string clinicna)
        {
            var getdata = db.Clinicinfo.Where(x => x.ClinicName == clinicna).ToList();
            var getclinicinfo = db.Appslotinfo.Where(x => x.ClinicName == clinicna).ToList();
            var clinicloc = getdata.Select(x => x.Location).ToList();
            ViewBag.loc = clinicloc;
            var getclinicdates = getclinicinfo.Select(x => x.Date).ToList();            
            return Json(getclinicdates, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Getclinicinfosbyid(int id)
        {
            var getdata = db.Clinicinfo.Where(x => x.Id == id).FirstOrDefault();
            var getclinicinfo = db.Appslotinfo.Where(x => x.ClinicName == getdata.ClinicName).ToList();
            ViewBag.loc = getdata.Location;
            ViewBag.Clinicname = getdata.ClinicName;
            var getclinicdates = getclinicinfo.Select(x => x.Date).ToList();
            // Get the current date as a DateTime object
            string currentDate = DateTime.Today.ToString("MMMM dd, yyyy (dddd)");

            // Filter the dates that are equal to or greater than the current date
            var getdates = db.Appslotinfo.Where(x => x.DoctorID == getdata.DoctorID && x.ClinicName == getdata.ClinicName && string.Compare(x.Date, currentDate) >= 0).ToList();
            ViewBag.getdate = getdates;
            var checkbooking = getdates.Any(x => x.AvailableSlots > 0 && x.IsAvailable == "1");

            if (checkbooking)
            {
                // Enable the book now button
                ViewBag.ButtonText = "Book Appointment";
                ViewBag.ButtonDisabled = false;
            }
            else
            {
                // Disable the book now button
                ViewBag.ButtonText = "Full Booked";
                ViewBag.ButtonDisabled = true;
            }
            return Json(new
            {
                Location = ViewBag.loc,
                ClinicName = ViewBag.Clinicname,
                ClinicDates = getclinicdates,
                ButtonTexts = ViewBag.ButtonText,
                ButtonDisables = ViewBag.ButtonDisabled
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SavePatientAppointment(AppointmentDetails data)
        {
            if (data != null)
            {
                // Store the form data in TempData or ViewBag
                TempData["PatientBookingInfo"] = data;
                string mail = data.Email;
                // Redirect to the OTP page
                ConfirmationMail(mail);
                TempData["Message"] = "Confirmation Code Mail Is Sent ";
                TempData["Type"] = "success";
                return RedirectToAction("ConfirmationCode");
            }
            else
            {
                // Display an error message
                TempData["Message"] = "Invalid Details Try Again !";
                TempData["Type"] = "error";
                return RedirectToAction("Doctor");
            }
            
        }

        [NonAction]
        public void ConfirmationMail(string emailid)
        {
            // Generate a random 6-digit OTP
            Random random = new Random();
            int otp = random.Next(100000, 999999);

            // Save the OTP to the session so it can be verified later
            Session["OTP"] = otp;

            var fromEmail = new MailAddress("@gmail.com", "Newlife");
            var toEmail = new MailAddress(emailid);
            var fromEmailPassword = ""; // Replace with actual password

            string subject = "";
            string body = "";

            subject = "Confirmation Mail ";
            body = otp+" is your confirmation code for appointment booking <br/>" +
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

        [HttpPost]
        public ActionResult ConfirmationCode(string ConfirmationCode)
        {

            // Retrieve the form data from TempData or ViewBag
            var model = TempData["PatientBookingInfo"] as AppointmentDetails;
            // Check if the OTP is valid
            if (ConfirmationCode != null)
                {
                    if (ConfirmationCode == Session["OTP"].ToString())
                    {
                        // Retrieve the appointment slot based on the doctor ID, date, and time
                        var appointmentSlot = db.Appslotinfo.Where(a => a.DoctorName == model.DoctorName && a.Date == model.Date && a.Time == model.Time).FirstOrDefault();

                        if (appointmentSlot == null)
                        {
                            // Slot not found in database, redirect back to booking page with error message
                            TempData["Message"] = "Invalid appointment slot";
                            TempData["Type"] = "error";
                            return View();
                        }

                        if (appointmentSlot.IsAvailable == "0" || appointmentSlot.AvailableSlots == 0)
                        {
                            // Slot is not available, redirect back to booking page with error message
                            TempData["Message"] = "Appointment slot is already booked or not available";
                            TempData["Type"] = "error";
                            return View();
                    }


                        string otp = Session["OTP"].ToString();
                        string bookingstatus = "CONFIRMED";
                        string mail = model.Email;
                        // Create a new Appointment object with the details submitted in the form
                        var appointment = new AppointmentDetails
                        {
                            PatientName = model.PatientName,
                            Email = model.Email,
                            DoctorName = model.DoctorName,
                            ClinicName = model.ClinicName,
                            ClinicLocation = model.ClinicLocation,
                            Date = model.Date,
                            Time = model.Time,
                            BookingStatus = bookingstatus,
                            ConfirmationCode = otp
                        };

                        // Save the new appointment to the database
                        db.Appointmenlists.Add(appointment);
                        db.SaveChanges();
                        Session["OTP"] = null;
                        // Update the appointment slot's AvailableSlots and IsAvailable fields in the database
                        appointmentSlot.AvailableSlots--;
                        if (appointmentSlot.AvailableSlots == 0) { appointmentSlot.IsAvailable = "0"; }
                        db.SaveChanges();
                        int getlastid = appointment.Id;
                        BookingConfirmationMail(mail, getlastid);
                        // Redirect to a confirmation page with the appointment details
                        return RedirectToAction("AppointmentBookingConfirmation", new { id = appointment.Id });
                    }
                    else
                    {
                        // Display an error message
                        TempData["Message"] = "Invalid OTP. Please try again.!";
                        TempData["Type"] = "error";
                        return RedirectToAction("ConfirmationCode");
                    }
                }
                else
                {
                    return RedirectToAction("Doctor");
                }           
           
           
        }        


        public ActionResult DocView(string id)
        {
          
            if (id != null)
            {
                var getdocid = db.DocDetail.Where(x => x.DoctorID == id).FirstOrDefault();
                var getclinic = db.Clinicinfo.Where(x => x.DoctorID == id).ToList();
                var getreviews = db.reviewdetails.Where(x => x.DoctorID == id).OrderByDescending(x=>x.Id).ToList();
                return View(new DoctorDataViewModel
                {
                    Doctorinfo = getdocid,
                    cliniclist = getclinic,
                    reviewlists = getreviews
                });
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        
        [NonAction]
        public void BookingConfirmationMail(string emailid, int id)
        {
            var model = db.Appointmenlists.Where(i => i.Id == id).FirstOrDefault();

            var fromEmail = new MailAddress("@gmail.com", "Newlife");
            var toEmail = new MailAddress(emailid);
            var fromEmailPassword = ""; // Replace with actual password

            string subject = "";
            string body = "";

            subject = "Appointment Booking Confirmation Mail ";
            body = " APPOINTMENT "+ model.BookingStatus +" Your appointment with Dr."+model.DoctorName + " at "+model.ClinicName +","+ model.ClinicLocation + " for " + model.Date +","+ model.Time +"is "+ model.BookingStatus  +
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
        public ActionResult AppointmentBookingConfirmation(int id)
        {
            var data = db.Appointmenlists.Where(x => x.Id == id).FirstOrDefault();
            return View(data);
        }

        public ActionResult ClinicView(int id)
        {
            if(id == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                var getclinic = db.Clinicinfo.Where(x => x.Id == id).FirstOrDefault();
                var getdocid = db.DocDetail.Where(x => x.DoctorID == getclinic.DoctorID).FirstOrDefault();
                // Get the current date as a DateTime object
                //string currentDate = DateTime.Today.ToString("MMMM dd, yyyy (dddd)");


                // Get current date
                DateTime currentDate = DateTime.Today;

                // Generate a list of dates starting from current date and ending after 7 days
                List<DateTime> dates = Enumerable.Range(0, 7)
                                .Select(i => currentDate.AddDays(i))
                                .ToList();


                var appslotinfos = db.Appslotinfo
                    .Where(x => x.DoctorID == getclinic.DoctorID &&
                                x.ClinicName == getclinic.ClinicName)
                    .ToList();

                var getdates = appslotinfos
                                    .Where(x => dates.Contains(DateTime.ParseExact(x.Date, "MMMM dd, yyyy (dddd)", CultureInfo.InvariantCulture)))
                                    .ToList();




                // Filter the dates that are equal to or greater than the current date
                //var getdates = db.Appslotinfo.Where(x => x.DoctorID == getclinic.DoctorID && x.ClinicName == getclinic.ClinicName && string.Compare(x.Date, currentDate) >= 0).ToList();
                ViewBag.getdate = getdates;
                var checkbooking = getdates.Any(x => x.AvailableSlots > 0 && x.IsAvailable == "1");

                if (checkbooking)
                {
                    // Enable the book now button
                    ViewBag.ButtonText = "Book Appointment";
                    ViewBag.ButtonDisabled = false;
                }
                else
                {
                    // Disable the book now button
                    ViewBag.ButtonText = "Full Booked";
                    ViewBag.ButtonDisabled = true;
                }
                return View(new DoctorDataViewModel
                {
                    Doctorinfo = getdocid,
                    clinicinformation = getclinic
                });
            }           
        }

        public async Task<string> GetChatGPTResponse(string inputText)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "API Key");

            var requestBody = new
            {
                prompt = inputText,
                temperature = 0.7,
                max_tokens = 20
            };

            var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            var stringContent = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/engines/davinci-codex/completions", stringContent);

            var jsonResponse = await response.Content.ReadAsStringAsync();

            dynamic responseObject = JsonConvert.DeserializeObject(jsonResponse);

            var chatGPTResponse = responseObject.choices[0].text;

            return chatGPTResponse;
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult Department()
        {
            return View();
        }

        

        public ActionResult Doctor(string searchstring, string searchbylocation, string searchbyspeciality)
        {
            var query = db.DocDetail.AsQueryable();
            
            // var query = from doc in db.DocDetail join clinic in db.Clinicinfo on doc.DoctorID equals clinic.DoctorID where clinic.Location == searchbylocation select new { doc, clinic };
            if (!string.IsNullOrEmpty(searchstring))
            {
                query = query.Where(d => d.DoctorName.Contains(searchstring) || d.Education.Contains(searchstring) || d.Location.Contains(searchstring));
            }

            if (!string.IsNullOrEmpty(searchbylocation))
            {
                query = query.Where(d => d.Location.Contains(searchbylocation));
            }
          

            if (!string.IsNullOrEmpty(searchbyspeciality))
            {
                query = query.Where(d => d.Specialization == searchbyspeciality);
            }

            var getdata = query.OrderByDescending(x => x.Id).ToList();
            //getdata = query.Select(x => x.doc).OrderByDescending(x => x.Id).ToList();


            // Retrieve distinct values of "location" column from the database
            var locations = db.DocDetail.AsEnumerable().SelectMany(x => x.Location.Split(' ')).Distinct().ToList();
            var speciality = db.DocDetail.Select(x => x.Specialization).Distinct().ToList();

            // Populate dropdown list with the distinct locations
            var locationSelectList = new SelectList(locations);
            var SpecializationList = new SelectList(speciality);

            // Pass the dropdown list to the view
            ViewBag.LocationSelectList = locationSelectList;
            ViewBag.SpecializationDropList = SpecializationList;

            return View(new DoctorDataViewModel
            {
                docdetails = getdata               
            });
        }


        public ActionResult Service()
        {
            return View();
        }
    }
}