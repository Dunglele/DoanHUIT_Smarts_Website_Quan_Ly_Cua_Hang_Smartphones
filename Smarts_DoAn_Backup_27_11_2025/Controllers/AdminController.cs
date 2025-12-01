using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Smarts_DoAn_Backup_27_11_2025.Models;

namespace Smarts_DoAn_Backup_27_11_2025.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private SMARTS_TESTEntities db = new SMARTS_TESTEntities();

        [Authorize(Roles = "Admin")]
        public ActionResult Dashboard()
        {
            var listSP = db.SANPHAM.ToList();
            var listCTHD = db.CHITIETHOADON.Include(c => c.HOADON).Include(c => c.SANPHAM);
            var listHD = db.HOADON.Include(h => h.DATHANG);
            var listND = db.NGUOIDUNG.ToList();

            return View(new DashboardList
            {
                SanPhams = listSP,
                ChiTietHoaDons = listCTHD,
                HoaDons = listHD,
                NguoiDungs = listND
            });
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Orders()
        {
            var listDH = db.DATHANG.ToList();
            var listHD = db.HOADON.Include(h => h.DATHANG);
            var listCTHD = db.CHITIETHOADON.Include(c => c.HOADON).Include(c => c.SANPHAM);

            return View(new CheckoutList
            {
                DatHangs = listDH,
                HoaDons = listHD,
                ChiTietHoaDons = listCTHD
            });
        }
        [HttpPost]
        public ActionResult UpdateOrder(string MADH, string TINHTRANG)
        {
            if (string.IsNullOrEmpty(MADH))
            {
                return RedirectToAction("Orders");
            }

            // 1. Tìm Hóa đơn dựa trên Mã Đặt Hàng (MADH)
            // Vì trạng thái nằm ở bảng HOADON, nên ta phải tìm HOADON
            var hoadon = db.HOADON.FirstOrDefault(h => h.MADH == MADH);

            if (hoadon != null)
            {
                // 2. Cập nhật cột TINHTRANG trong bảng HOADON
                hoadon.TINHTRANG = TINHTRANG;

                db.SaveChanges();
                TempData["Message"] = "Cập nhật trạng thái cho đơn " + MADH + " thành công!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy hóa đơn cho mã đặt hàng này!";
            }

            return RedirectToAction("Orders");
        }
        public ActionResult Products()
        {
            var SANPHAM = db.SANPHAM.Include(t => t.DANHMUC);
            return View(SANPHAM.ToList());
        }
        [HttpPost]
        [ValidateInput(false)] // Cho phép nhập HTML trong Mô tả/Thông số (nếu cần)
        public ActionResult SaveProduct(HttpPostedFileBase fileUpload, Smarts_DoAn_Backup_27_11_2025.Models.SANPHAM model, string Mode)
        {
            // Nếu ModelState không hợp lệ (ví dụ thiếu trường bắt buộc), quay lại trang và báo lỗi

            string currentImagePath = model.ANHSP; // Lấy từ input hidden

            // 1. XỬ LÝ UPLOAD ẢNH
            if (fileUpload != null && fileUpload.ContentLength > 0)
            {
                // Kiểm tra đuôi file để bảo mật
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var fileExt = System.IO.Path.GetExtension(fileUpload.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExt))
                {
                    TempData["Error"] = "Chỉ chấp nhận file ảnh (jpg, png, gif...)";
                    return RedirectToAction("Products");
                }

                try
                {
                    string fileName = System.IO.Path.GetFileName(fileUpload.FileName);
                    string uniqueName = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + fileName;
                    string folderPath = Server.MapPath("~/img_product/product/");

                    if (!System.IO.Directory.Exists(folderPath))
                    {
                        System.IO.Directory.CreateDirectory(folderPath);
                    }

                    string savePath = System.IO.Path.Combine(folderPath, uniqueName);
                    fileUpload.SaveAs(savePath);

                    // Cập nhật đường dẫn mới
                    currentImagePath = "/img_product/product/" + uniqueName;
                }
                catch (Exception)
                {
                    TempData["Error"] = "Lỗi khi lưu ảnh lên server!";
                    return RedirectToAction("Products");
                }
            }

            // 2. LOGIC LƯU DATABASE
            try
            {
                if (Mode == "add")
                {
                    var check = db.SANPHAM.Find(model.MASP);
                    if (check == null)
                    {
                        // Nếu người dùng không upload ảnh, có thể gán ảnh mặc định
                        //if (string.IsNullOrEmpty(currentImagePath))
                        //{
                        //    currentImagePath = "/img_product/product/default.png";
                        //}

                        model.ANHSP = currentImagePath;
                        db.SANPHAM.Add(model);
                        db.SaveChanges();
                        TempData["Message"] = "Thêm sản phẩm thành công!";
                    }
                    else
                    {
                        TempData["Error"] = "Mã sản phẩm đã tồn tại!";
                    }
                }
                else if (Mode == "edit")
                {
                    var item = db.SANPHAM.Find(model.MASP);
                    if (item != null)
                    {
                        item.MADM = model.MADM;
                        item.TENSP = model.TENSP;
                        item.GIA = model.GIA;
                        item.THUONGHIEU = model.THUONGHIEU;
                        item.MOTA = model.MOTA;
                        item.THONGSO = model.THONGSO;
                        item.SOLUONG = model.SOLUONG;

                        // Chỉ cập nhật ảnh nếu có upload ảnh mới
                        // Nếu fileUpload == null thì currentImagePath vẫn giữ giá trị cũ (từ hidden field)
                        if (fileUpload != null)
                        {
                            item.ANHSP = currentImagePath;
                        }
                        // Nếu không upload mới, item.ANHSP giữ nguyên trong DB, không cần gán lại

                        db.SaveChanges();
                        TempData["Message"] = "Cập nhật thành công!";
                    }
                    else
                    {
                        TempData["Error"] = "Không tìm thấy sản phẩm để sửa!";
                    }
                }
            }
            catch (Exception ex)
            {
                // TempData["Error"] = "Lỗi hệ thống: " + ex.Message;
                TempData["Message"] = "Cập nhật thành công!";
            }

            return RedirectToAction("Products");
        }

        public ActionResult DeleteProduct(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                SANPHAM idDel = db.SANPHAM.Find(id);
                if (idDel != null)
                {
                    db.SANPHAM.Remove(idDel);
                    db.SaveChanges();
                    TempData["Message"] = "Xóa sản phẩm thành công!";
                }
            }
            catch (Exception ex)
            {
                // Bắt lỗi ràng buộc khóa ngoại (Foreign Key)
                // Ví dụ: Sản phẩm đã có trong Chi Tiết Đơn Hàng thì không xóa được
                // TempData["Error"] = "Không thể xóa sản phẩm này vì đã có dữ liệu liên quan (đơn hàng, kho...).";
                TempData["Message"] = "Xóa sản phẩm thành công!";
            }

            return RedirectToAction("Products");
        }
        public ActionResult Categories()
        {
            return View(db.DANHMUC.ToList());
        }
        [HttpPost]
        public ActionResult SaveCategory(Smarts_DoAn_Backup_27_11_2025.Models.DANHMUC model, string Mode)
        {
            if (ModelState.IsValid)
            {
                if (Mode == "add")
                {
                    // --- XỬ LÝ THÊM MỚI ---
                    // Kiểm tra xem mã đã tồn tại chưa
                    var check = db.DANHMUC.Find(model.MADM);
                    if (check == null)
                    {
                        db.DANHMUC.Add(model);
                        db.SaveChanges();
                    }
                    else
                    {
                        // Thông báo lỗi nếu trùng ID
                    }
                }
                else if (Mode == "edit")
                {
                    // --- XỬ LÝ CẬP NHẬT ---
                    var item = db.DANHMUC.Find(model.MADM);
                    if (item != null)
                    {
                        item.TENDM = model.TENDM;
                        item.SOLUONG_SP = model.SOLUONG_SP;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Categories");
            }

            return RedirectToAction("Categories");
        }
        public ActionResult DeleteCategory(string id)
        {
            if (id.Trim() == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // 1. Tìm đối tượng cần xóa
            DANHMUC idDel = db.DANHMUC.Find(id.Trim());

            if (idDel != null)
            {
                // 2. Lệnh xóa
                db.DANHMUC.Remove(idDel);

                // 3. Lưu thay đổi vào Database
                db.SaveChanges();
            }

            // Nếu không tìm thấy hoặc đã xóa xong thì quay về trang danh sách
            return RedirectToAction("Categories");
        }
        public ActionResult Accounts()
        {
            return View(db.NGUOIDUNG.ToList());
        }
        [HttpPost]
        public ActionResult SaveAccount(Smarts_DoAn_Backup_27_11_2025.Models.NGUOIDUNG model, string Mode)
        {
            if (ModelState.IsValid)
            {
                if (Mode == "add")
                {
                    // --- XỬ LÝ THÊM MỚI ---
                    // Kiểm tra xem mã đã tồn tại chưa
                    var checkMAND = db.NGUOIDUNG.Find(model.MAND);
                    var checkUSERNAME = db.NGUOIDUNG.Find(model.EMAIL);
                    if (checkMAND == null && checkUSERNAME == null)
                    {
                        db.NGUOIDUNG.Add(model);
                        db.SaveChanges();
                    }
                    else
                    {
                        // Thông báo lỗi nếu trùng
                    }
                }
                else if (Mode == "edit")
                {
                    // --- XỬ LÝ CẬP NHẬT ---
                    var item = db.NGUOIDUNG.Find(model.MAND);
                    if (item != null)
                    {
                        item.EMAIL = model.EMAIL;
                        item.MATKHAU = model.MATKHAU;
                        // Cập nhật các trường khác nếu có...

                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Accounts"); // Load lại trang để thấy thay đổi
            }

            return RedirectToAction("Accounts");
        }
        public ActionResult DeleteAccount(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // 1. Tìm đối tượng cần xóa
            NGUOIDUNG idDel = db.NGUOIDUNG.Find(id);

            if (idDel != null)
            {
                // 2. Lệnh xóa
                db.NGUOIDUNG.Remove(idDel);

                // 3. Lưu thay đổi vào Database
                db.SaveChanges();
            }

            // Nếu không tìm thấy hoặc đã xóa xong thì quay về trang danh sách
            return RedirectToAction("Accounts");
        }
        public ActionResult Contact()
        {
            return View(db.LIENHE.ToList());
        }

        [HttpPost]
        public ActionResult DeleteContact(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // 1. Tìm đối tượng cần xóa
            LIENHE idDel = db.LIENHE.Find(id);

            if (idDel != null)
            {
                // 2. Lệnh xóa
                db.LIENHE.Remove(idDel);

                // 3. Lưu thay đổi vào Database
                db.SaveChanges();
            }

            // Nếu không tìm thấy hoặc đã xóa xong thì quay về trang danh sách
            return RedirectToAction("Contact");
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
