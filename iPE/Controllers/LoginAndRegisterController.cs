using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iPE.Models;

namespace iPE.Controllers
{
    public class LoginAndRegisterController : Controller
    {
        private Users db = new Users();
        private UserLoginModel userMessage = new UserLoginModel();

        // GET: UserLoginModels
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Index(string userName, string password, string phone)
        {
            if(string.IsNullOrEmpty(Request.Form["signin"]) == false)
            {
                userName = Request.Form["Username"];
                password = Request.Form["Password"];
                if(string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                {
                    return View();
                }
                if (ModelState.IsValid)
                {
                    foreach (TB_User myUser in db.TB_User)
                    {
                        if (myUser.username == userName)
                        {
                            if (myUser.pwd == password)
                            {
                                userMessage.username = myUser.username;
                                userMessage.id = myUser.u_id;
                                userMessage.authority = myUser.authority;

                                Session["UserMessage"] = userMessage;
                                return RedirectToAction("Index", "Home");                 // !!!!!!!!!!!!!!!!!!!!!!!!登陆成功跳转页面
                            }
                            else
                            {
                                return View();
                            }
                        }
                    }
                }
                
            }
            else if(string.IsNullOrEmpty(Request.Form["signup"]) == false)
            {
                TB_User newUser = new TB_User();
                newUser.authority = Users.NORMAL_USER;
                newUser.username = Request.Form["signupUsername"];
                newUser.pwd = Request.Form["signupPassword"];
                newUser.phone = Request.Form["phone"];
                if (ModelState.IsValid)
                {
                    try
                    {
                        db.TB_User.Add(newUser);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Home");
                    }
                    catch(Exception e)
                    {
                        return Content("<script language='javascript'>alert('This user have been registed!');</script>");
                    }
                }
                else
                {
                    return View();
                }
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
