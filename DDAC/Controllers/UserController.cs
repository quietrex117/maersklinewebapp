using DDAC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DDAC.Controllers
{
    public class UserController : Controller
    {
        //Registration Action
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        //Post Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] User user)
        {
            bool Status = false;
            string message = "";

            //saving data 
            //Model Validation
            if(ModelState.IsValid)
            {
                //Email is already existed
                var isExist = IsEmailAlreadyExisted(user.Email);
                if(isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already existed");
                    return View(user);
                }
                
                #region Generate Activation Code
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region Password Hashing
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword);
                #endregion

                user.IsEmailVerified = false;


                #region Save to Database
                using (DBDDACEntities3 dc = new DBDDACEntities3())
                {
                    dc.Users.Add(user);
                    dc.SaveChanges();

                    //Send Email to User
                    SendVerificationLinkEmail(user.Email, user.ActivationCode.ToString());
                    message = "Registration Succeed, Account Activation link " +
                        "has been sent to your email: " + user.Email;
                    Status = true;


                }
                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        //Verify Account
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;
                var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
                if(v!=null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                } else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }
        
        //Login
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        //Login Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl="")
        {
            //if credentials valid
            string message = "";
            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var v = dc.Users.Where(a => a.Email == login.Email).FirstOrDefault();
                if(v!=null)
                {
                    if(string.Compare(Crypto.Hash(login.Password), v.Password) == 0)
                    {
                        //provided password is valid
                        int timeout = login.RememberMe ? 525600 : 20; // 1year
                        var ticket = new FormsAuthenticationTicket(login.Email, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if(Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        } 
                        else
                        {
                            return RedirectToAction("Index", "Schedules");
                        }
                    } else
                    {
                        message = "Invalid credential provided";
                    }
                } else
                {
                    message = "Invalid credential provided";
                }
            }
                ViewBag.Message = message;
            return View();
        }

        //Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [NonAction]
        public bool IsEmailAlreadyExisted(string email)
        {
            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var v = dc.Users.Where(a => a.Email == email).FirstOrDefault();
                return v != null;
            }
        }

        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifiyUrl = "/User/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifiyUrl);

            var fromEmail = new MailAddress("maersklinetester@gmail.com", "Your Registration request has been received and is under process, registration@maersk.com");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "passwordtester123";

            string subject = "";
            string body = "";
            string bodys = "";

            if(emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";

                bodys = "<br/>Your registration request has been forwarded to the administrator for approval" +
                    "<br/>" +
                    "You will be informed by email when your request has been approved." +
                    "<br/>If you have general questions about Maersk Line, please go to http://www.maerskline.com" +
                    "<br/>" +
                    "Office Name: Marsek Sdn Bhd" +
                    "Office City: KL" +
                    "Phone: 60312384323" +
                    "<br/>" +
                    "Thank you for using Maersk Line online services" +
                    "<br/>" +
                    "Yours sincerly," +
                    "Maresk Line" +
                    "<br/>" +
                    "Please do not reply to this email, as we are unable to respond from this email address.";
                body = "<br/></br>We are excited to tell you that your Maersk Line account is" +
                        " successfully created.  Please proceed verify your account by clicking link below" +
                        "<br/><br/><a href='" + link + "'>" + link + "</a> <br/><br/>" +
                        "Yours sincerly,<br/>" +
                        "Maresk Line <br/><br/>" +
                        "Please do not reply to this email, as we are unable to respond from this email address.";
            } else if(emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi, <br/><br/>We got request for reset your account password.  Please click on the below link to reset your password" +
                    "<br/><br/><a href="+ link +">Reset Password Link</a>";
            }

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

        //Forgot Password
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            //Verify Email

            //Generate Reset Password Link

            //Send Email

            string message = "";

            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var account = dc.Users.Where(a => a.Email == Email).FirstOrDefault();
                if(account != null)
                {
                    //Send email for reset password
                    //unique identification number which is stored 
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;

                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "Reset password link has been sent to your email.";

                } else
                {
                    message = "Account not found";
                }
            }

            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            //Verify the password link

            //Find account associated with this link

            //redirect to reset password page

            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var user = dc.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();

                if(user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                } 
                else
                {
                    //Invalid link
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if(ModelState.IsValid)
            {
                //update new pw if valid
                using (DBDDACEntities3 dc = new DBDDACEntities3())
                {
                    var user = dc.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if(user!=null)
                    {
                        user.Password = Crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "New password updated Successfully";
                    }
                }
            }
            else
            {
                message = "Something Invalid";
            }
            ViewBag.Message = message; 
            return View(model);
        }

        public ActionResult GetVessels()
        {
            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var vessels = dc.Vessels.OrderBy(a => a.VesselName).ToList();
                return Json(new { data = vessels }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public ActionResult Save(int id)
        {
            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var v = dc.Vessels.Where(a=>a.VesselsId == id).FirstOrDefault();
                return View(v);
            }
        }
        [HttpGet]
        public ActionResult Details(int id)
        {
            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var v = dc.Vessels.Where(a => a.VesselsId == id).FirstOrDefault();
                return View(v);
            }
        }

        [HttpPost]
        public ActionResult Details(Vessel vessel)
        {
            bool status = false;
            if (ModelState.IsValid)
            {
                using (DBDDACEntities3 dc = new DBDDACEntities3())
                {
                    if (vessel.VesselsId > 0)
                    {
                        //Edit
                        var v = dc.Vessels.Where(a => a.VesselsId == vessel.VesselsId).FirstOrDefault();
                        if (v != null)
                        {
                            v.VesselName = vessel.VesselName;
                            v.Terminal = vessel.Terminal;
                            v.Country = vessel.Country;
                        }
                    }
                    else
                    {
                        //Save
                        dc.Vessels.Add(vessel);
                    }
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpPost]
        public ActionResult Save(Vessel vessel)
        {
            bool status = false;
            if(ModelState.IsValid)
            {
                using (DBDDACEntities3 dc = new DBDDACEntities3())
                {
                    if(vessel.VesselsId>0)
                    {
                        //Edit
                        var v = dc.Vessels.Where(a => a.VesselsId == vessel.VesselsId).FirstOrDefault();
                        if(v!=null)
                        {
                            v.VesselName = vessel.VesselName;
                            v.Terminal = vessel.Terminal;
                            v.Country = vessel.Country;
                        }
                    } else
                    {
                        //Save
                        dc.Vessels.Add(vessel);
                    }
                    dc.SaveChanges();
                    status = true;
                }
            }
            return new JsonResult { Data = new { status = status } };
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var v = dc.Vessels.Where(a => a.VesselsId == id).FirstOrDefault();
                if(v!=null)
                {
                    return View(v);
                } else
                {
                    return HttpNotFound();
                }
            }
        }

        [HttpPost]
        [ActionName("Delete")] 
        public ActionResult DeleteVessel(int id)
        {
            bool status = false;
            using (DBDDACEntities3 dc = new DBDDACEntities3())
            {
                var v = dc.Vessels.Where(a => a.VesselsId == id).FirstOrDefault();
                if(v!=null)
                {
                    dc.Vessels.Remove(v);
                    dc.SaveChanges();
                    status = true;
                }

            }
                return new JsonResult { Data = new { status = status } };
        }


    }
}