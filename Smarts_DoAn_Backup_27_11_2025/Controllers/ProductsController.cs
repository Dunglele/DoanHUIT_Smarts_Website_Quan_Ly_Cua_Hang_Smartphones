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
            // --- GỌI TABLE-VALUED FUNCTION: DS_SANPHAM() ---

            // 1. Định nghĩa truy vấn gọi Function
            string sqlQuery = "SELECT * FROM DBO.DS_SANPHAM()";

            // 2. Thực thi Function và ánh xạ kết quả vào lớp SANPHAM
            // SqlQuery<T> là phương thức của EF6 để thực thi SQL và trả về một tập hợp Entity/Model.
            // *Lưu ý: Function này không cần tham số, nếu cần tham số, truyền chúng tương tự ExecuteSqlCommand.*
            var listSp = db.SANPHAM.SqlQuery(sqlQuery).ToList();

            // Lấy danh mục như cũ
            var listDm = db.DANHMUC.ToList();

            // 3. Trả về View với ViewModel chứa dữ liệu từ Function
            return View(new ProductListVM
            {
                // Truyền List<SANPHAM> (kết quả từ Function) vào ViewModel
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
            string sqlQuery = "SELECT * FROM DBO.DS_DANHMUC()";
            return PartialView(db.DANHMUC.SqlQuery(sqlQuery).ToList());
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
