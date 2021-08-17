using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MovieShop.Models;
using MovieShop.Models.ViewModels;

namespace MovieShop.Controllers
{
    public class CustomersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Customers
        [Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        public ActionResult Index()
        {
           
            return View(db.Customers.ToList());
        }
        [Authorize(Roles ="Admin")]
        public ActionResult CheckOrders(string email)
        {
            List<OrderHistoryVM> OrdVmLst = new List<OrderHistoryVM>();
            if (!string.IsNullOrEmpty(email))
            {
                
                Customer Cust = db.Customers.Where(c => c.EmailAddress.ToLower() == email.ToLower()).FirstOrDefault();
                if (Cust!=null)
                {
                    OrdVmLst=db.Orders.Where(o => o.CustomerId == Cust.Id)
                        .Join(db.OrderRows, o => o.Id, r => r.OrderId, (o, r) => new { o, r })
                        .Select(ord => new OrderHistoryVM
                        {
                            OrderId = ord.o.Id, 
                            CustomerName=ord.o.Customer.FirstName+" "+ord.o.Customer.LastName,
                            MovieTitle=ord.r.Movie.Title,
                            OrderDate=ord.o.OrderDate,
                            Price=ord.r.Price,
                            TotalPrice = ord.o.TotalPrice,
                            NoofCopies=ord.r.NoofCopies
                        }).ToList();
                        
                }
            }

            return View(OrdVmLst);
        }
        [AllowAnonymous]
        public ActionResult ValidateCustomer(string email)
        {
            Session["Msg"] = "Already a Customer!";
            Session["Valid"] = 1;
            Customer cust = db.Customers.Where(c => c.EmailAddress.ToLower() == email.ToLower()).FirstOrDefault();
            if(cust==null)
            {
                Session["Msg"] = "Not a registered Customer!";
                Session["Valid"] = 0;
            }
            else
            {
                Session["CustId"] = cust.Id;
            }
            Session["UsrEmail"] = email;
            return View();
        }

        // GET: Customers/Details/5
        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }
        [AllowAnonymous]
        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }
       
        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,BillingAdress,BillingZip,BillingCity,DeliveryAdress,DeliveryZip,DeliveryCity,EmailAddress,PhoneNo")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                Session["CustId"] = customer.Id;
                //return RedirectToAction("Index");
                ViewBag.Email = customer.EmailAddress;
                return View("RegistrationSuccess");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        [Authorize(Roles ="Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,BillingAdress,BillingZip,BillingCity,DeliveryAdress,DeliveryZip,DeliveryCity,EmailAddress,PhoneNo")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }


        // POST: Customers/Delete/5
        [Authorize(Roles ="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
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
    }
}
