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
    public class MatchController : Controller
    {
        private Matches dbMat = new Matches();
        private Joins dbJoi = new Joins();

        // GET: Matches
        public ActionResult Matches()
        {
            List<TB_Match> matchList = (from a in dbMat.TB_Match where a.c_time > DateTime.Now select a).ToList();
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
                matchViewList.Add(matchView);
            }
            return View(matchViewList);
        }

        // GET: Match/Details
        public ActionResult Details(int? id = null)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_Match tB_Match = dbMat.TB_Match.Find(id);
            if (tB_Match == null)
            {
                return HttpNotFound();
            }
            return View(tB_Match);
        }

        // GET: Match/CreateMatch
        public ActionResult CreateMatch()
        {
            //int curUserAuthority = (Session["USerMessage"] as UserLoginModel).authority;
            int curUserAuthority = 1;

            // due to have no authority
            if (curUserAuthority == 0)
            {
                return RedirectToAction("Fail", new { failCode = 1 });
            }
            return View();
        }

        // POST: Match/CreateMatch
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMatch([Bind(Include = "m_id,u_id,t_id,name,sponsor,m_num,w_num,s_time,e_time,c_time,location,description")] TB_Match tB_Mmatch)
        {
            //int curUserId = (Session["UserMessage"] as UserLoginModel).id;
            int curUserId = 9;

            TB_Match tB_Match = new TB_Match();

            tB_Match.u_id = curUserId;
            tB_Match.name = Request.Form["Name"];
            tB_Match.sponsor = Request.Form["Sponsor"];
            tB_Match.s_time = DateTime.Parse(Request.Form["StartTime"]);
            tB_Match.e_time = DateTime.Parse(Request.Form["EndTime"]);
            tB_Match.c_time = DateTime.Parse(Request.Form["EnrollTime"]);
            tB_Match.m_num = int.Parse(Request.Form["EnrollNumber"]);
            tB_Match.w_num = int.Parse(Request.Form["VisitNumber"]);
            tB_Match.location = Request.Form["Location"];
            if (Request.Form["Description"] == "You can keep it empty")
            {
                tB_Match.description = null;
            }
            else
            {
                tB_Match.description = Request.Form["Description"];
            }

            if (ModelState.IsValid)
            {
                dbMat.TB_Match.Add(tB_Match);
                dbMat.SaveChanges();
                if (Request.Form["Ticket"] != null)
                {
                    return RedirectToAction("Create", "Ticket");
                }
                return RedirectToAction("Matches");
            }

            return View(tB_Match);
        }

        // GET: Match/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_Match tB_Match = dbMat.TB_Match.Find(id);
            if (tB_Match == null)
            {
                return HttpNotFound();
            }
            return View(tB_Match);
        }

        // POST: Match/Edit
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit()
        {
            TB_Match tB_Match = new TB_Match();

            tB_Match.name = Request.Form["Name"];
            tB_Match.sponsor = Request.Form["Sponsor"];
            tB_Match.s_time = DateTime.Parse(Request.Form["StartTime"]);
            tB_Match.e_time = DateTime.Parse(Request.Form["EndTime"]);
            tB_Match.c_time = DateTime.Parse(Request.Form["EnrollTime"]);
            tB_Match.m_num = int.Parse(Request.Form["EnrollNumber"]);
            tB_Match.w_num = int.Parse(Request.Form["VisitNumber"]);
            tB_Match.location = Request.Form["Location"];
            tB_Match.description = Request.Form["Description"];

            if (string.IsNullOrEmpty(Request.Form["Submit"]) == false)
            {
                if (ModelState.IsValid)
                {
                    dbMat.Entry(tB_Match).State = EntityState.Modified;
                    dbMat.SaveChanges();
                    return RedirectToAction("Matches");
                }
                return View(tB_Match);
            }
            return View(tB_Match);
        }

        public ActionResult Detail(int id)
        {
            TB_Match match = new TB_Match();
            match = (from a in dbMat.TB_Match where a.m_id == id select a).ToList().FirstOrDefault();
            if (match != null)
            {
                return View(match);
            }
            return RedirectToAction("Matches");
        }

        [HttpPost]
        public ActionResult Detail(string id, string none)
        {
            if (ModelState.IsValid)
            {
                if (Request.Form["Enroll"] != null)
                {
                    TB_Join join = new TB_Join();
                    join.m_id = int.Parse(Request.Form["Enroll"]);
                    //join.u_id = (Session["UserMessage"] as UserLoginModel).id;
                    join.u_id = 9;
                    join.time = DateTime.Now;
                    foreach (TB_Join j in dbJoi.TB_Join)
                    {
                        if (j.m_id == join.m_id && j.u_id == join.u_id)
                        {
                            return Content("<script>alert('You have enrolled this match!'); top.location='Matches'; </script>");
                        }
                    }

                    dbJoi.TB_Join.Add(join);
                    dbJoi.SaveChanges();
                    return Content("<script language='javascript'>alert('Enroll successfully!');top.location='Matches';</script>");
                }
            }
            
            return Content("<script language='javascript'>alert('Interal error!');top.location='Matches';</script>");
        }

        // GET: Match/Cancel
        public ActionResult Cancel(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_Match tB_Match = dbMat.TB_Match.Find(id);
            if (tB_Match == null)
            {
                return HttpNotFound();
            }
            return View(tB_Match);
        }

        // POST: Match/Cancel
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TB_Match tB_Match = dbMat.TB_Match.Find(id);
            dbMat.TB_Match.Remove(tB_Match);
            dbMat.SaveChanges();
            return RedirectToAction("Matches");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbMat.Dispose();
            }
            base.Dispose(disposing);
        }
        public static void Show(System.Web.UI.Page page, string msg)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "message", "<script language='javascript' defer>alert('" + msg.ToString() + "');</script>");
        }
    }
}
