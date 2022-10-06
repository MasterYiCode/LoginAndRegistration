using LoginAndRegistration.Common;
using LoginAndRegistration.Models;
using Model.Dao;
using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LoginAndRegistration.Controllers
{
    public class UserController : Controller
    {
        [HttpGet]
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        // Registration Action (Hành động đăng ký)
        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        // Registration POST Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(UserModel userModel)
        {
            bool Status = false;
            string message = "";
            // model Validation
            if (ModelState.IsValid)
            {
                #region // Email is already exist: đã tồn tại
                var isExist = IsEmailExist(userModel.Email);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email đã tồn tại");
                    return View(userModel);
                }
                #endregion

                #region // Generate activation Code: tạo mã kích hoạt
                var ActivationCode = Guid.NewGuid();
                #endregion

                #region // Password Hashing
                userModel.Password = Encryptor.MD5Hash(userModel.Password);
                userModel.ConfirmPassword = Encryptor.MD5Hash(userModel.Password);
                #endregion

                #region // Save data to database
                var dao = new UserDao();
                dao.Insert(new User()
                {
                    Name = userModel.Name,
                    Email = userModel.Email,
                    Password = userModel.Password,
                    DateOfBirth = userModel.DateOfBirth,
                    ActivationCode = ActivationCode,
                    IsEmailVerified = false
                });
                #endregion

                #region // Send Email to User
                SendVerificationLinkEmail(userModel.Email, ActivationCode.ToString());
                #endregion

                message = "Đăng ký tài khoản thành công" +
                    "Liên kết kích hoạt tài khoản đã được gửi đến email của bạn: " +
                    userModel.Email;


            }
            else
            {
                message = "Yêu cầu không hợp lệ";
            }


            // Save data to database

            // Send Email to User
            ViewBag.Message = message;
            ViewBag.Status = true;

            return View(userModel);
        }

        // Verify Account via Email: Xác minh tài khoản thông qua email
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool status = false;

            var dao = new UserDao();
            var result = dao.VerifyAccount(id);
            if (result)
            {
                status = true;
            }
            else
            {
                ViewBag.Message = "Yêu cầu không hợp lệ";
            }
            ViewBag.Status = status;
            return View();
        }

        // Login Action
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            string message = "";
            if (ModelState.IsValid)
            {
                var dao = new UserDao();
                var user = dao.GetUserByEmail(login.Email);

                if (user != null)
                {
                    if (!user.IsEmailVerified)
                    {
                        ViewBag.Message = $"Vui lòng xác minh email của bạn trước khi đăng nhập. Xác minh đã được gửi về Gmail: {user.Email}";
                        return View();
                    }
                    if (string.Compare(Encryptor.MD5Hash(login.Password), user.Password) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20;
                        var ticket = new FormsAuthenticationTicket(login.Email, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;

                        Response.Cookies.Add(cookie);

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }
                    else
                    {
                        message = "Thông tin đăng nhập không hợp lệ.";
                    }
                }
                else
                {
                    message = "Thông tin đăng nhập không hợp lệ.";
                }
            }
            ViewBag.Message = message;
            return View(login);
        }

        // Logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [NonAction]
        public bool IsEmailExist(string email)
        {
            var dao = new UserDao();
            return dao.FindEmailFirstOrDefault(email);
        }

        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/User/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("dochihung492002@gmail.com", "Do Chi Hung");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "Cogangleniy1";

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";

                body = "<br/><br/>We are excited to tell you that your Do Chi Hung account is " +
                            "successfully created. Please click on the below link to verify your account" +
                            " <br/><br/><a href='" + link + "'>" + link + "</a>";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password!";
                body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password " +
                           " <br/><br/><a href='" + link + "'>Reset password link</a>";
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
                IsBodyHtml = true,
            })
            {
                //smtp.Send(message);
            };
        }

        // Forgot password
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            // Verify Email: kiểm chứng email
            // Generate Reset password link: Tạo liên kết đặt lại mật khẩu
            // Send: gửi link về Email
            string message = "";
            bool status = false;

            var dao = new UserDao();
            var user = dao.GetUserByEmail(email);
            if (user != null)
            {
                // Send email for reset password
                string resetCode = Guid.NewGuid().ToString();
                SendVerificationLinkEmail(user.Email, resetCode, "ResetPassword");
                dao.UpdateResetPasswordCode(user.Email, resetCode);
                status = true;
                message = "Link đặt lại mật khẩu đã được gửi về gmail bạn. Vui lòng vào gmail kiểm tra tin nhắn.";
            }
            else
            {
                message = "Tài khoán không hợp lệ.";
            }

            ViewBag.Status = status;
            ViewBag.Message = message;

            return View();
        }

        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            // verify the reset password link
            // find account associated with this link 
            // redirect to reset password page
            var dao = new UserDao();
            var user = dao.GetUserResetPasswordCode(id);
            if(user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }   
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";


            if (ModelState.IsValid)
            {
                var dao = new UserDao();
                string newPasswordMD5Hash = Encryptor.MD5Hash(model.NewPassword);
                bool result = dao.UpdatePassword(model.ResetCode, newPasswordMD5Hash);
                if (result)
                {
                    message = "Cập nhập mật khẩu thành công";
                }
            }
            else
            {
                message = "Một cái gì đó không hợp lệ rùi?";
            }    

            ViewBag.Message = message;
            return View();
        }
    }
}