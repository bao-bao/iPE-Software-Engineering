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
    public class HomePageController : Controller
    {
        private Users dbUse = new Users();
        private Matches dbMat = new Matches();
        private Buys dbBuy = new Buys();
        private Tickets dbTic = new Tickets();
        private Joins dbJoi = new Joins();
        private Collections dbCol = new Collections();
        private HomePageViewModel viewModel = new HomePageViewModel();

        // GET: HomePage
        public ActionResult Homepage()
        {
            UserLoginModel user = (Session["UserMessage"] as UserLoginModel);
            if(user == null)
            {
                return Content("<script language='javascript'>alert('Please Login First!');top.location='/LoginAndRegister/Index';</script>");
            }
            int curUserId = user.id;
            ViewData["uid"] = curUserId;

            viewModel.myself = (from a in dbUse.TB_User where (a.u_id == curUserId) select a).ToList().FirstOrDefault();

            viewModel.releasedMatch = (from a in dbMat.TB_Match where (a.u_id == curUserId) select a).ToList().FirstOrDefault();

            List<TB_Buy> bought = (from a in dbBuy.TB_Buy where (a.u_id == curUserId) select a).ToList();
            foreach(TB_Buy item in bought)
            {
                viewModel.boughtTickets.Add((from a in dbTic.TB_Tickets where (a.t_id == item.t_id) select a).ToList().FirstOrDefault());
            }

            List<TB_Join> joined = (from a in dbJoi.TB_Join where (a.u_id == curUserId) select a).ToList();
            foreach (TB_Join item in joined)
            {
                List<TB_Match> matchList = (from a in dbMat.TB_Match where (a.m_id == item.m_id) select a).ToList();
                viewModel.enrollMatches = new List<MatchViewModels>();
                foreach (TB_Match match in matchList)
                {
                    MatchViewModels matchView = new MatchViewModels();
                    matchView.id = match.m_id;
                    matchView.name = match.name;
                    matchView.sponsor = match.sponsor;
                    matchView.time = match.s_time.ToShortDateString() + " —— " + match.e_time.ToShortDateString();
                    matchView.location = match.location;
                    if (match.description == null)
                    {
                        matchView.description = "there's nothing";
                    }
                    else
                    {
                        matchView.description = match.description;
                    }
                    viewModel.enrollMatches.Add(matchView);
                }
            }

            viewModel.collection = (from a in dbCol.TB_Collection where (a.u_id == curUserId) select a).ToList();
            
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Homepage(int? id)
        {
            if(string.IsNullOrEmpty(Request.Form["Apply"]) == false)
            {
                UserLoginModel user = (Session["UserMessage"] as UserLoginModel);
                id = user.id;
                TB_User my = dbUse.TB_User.Find(id);
                if(my != null)
                {
                    if(my.authority == Users.NORMAL_USER)
                    {
                        my.authority = Users.MANAGER_USER;
                        if(ModelState.IsValid)
                        {
                            dbUse.Entry(my).State = EntityState.Modified;
                            dbUse.SaveChanges();
                            return Content("<script language='javascript'>alert('You are match manager now!');history.go(-1);</script>");
                        }
                    }
                    return Content("<script language='javascript'>alert('You already have this privilege!');top.location='/Match/CreateMatch';</script>");
                }
            }
            return View();
        }

        public ActionResult personalInfo(int? id)
        {
            TB_User user = new TB_User();
            user = dbUse.TB_User.Find(id);
            if(user != null)
            {
                return View(user);
            }
            return HttpNotFound();
        }

        [HttpPost]
        public ActionResult personalInfo(string username)
        {
            TB_User user = dbUse.TB_User.Find((Session["UserMessage"] as UserLoginModel).id);
            user.username = Request.Form["Username"];
            if(Request.Form["Pwd"] != "")
            {
                user.pwd = Request.Form["Pwd"];
            }
            user.name = Request.Form["Name"];
            if(Request.Form["Gender"] == "male")
            {
                user.gender = 1;
            }
            else if(Request.Form["Gender"] == "female")
            {
                user.gender = 2;
            }
            else
            {
                user.gender = 0;
            }
            user.card_id = Request.Form["Cardid"];
            user.phone = Request.Form["Phone"];
            if (Request.Form["Birthday"] != "")
            {
                user.birthday = DateTime.Parse(Request.Form["Birthday"]);
            }
            user.organization = Request.Form["Organization"];
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if (ModelState.IsValid)
            {
                try
                {
                    dbUse.Entry(user).State = EntityState.Modified;
                    dbUse.SaveChanges();
                }
                catch(Exception e)
                {
                    return Content("<script language='javascript'>alert('Username has been used!');history.go(-1);</script>");
                }
                return Content("<script language='javascript'>alert('Change successfully!');history.go(-1);</script>");
            }
            return RedirectToAction("homepage");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbUse.Dispose();
                dbMat.Dispose();
                dbBuy.Dispose();
                dbTic.Dispose();
                dbJoi.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
