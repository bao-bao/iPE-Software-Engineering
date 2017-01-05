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
            UserLoginModel userLoginModel = Session["UserMessage"] as UserLoginModel;
            if (userLoginModel == null) {
                return RedirectToAction("Index", "LoginAndRegister");
            }
            return View();
        }

        // POST: Ticket/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name,price,max,description")] TicketCreateModel ticketCreateModel) {
            UserLoginModel userLoginModel = Session["UserMessage"] as UserLoginModel;
            if (userLoginModel == null) {
                return RedirectToAction("Index", "LoginAndRegister");
            }
            var new_TB_Ticket = new TB_Ticket();
            if (ModelState.IsValid) {
                new_TB_Ticket.u_id = userLoginModel.id;
                //new_TB_Ticket.u_id = 5;
                new_TB_Ticket.m_id = 0;
                //new_TB_Ticket.t_id = 11;
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
                dbTicket.TB_Tickets.Add(new_TB_Ticket);
                dbTicket.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ticketCreateModel);
        }

        // GET: Ticket/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TB_Ticket tB_Ticket = dbTicket.TB_Tickets.Find(id);
            if (tB_Ticket == null) {
                return HttpNotFound();
            }
            var ticketEditModel = new TicketEditModel();
            ticketEditModel.id = tB_Ticket.t_id;
            ticketEditModel.name = tB_Ticket.name;
            ticketEditModel.max = tB_Ticket.max;
            ticketEditModel.description = tB_Ticket.description;
            return View(ticketEditModel);
        }

        // POST: Ticket/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,max,description")] TicketEditModel ticketEditModel) {
            if (ModelState.IsValid) {
                UserLoginModel userLoginModel = Session["UserMessage"] as UserLoginModel;
                if (userLoginModel == null) {
                    return RedirectToAction("Index", "LoginAndRegister");
                }
                if (userLoginModel.id != ticketEditModel.u_id) {
                    return Content("<script>alert('不好意思，您没有权限');</script>");
                }
                TB_Ticket tB_Ticket = dbTicket.TB_Tickets.Find(ticketEditModel.id);
                if (tB_Ticket == null) {
                    return HttpNotFound();
                }
                if(ticketEditModel.name != null) {
                    tB_Ticket.name = ticketEditModel.name;
                }
                if(ticketEditModel.max != null) {
                    if(tB_Ticket.sell > ticketEditModel.max) {
                        return Content(
                            "<script>alert('售出的票数已大于设定的票数\n\n提示：已卖出 "
                            + tB_Ticket.sell
                            + " 张票');</script>");
                    }
                    tB_Ticket.max = (int)ticketEditModel.max;
                }
                if (ticketEditModel.description != null) {
                    tB_Ticket.description = ticketEditModel.description;
                }

                dbTicket.Entry(tB_Ticket).State = EntityState.Modified;
                dbTicket.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Ticket/Delete/5
        public ActionResult Delete(int? id) {
            //UserLoginModel userLoginModel = Session["UserMessage"] as UserLoginModel;
            //if(userLoginModel == null) {
            //    return RedirectToAction("Index", "LoginAndRegister");
            //}
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
