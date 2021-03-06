﻿using System;
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


    public class TicketController : Controller
    {
        private Tickets dbTicket = new Tickets();
        private Buys dbBuys = new Buys();

        // GET: Ticket
        public ActionResult Index()
        {
            List<TicketInfo> list = new List<TicketInfo>();
            string sql = "select * "
                        + "from ticket";
            List<TB_Ticket> list_TB_Ticket = new List<TB_Ticket>();
            if (ModelState.IsValid)
            {
                list_TB_Ticket = dbTicket.Database.SqlQuery<TB_Ticket>(sql).ToList();
                foreach (var a_TB_Ticket in list_TB_Ticket)
                {
                    if (a_TB_Ticket.time < DateTime.Now)
                    {
                        continue;
                    }
                    var a_TicketInfo = new TicketInfo();
                    a_TicketInfo.id = a_TB_Ticket.t_id;
                    a_TicketInfo.name = a_TB_Ticket.name;
                    a_TicketInfo.type = a_TB_Ticket.type;
                    a_TicketInfo.surplus = a_TB_Ticket.max - a_TB_Ticket.sell;
                    a_TicketInfo.price = a_TB_Ticket.price;
                    a_TicketInfo.description = a_TB_Ticket.description;
                    a_TicketInfo.time = a_TB_Ticket.time;
                    list.Add(a_TicketInfo);
                }
                return View(list.AsEnumerable());
            }
            return View();
        }

        // GET: Ticket/Details/5
        public ActionResult Details(int? id)
        {
            TB_Ticket ticketInfo;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (ModelState.IsValid)
            {
                ticketInfo = dbTicket.TB_Tickets.Find(id);
                if (ticketInfo == null)
                {
                    return HttpNotFound();
                }
                return View(ticketInfo);
            }
            return HttpNotFound();
        }

        // GET: Ticket/Create
        public ActionResult Create(int? id)
        {
            ViewData["Matchid"] = id;
            return View();
        }

        // POST: Ticket/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public ActionResult Create([Bind(Include = "name,price,max,description,time")] TicketCreateModel ticketCreateModel)
        {
            UserLoginModel user = (Session["UserMessage"] as UserLoginModel);
            if (user == null)
            {
                return Content("<script language='javascript'>alert('Please Login First!');top.location='/LoginAndRegister/Index';</script>");
            }
            var new_TB_Ticket = new TB_Ticket();
            if (ModelState.IsValid)
            {
                new_TB_Ticket.u_id = user.id;
                new_TB_Ticket.m_id = int.Parse(Request.Form["m_id"]);
                new_TB_Ticket.name = ticketCreateModel.name;
                if (ticketCreateModel.price == 0)
                {
                    new_TB_Ticket.type = 0;
                }
                if (ticketCreateModel.price > 0)
                {
                    new_TB_Ticket.type = 1;
                }
                new_TB_Ticket.max = ticketCreateModel.max;
                new_TB_Ticket.price = ticketCreateModel.price;
                new_TB_Ticket.description = ticketCreateModel.description;
                new_TB_Ticket.time = ticketCreateModel.time;
                new_TB_Ticket.sell = 0;
                dbTicket.TB_Tickets.Add(new_TB_Ticket);
                dbTicket.SaveChanges();
                return RedirectToAction("Index");
            }
            return Content("<script>alert('创建失败');history.go(-1);</script>");
            //return View(ticketCreateModel);
        }

        // GET: Ticket/Edit/5
        public ActionResult Edit(int? id)
        {
            UserLoginModel user = (Session["UserMessage"] as UserLoginModel);
            int uid = user.id;

            TB_Ticket tB_Ticket = (from a in dbTicket.TB_Tickets where a.u_id == uid select a).ToList().FirstOrDefault();
            if (tB_Ticket == null)
            {
                return Content("<script>alert('You have NO ticket released!');history.go(-1);</script>");
            }
            var ticketEditModel = new TicketEditModel();
            ticketEditModel.id = tB_Ticket.t_id;
            ticketEditModel.name = tB_Ticket.name;
            ticketEditModel.max = tB_Ticket.max;
            ticketEditModel.description = tB_Ticket.description;
            ticketEditModel.time = tB_Ticket.time;
            return View(ticketEditModel);
        }

        // POST: Ticket/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        public ActionResult Edit([Bind(Include = "id,name,max,description,time")] TicketEditModel ticketEditModel)
        {
            if (ModelState.IsValid)
            {
                //UserLoginModel userLoginModel = Session["UserMessage"] as UserLoginModel;
                //if (userLoginModel == null) {
                //    return RedirectToAction("Index", "LoginAndRegister");
                //}
                //if (userLoginModel.id != ticketEditModel.u_id) {
                //    return Content("<script>alert('不好意思，您没有权限');</script>");
                //}
                TB_Ticket tB_Ticket = dbTicket.TB_Tickets.Find(ticketEditModel.id);
                if (tB_Ticket == null)
                {
                    return HttpNotFound();
                }
                if (ticketEditModel.name != null)
                {
                    tB_Ticket.name = ticketEditModel.name;
                }
                if (ticketEditModel.max != null)
                {
                    if (tB_Ticket.sell > ticketEditModel.max)
                    {
                        return Content(
                            "<script>alert('售出的票数已大于设定的票数\n\n提示：已卖出 "
                            + tB_Ticket.sell
                            + " 张票');</script>");
                    }
                    tB_Ticket.max = (int)ticketEditModel.max;
                }
                if (ticketEditModel.description != null)
                {
                    tB_Ticket.description = ticketEditModel.description;
                }
                if (ticketEditModel.time != null)
                {
                    tB_Ticket.time = ticketEditModel.time;
                }

                dbTicket.Entry(tB_Ticket).State = EntityState.Modified;
                dbTicket.SaveChanges();
                return Content("<script>alert('Edit succseefully!');top.location='/Ticket/Index';</script>"); ;
            }
            return View();
        }

        public ActionResult Buy(int? id)
        {
            TB_Ticket ticket = new TB_Ticket();
            ticket = dbTicket.TB_Tickets.Find(id);
            if (ticket != null)
            {
                return View(ticket);
            }
            return HttpNotFound();
        }

        [HttpPost, ActionName("Buy")]
        public ActionResult Buy(int id)
        {
            int number = Convert.ToInt32(Request.Form["Number"]);
            TB_Ticket ticket = dbTicket.TB_Tickets.Find(id);
            UserLoginModel user = (Session["USerMessage"] as UserLoginModel);
            if (user == null)
            {
                return Content("<script language='javascript'>alert('Please Login First!');top.location='/LoginAndRegister/Index';</script>");
            }

            if (ticket.max - ticket.sell > number)
            {
                int userID = user.id;
                TB_Buy new_TB_Buy = new TB_Buy();
                new_TB_Buy.t_id = ticket.t_id;
                new_TB_Buy.u_id = user.id;
                new_TB_Buy.number = number;
                new_TB_Buy.time = DateTime.Now;
                new_TB_Buy.price = ticket.price * number;
                dbBuys.TB_Buy.Add(new_TB_Buy);
                dbBuys.SaveChanges();
                ticket.sell += number;
                dbTicket.Entry(ticket).State = EntityState.Modified;
                dbTicket.SaveChanges();
            }
            else
            {
                return Content("<script language='javascript'>alert('Ticket not enough!');history.go(-1);</script>");

            }

            return Content("<script language='javascript'>alert('you get this tickets!');top.location='/HomePage/homepage#services';</script>");
        }

        public ActionResult detail(int? id)
        {
            BuyDetail detail = (Session[id.ToString()] as BuyDetail);
            return View(detail);
        }

        //public ActionResult Error() {

        //}

        // GET: Ticket/Delete/5
        public ActionResult Delete(int? id)
        {
            //UserLoginModel userLoginModel = Session["UserMessage"] as UserLoginModel;
            //if(userLoginModel == null) {
            //    return RedirectToAction("Index", "LoginAndRegister");
            //}
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_Ticket ticket = dbTicket.TB_Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TB_Ticket ticket = dbTicket.TB_Tickets.Find(id);
            dbTicket.TB_Tickets.Remove(ticket);
            dbTicket.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbTicket.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
