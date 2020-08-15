using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Carents.Models;
using Microsoft.AspNet.Identity;

namespace Carents.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private Entities1 db = new Entities1();

        // GET: Orders
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.AspNetUser).Include(o => o.Car).Include(o => o.Rate);
            return View(orders.ToList());
        }

        public ActionResult GeneratePdf()
        {
            return new Rotativa.ActionAsPdf("Index") { FileName = "Order List.pdf" };
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.CustomerId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.Car_Id = new SelectList(db.Cars, "Id", "OwnerId");
            ViewBag.Rate_Id = new SelectList(db.Rates, "Id", "Rating");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Start_date,Tot_price,Car_name,Status,Date_number,CustomerId,Car_Id,Rate_Id")] Order order)
        {
            order.CustomerId = User.Identity.GetUserId();
            ModelState.Clear();
            TryValidateModel(order);
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerId = new SelectList(db.AspNetUsers, "Id", "Email", order.CustomerId);
            ViewBag.Car_Id = new SelectList(db.Cars, "Id", "OwnerId", order.Car_Id);
            ViewBag.Rate_Id = new SelectList(db.Rates, "Id", "Rating", order.Rate_Id);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.AspNetUsers, "Id", "Email", order.CustomerId);
            ViewBag.Car_Id = new SelectList(db.Cars, "Id", "OwnerId", order.Car_Id);
            ViewBag.Rate_Id = new SelectList(db.Rates, "Id", "Rating", order.Rate_Id);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Start_date,Tot_price,Car_name,Status,Date_number,CustomerId,Car_Id,Rate_Id")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.AspNetUsers, "Id", "Email", order.CustomerId);
            ViewBag.Car_Id = new SelectList(db.Cars, "Id", "OwnerId", order.Car_Id);
            ViewBag.Rate_Id = new SelectList(db.Rates, "Id", "Rating", order.Rate_Id);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult Chart()
        {
            ArrayList xValue = new ArrayList();
            ArrayList yValue = new ArrayList();

            var results = (from a in db.Orders select a);

            results.ToList().ForEach(rs => xValue.Add(rs.Car.Name));
            results.ToList().ForEach(rs => yValue.Add(rs.Rate.Rating));

            new Chart(width: 600, height: 400, theme: ChartTheme.Blue)

            .AddTitle("Popularity of Car")

            .AddSeries("Default", chartType: "column", xValue: xValue, yValues: yValue)

            .Write("bmp");



            return null;
        }
    }
}
