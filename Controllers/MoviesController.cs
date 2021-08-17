using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MovieShop.Models;
using MovieShop.Models.ViewModels;

namespace MovieShop.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Movies
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            return View(db.movies.ToList());
        }
        [AllowAnonymous]
        public ActionResult MoviesList()
        {
            return View(db.movies.ToList());
        }
        [AllowAnonymous]
        public ActionResult FrontPage()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            
            //The 5 newest movies(based on release year) 
            var MovieListTi = db.movies.OrderByDescending(m => m.ReleaseYear).ToList().Take(5);
            //===========================================================================================================================================
            
            //The 5 oldest movies(based to release year)
            var MovieListRe = db.movies.OrderBy(m => m.ReleaseYear).ToList().Take(5);
            //===========================================================================================================================================
            
            //The 5 cheapest movies(based to price)
            var MovieListPr = db.movies.OrderBy(m => m.price).ToList().Take(5);
            //===========================================================================================================================================
            
            //var ExpensiveOrder = db.Orders.OrderByDescending(o => o.TotalPrice).ToList().Take(1);
            var ExpensiveOrder2 = db.Orders.OrderByDescending(o => o.TotalPrice).ToList().Take(1).FirstOrDefault();
            //===========================================================================================================================================
            
            //the most expensive order
            var ExpensiveCustomer = db.Customers.Where(c => c.Id == ExpensiveOrder2.CustomerId).FirstOrDefault();
            var CustomerName = ExpensiveCustomer.FirstName + " " + ExpensiveCustomer.LastName;

            ViewBag.CustName = CustomerName;
            //===========================================================================================================================================
            
            var tupleModel = new Tuple<List<Movie>, List<Movie>, List<Movie>/*, List<Order*//*>*/>(MovieListTi.ToList(), MovieListRe.ToList(), MovieListPr.ToList()/*,*/ /*ExpensiveOrder2.ToList*/);
            return View(tupleModel);
        }
        [AllowAnonymous]
        public ActionResult AddToCart(int MovieId, int Add, int IsCart)
        {
            List<int> MovieIdList = new List<int>();
            if (Session["MovieList"] != null) MovieIdList = (List<int>)Session["MovieList"];
            if (Add == 1) MovieIdList.Add(MovieId);
            else
            {
                MovieIdList.Reverse();
                MovieIdList.Remove(MovieId);
                MovieIdList.Reverse();
            }
            Session["MovieList"] = MovieIdList;
            Session["MovieCount"] = MovieIdList.Count;
            if (MovieIdList.Count == 0) return View("NoItems");
            if (IsCart == 0) return RedirectToAction("MoviesList");
            
            return RedirectToAction("DisplayCart", new { IsSummary = 0 });
           
        }
        [AllowAnonymous]
        public ActionResult DisplayCart(int IsSummary)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");

            Session["IsSummary"] = IsSummary;
            if (Session["MovieList"] ==null || (int)Session["MovieCount"] == 0) return View("NoItems");
            List<int> NoofCopiesList = new List<int>();
            //List<Movie> MovieDataList = new List<Movie>();
            List<decimal> PriceList = new List<decimal>();
            List<int> SelectedMovIdList = new List<int>();
            List<int> DistinctMovIdList = new List<int>();
            List<MovieListVM> MoviesVMList = new List<MovieListVM>();
            MovieListVM vmobj;
            decimal total = 0;

            SelectedMovIdList = (List<int>)Session["MovieList"];

            foreach (int id in SelectedMovIdList)
            {
                if (!DistinctMovIdList.Contains(id))
                {
                    DistinctMovIdList.Add(id);
                }
            }
            foreach (int Id in DistinctMovIdList)
            {
                vmobj = new MovieListVM();
                var movie = db.movies.Find(Id);
                int copies = SelectedMovIdList.Count(m => m == Id);
                vmobj.MovieId = movie.Id;
                vmobj.Movie = movie.Title; /*MovieDataList.Add(movie);*/
                vmobj.NoofCopies = copies;
                vmobj.ReleaseYear = movie.ReleaseYear;
                NoofCopiesList.Add(copies);
                vmobj.Price = movie.price * copies;
                    PriceList.Add(vmobj.Price);
                total += vmobj.Price;
                MoviesVMList.Add(vmobj);
            }
            ViewBag.TotalPrice = total;
            Session["MovieIdList"] = DistinctMovIdList;
            Session["CopiesLsit"] = NoofCopiesList;
            Session["PriceList"] = PriceList;
            Session["TotalPrice"] = total;
            return View(MoviesVMList);
        }
        [AllowAnonymous]
        public ActionResult SearchMovies()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult FilterMoviesList(string srchtxt)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            var movieslist = db.movies.Where(m => m.Title.Contains(srchtxt));
            return View("MoviesList", movieslist);
        }
        [AllowAnonymous]
        public ActionResult PaginateMovies(int page=1)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");

            PaginateVM paginateobj = new PaginateVM();
            paginateobj.CurrentPage = page;
            paginateobj.MoviesPerPage = 9;
            paginateobj.Movies = CurrentPageMovies(paginateobj.CurrentPage, paginateobj.MoviesPerPage);
            paginateobj.MoviesPerRow = 3;
            paginateobj.PageCount = PaginateVM.TotalPages(db.movies.Count(), paginateobj.MoviesPerPage);

            return View(paginateobj);
        }
        public IEnumerable<Movie> CurrentPageMovies(int CurrentPage,int CountPerPage)
        {
            int startcount = (CurrentPage - 1) * CountPerPage;
            var movieslist = db.movies.OrderBy(m => m.Id);
            return movieslist.Skip(startcount).Take(CountPerPage);
        }
        [AllowAnonymous]
        public ActionResult CheckOut()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            List<int> MovieIdList = new List<int>();
            List<int> CopiesList = new List<int>();
            List<decimal> PriceList = new List<decimal>();
            int idx = 0;

            Order ord = new Order();
            ord.CustomerId = (int)Session["CustId"];
            ord.OrderDate = DateTime.Now;
            ord.Customer = (Customer)Session["Customer"];
            ord.TotalPrice = (decimal)Session["TotalPrice"];
            db.Orders.Add(ord);
            db.SaveChanges();

            MovieIdList = (List<int>)Session["MovieIdList"];
            CopiesList = (List<int>)Session["CopiesLsit"];
            PriceList = (List<decimal>)Session["PriceList"];

            foreach (var movieid in MovieIdList)
            {
                OrderRow ordrw = new OrderRow();
                ordrw.OrderId = ord.Id;
                ordrw.MovieId = movieid;
                ordrw.NoofCopies = CopiesList.ElementAt(idx);
                ordrw.Price = PriceList.ElementAt(idx);
                db.OrderRows.Add(ordrw);
                db.SaveChanges();
                idx += 1;

            }
            ViewBag.OrderId = ord.Id;
            Session.Clear();
            return View();
           
        }
        //GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }
      

         //GET: Movies/Create
        public ActionResult Create()
        {
            return View();
        }

         //POST: Movies/Create
         //To protect from overposting attacks, enable the specific properties you want to bind to, for 
         //more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Director,ReleaseYear,LeadActor,price,ImageUrl")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                db.movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

         //GET: Movies/Edit/5
        public ActionResult Edit(int? id)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

         ////POST: Movies/Edit/5
         //To protect from overposting attacks, enable the specific properties you want to bind to, for 
         //more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Director,ReleaseYear,LeadActor,price,ImageUrl")] Movie movie)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            if (ModelState.IsValid)
            {
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

         //GET: Movies/Delete/5
        public ActionResult Delete(int? id)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

         //POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            Movie movie = db.movies.Find(id);
            db.movies.Remove(movie);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("sv-SE");
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
