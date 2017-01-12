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
            //int curUserId = (Session["USerMessage"] as UserLoginModel).id;
            int curUserId = 5;
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
