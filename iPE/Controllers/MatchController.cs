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

        // GET: Match
        public ActionResult Matches()
        {
            List<TB_Match> matchList = (from a in dbMat.TB_Match where a.c_time > DateTime.Now select a).ToList();
            List <MatchViewModels> matchViewList = new List<MatchViewModels>();
            foreach(TB_Match match in matchList)
            {
                MatchViewModels matchView = new MatchViewModels();
                matchView.m_id = match.m_id;
                matchView.name = match.name;
                matchView.sponsor = match.sponsor;
                matchView.time = match.s_time.ToShortDateString() + " —— " + match.e_time.ToShortDateString();
                matchView.location = match.location;
                matchViewList.Add(matchView);
            }
            return View(matchViewList);
        }

        // GET: Match/Details
        public ActionResult Details(int? m_id = null)
        {
            if (m_id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_Match tB_Match = dbMat.TB_Match.Find(m_id);
            if (tB_Match == null)
            {
                return HttpNotFound();
            }
            return View(tB_Match);
        }

        // GET: Match/CreateMatch
        public ActionResult CreateMatch()
        {
            return View();
        }

        // POST: Match/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMatch([Bind(Include = "m_id,u_id,t_id,name,sponsor,m_num,w_num,s_time,e_time,c_time,location,description")] TB_Match tB_Match)
        {
            if (ModelState.IsValid)
            {
                dbMat.TB_Match.Add(tB_Match);
                dbMat.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tB_Match);
        }

        // GET: Match/Edit/5
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

        // POST: Match/Edit/5
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
                return RedirectToAction("Index");
            }
            return View(tB_Match);
        }

        // GET: Match/Delete/5
        public ActionResult Delete(int? id)
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

        // POST: Match/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TB_Match tB_Match = dbMat.TB_Match.Find(id);
            dbMat.TB_Match.Remove(tB_Match);
            dbMat.SaveChanges();
            return RedirectToAction("Index");
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
