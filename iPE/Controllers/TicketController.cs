using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using iPE.Models;

namespace iPE.Controllers {
    public class TicketController : Controller {
        private Tickets dbTicket = new Tickets();
        private Buys dbBuys = new Buys();

        // GET: Ticket
        public ActionResult Index() {
            List<TicketInfo> list = new List<TicketInfo>();
            string sql = "select * "
                        + "from ticket";
            List<TB_Ticket> list_TB_Ticket = new List<TB_Ticket>();
            list_TB_Ticket = dbTicket.Database.SqlQuery<TB_Ticket>(sql).ToList();
            foreach (var a_TB_Ticket in list_TB_Ticket) {
                var a_TicketInfo = new TicketInfo();
                a_TicketInfo.id = a_TB_Ticket.t_id;
                a_TicketInfo.name = a_TB_Ticket.name;
                a_TicketInfo.type = a_TB_Ticket.type;
                a_TicketInfo.surplus = a_TB_Ticket.max - a_TB_Ticket.sell;
                a_TicketInfo.price = a_TB_Ticket.price;
                a_TicketInfo.description = a_TB_Ticket.description;
                list.Add(a_TicketInfo);
            }
            return View(list.AsEnumerable());
        }

        // GET: Ticket/Details/5
        public ActionResult Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_Ticket ticketInfo = dbTicket.TB_Tickets.Find(id);
            if (ticketInfo == null) {
                return HttpNotFound();
            }
            return View(ticketInfo);
        }

        // GET: Ticket/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Ticket/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,price,max,description")] TicketCreateModel ticketCreateModel) {
            var new_TB_Ticket = new TB_Ticket();
            if (ModelState.IsValid) {
                new_TB_Ticket.u_id = 1;
                new_TB_Ticket.m_id = 0;
                new_TB_Ticket.t_id = 11;
                new_TB_Ticket.name = ticketCreateModel.name;
                if (ticketCreateModel.price == 0) {
                    new_TB_Ticket.type = 0;
                }
                if (ticketCreateModel.price > 0) {
                    new_TB_Ticket.type = 1;
                }
                new_TB_Ticket.max = ticketCreateModel.max;
                new_TB_Ticket.price = ticketCreateModel.price;
                new_TB_Ticket.description = ticketCreateModel.description;
                new_TB_Ticket.sell = 0;
                //new_TB_Ticket.u_id
                dbTicket.TB_Tickets.Add(new_TB_Ticket);
                try {
                    dbTicket.SaveChanges();
                }
                catch (Exception ex) {
                    return Content("<script>alert('创建失败 :" + ex.Message + "');</script>");
                }
                return RedirectToAction("Index");
            }

            return View(ticketCreateModel);
        }

        // GET: Ticket/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_Ticket new_TB_Ticket = dbTicket.TB_Tickets.Find(id);
            if (new_TB_Ticket == null) {
                return HttpNotFound();
            }
            var ticketInfo = new TicketInfo();
            ticketInfo.id = new_TB_Ticket.t_id;
            ticketInfo.name = new_TB_Ticket.name;
            ticketInfo.type = new_TB_Ticket.type;
            ticketInfo.surplus = new_TB_Ticket.max - new_TB_Ticket.sell;
            ticketInfo.price = new_TB_Ticket.price;
            ticketInfo.description = new_TB_Ticket.description;
            return View(ticketInfo);
        }

        // POST: Ticket/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "t_id,name,type,price,number,describe")] TB_Ticket ticket) {
            if (ModelState.IsValid) {
                dbTicket.Entry(ticket).State = EntityState.Modified;
                dbTicket.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticket);
        }

        // GET: Ticket/Delete/5
        public ActionResult Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_Ticket ticket = dbTicket.TB_Tickets.Find(id);
            if (ticket == null) {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            TB_Ticket ticket = dbTicket.TB_Tickets.Find(id);
            dbTicket.TB_Tickets.Remove(ticket);
            dbTicket.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                dbTicket.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpPost, ActionName("Buy")]
        public ActionResult Buy(int id) {
            int number = Convert.ToInt32(Request.Form["number"]);
            TB_Ticket ticket = dbTicket.TB_Tickets.Find(id);
            TB_User user = Session["UserLogin"] as TB_User;
            if (null == user) {
                return RedirectToAction("Login");
            }

            decimal userID = user.u_id;
            TB_Buy new_TB_Buy = new TB_Buy();
            new_TB_Buy.t_id = ticket.t_id;
            new_TB_Buy.u_id = user.u_id;
            new_TB_Buy.number = number;
            new_TB_Buy.time = DateTime.Now;
            new_TB_Buy.price = ticket.price * number;
            dbBuys.TB_Buy.Add(new_TB_Buy);
            dbBuys.SaveChanges();

            return RedirectToAction("Personal");
        }

        //public ActionResult Error() {

        //}
    }
}
