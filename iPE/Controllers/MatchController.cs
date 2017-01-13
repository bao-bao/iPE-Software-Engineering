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

        // GET: Matches
        public ActionResult Matches()
        {
            List<TB_Match> matchList = (from a in dbMat.TB_Match where a.c_time > DateTime.Now select a).ToList();
            List <MatchViewModels> matchViewList = new List<MatchViewModels>();
            foreach(TB_Match match in matchList)
            {
                MatchViewModels matchView = new MatchViewModels();
                matchView.id = match.m_id;
                matchView.name = match.name;
                matchView.sponsor = match.sponsor;
                matchView.time = match.s_time.ToShortDateString() + " —— " + match.e_time.ToShortDateString();
                matchView.location = match.location;
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
            //decimal curUserAuthority = (Session["UserLogin"] as UserLoginModel).authority;
            decimal curUserAuthority = 1;

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
        public ActionResult CreateMatch([Bind(Include = "m_id,u_id,t_id,name,sponsor,m_num,w_num,s_time,e_time,c_time,location,description")] TB_Match tB_Match)
        {
            //decimal curUserId = (Session["UserLogin"] as UserLoginModel).id;
            decimal curUserId = 1;
            //decimal curUserAuthority = (Session["UserLogin"] as UserLoginModel).authority;
            decimal curUserAuthority = 1;

            // due to have no authority
            if (curUserAuthority == 0)
            {
                return RedirectToAction("Fail", new { failCode = 1 });
            }
            // due to have been created
            foreach(TB_Match match in dbMat.TB_Match)
            {
                if(match.u_id == curUserId && match.e_time > DateTime.Now)
                {
                    return RedirectToAction("Fail", new { failCode = 2 });
                }
            }

            if (ModelState.IsValid)
            {
                dbMat.TB_Match.Add(tB_Match);
                dbMat.SaveChanges();
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
        public ActionResult Edit([Bind(Include = "m_id,u_id,t_id,name,sponsor,m_num,w_num,s_time,e_time,c_time,location,description")] TB_Match tB_Match)
        {
            if (ModelState.IsValid)
            {
                dbMat.Entry(tB_Match).State = EntityState.Modified;
                dbMat.SaveChanges();
                return RedirectToAction("Matches");
            }
            return View(tB_Match);
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

        //GET: Match/Fail
        public ActionResult Fail(int failCode = 0)
        {
            string hit = "";
            if(failCode == 1)
            {
                hit = "you have no authority!";
            }
            if(failCode == 2)
            {
                hit = "you have been created a match!";
            }
            return View(hit);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbMat.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
