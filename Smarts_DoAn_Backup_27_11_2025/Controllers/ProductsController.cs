using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Smarts_DoAn_Backup_27_11_2025.Models;

namespace Smarts_DoAn_Backup.Controllers
{
    public class ProductsController : Controller
    {
        private SMARTS_TESTEntities db = new SMARTS_TESTEntities();
        // GET: Products
        public ActionResult ProductList()
        {
            // Corrected the variable to fetch the list of DANHMUC instead of SANPHAM
            var listSp = db.SANPHAM.Include("DANHMUC");
            var listDm = db.DANHMUC.ToList(); // Fetching DANHMUC entities instead of SANPHAM

            return View(new ProductListVM
            {
                SanPhams = listSp,
                DanhMucs = listDm
            });
        }

        public ActionResult Details(string id)
        {
            if (id == null)
            {
                //RedirectToAction("Error","Shared");
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SANPHAM SANPHAM = db.SANPHAM.Find(id);
            if (SANPHAM == null)
            {
                return HttpNotFound();
            }

            var listSp = db.SANPHAM.Include("DANHMUC");
            ViewBag.ListSP = listSp;

            return View(new ProductDetailsMD
            {
                Product = SANPHAM,
                Products = listSp
            });
        }

        public ActionResult Filter()
        {
            return PartialView(db.DANHMUC.ToList());
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