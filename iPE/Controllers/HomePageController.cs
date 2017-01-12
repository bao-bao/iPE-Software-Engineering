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
                List<MatchViewModels> matchViewList = new List<MatchViewModels>();
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
        public ActionResult personalInfo([Bind(Include = "u_id,username,pwd,name,gender,card_id,phone,birthday,authority,organization")] TB_User tB_user)
        {
            TB_User user = new TB_User();
            user.username = Request.Form["Username"];
            if(Request.Form["Pwd"] != null)
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
            user.birthday = DateTime.Parse(Request.Form["Birthday"]);
            user.organization = Request.Form["Organization"];

            if(ModelState.IsValid)
            {
                dbUse.Entry(user).State = EntityState.Modified;
                dbUse.SaveChanges();
                return RedirectToAction("homepage");
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
