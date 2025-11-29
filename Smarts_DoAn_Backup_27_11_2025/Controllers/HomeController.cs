using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Smarts_DoAn_Backup_27_11_2025.Models;

namespace Smarts_DoAn_Backup_27_11_2025.Controllers
{
    public class HomeController : Controller
    {
        private SMARTS_TESTEntities db = new SMARTS_TESTEntities();
        // GET: TrangChu
        public ActionResult Index()
        {
            var SANPHAM = db.SANPHAM.Include("DANHMUC");
            return View(SANPHAM.ToList());
        }
        public ActionResult Signin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signin(Smarts_DoAn_Backup_27_11_2025.Models.NGUOIDUNG app)
        {
            // Việc sử dụng EF Framework, nhất là hàm FirstOrDefault khiến việc bị SQL injection hoàn toàn là không thể xảy ra có thể sử dụng Burpsuite để tìm lời giải đáp chi tiết
            var user = db.NGUOIDUNG.FirstOrDefault(us => us.EMAIL == app.EMAIL && us.MATKHAU == app.MATKHAU);

            if (user != null)
            {
                // 2. Tạo vé đăng nhập (Ticket) chứa thông tin User và Quyền (Role)
                // Lưu quyền vào tham số thứ 2 (userData) của ticket
                var ticket = new FormsAuthenticationTicket(
                    1,                                     // Version
                    user.EMAIL,                         // User name
                    DateTime.Now,                          // Ngày tạo
                    DateTime.Now.AddMinutes(30),           // Ngày hết hạn
                    false,                                 // IsPersistent (Ghi nhớ đăng nhập?)
                    user.VAITRO.Trim()                              // <--- LƯU QUYỀN VÀO ĐÂY (VD: "Admin")
                );

                // 3. Mã hóa ticket
                string encryptedTicket = FormsAuthentication.Encrypt(ticket);

                // 4. Tạo Cookie
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
                Response.Cookies.Add(cookie);

                if (user.VAITRO.Trim() == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tài khoản mật khẩu";
            return View();
        }
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(string EMAIL, string MATKHAU, string XACNHANMATKHAU, string HOTEN, string SODIENTHOAI)
        {
            // 1. Kiểm tra ràng buộc đầu vào
            if (MATKHAU != XACNHANMATKHAU)
            {
                ViewBag.Msg = "Mật khẩu và Xác nhận Mật khẩu không khớp.";
                return View();
            }

            // 2. Kiểm tra trùng lặp Email và SĐT
            var CEMAIL = db.NGUOIDUNG.FirstOrDefault(e => e.EMAIL == EMAIL);
            var CPHONE = db.NGUOIDUNG.FirstOrDefault(e => e.SODIENTHOAI == SODIENTHOAI);

            if (CEMAIL != null)
            {
                ViewBag.Msg = "Email này đã được sử dụng.";
                return View();
            }

            if (CPHONE != null)
            {
                ViewBag.Msg = "Số điện thoại này đã được sử dụng.";
                return View();
            }

            // --- BẮT ĐẦU SINH MÃ MAND (ND001, ND002, ...) ---

            // 3. Tìm bản ghi cuối cùng (Đảm bảo NGUOIDUNG có cột MAND)
            var lastNguoiDung = db.NGUOIDUNG
                // Quan trọng: Phải OrderByDescending theo ID (Primary Key) hoặc cột
                // có thể sắp xếp để tìm chính xác bản ghi cuối cùng
                .OrderByDescending(h => h.MAND) // Nếu MAND có thể sắp xếp chuỗi "ND009" > "ND001"
                                                // HOẶC dùng .OrderByDescending(h => h.ID) nếu bạn có ID tự tăng là số
                .FirstOrDefault();

            string MAND;

            if (lastNguoiDung == null)
            {
                // Trường hợp 1: Database trống
                MAND = "ND001";
            }
            else
            {
                // Trường hợp 2: Đã có dữ liệu
                string lastMa = lastNguoiDung.MAND; // Ví dụ: "ND009"

                // Trích xuất phần số (bỏ qua 2 ký tự tiền tố "ND")
                // Sử dụng Try/Catch để đảm bảo an toàn nếu dữ liệu cũ bị sai định dạng
                string numberPartString = lastMa.Substring(2);

                int nextNumber = 1;
                if (int.TryParse(numberPartString, out int currentNumber))
                {
                    nextNumber = currentNumber + 1;
                }
                else
                {
                    // Xử lý nếu parse thất bại (ví dụ: gán mã mặc định hoặc throw lỗi)
                    // Ở đây sẽ gán tạm "ND001" để tránh lỗi, nhưng bạn nên xử lý lỗi tốt hơn
                    nextNumber = 1;
                }

                // Định dạng lại thành chuỗi 3 chữ số có đệm số 0 ở đầu (001, 010, 100...)
                string nextNumberString = nextNumber.ToString("D3");
                MAND = "ND" + nextNumberString;
            }

            // --- KẾT THÚC SINH MÃ MAND ---

            // 4. Tạo và lưu Người dùng mới
            var newNguoiDung = new NGUOIDUNG
            {
                MAND = MAND,
                EMAIL = EMAIL,
                VAITRO = "Client", // Gán vai trò mặc định
                MATKHAU = MATKHAU, // Lưu ý: Nên mã hóa mật khẩu ở đây
                HOTEN = HOTEN,
                SODIENTHOAI = SODIENTHOAI,
            };
            db.NGUOIDUNG.Add(newNguoiDung);

            try
            {
                db.SaveChanges();

                // Đăng ký thành công -> Chuyển hướng sang trang đăng nhập
                TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("SignIn", "Home");
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi lưu database (ví dụ: lỗi trùng khóa, lỗi kết nối)
                ViewBag.Msg = "Đăng ký thất bại do lỗi hệ thống: " + ex.Message;
                return View();
            }
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(string EMAIL, string HOTEN, string SODIENTHOAI, string DIACHI)
        {
            // Khởi tạo đối tượng LIENHE mới
            var newLienHe = new LIENHE
            {
                EMAIL = EMAIL,
                HOTEN = HOTEN,
                SODIENTHOAI = SODIENTHOAI,
                DIACHI = DIACHI
                // **LƯU Ý:** Bạn KHÔNG cần phải gán MALH ở đây.
                // MALH sẽ được tạo tự động trong phần logic bên dưới.
            };

            // --- BẮT ĐẦU SINH MÃ MALH (LH001, LH002, ...) ---

            // 1. Tìm bản ghi cuối cùng, chỉ lấy cột MALH
            // .OrderByDescending(h => h.MALH): Sắp xếp theo chuỗi 'MALH' giảm dần (ví dụ: LH010, LH009, LH008...)
            var lastLienHe = db.LIENHE
                .Select(h => h.MALH)
                .OrderByDescending(malh => malh)
                .FirstOrDefault();

            string MALH = "LH001"; // Mặc định là LH001 nếu chưa có bản ghi nào

            if (lastLienHe != null)
            {
                // 2. Tách và chuyển đổi phần số (ví dụ: "LH015" -> "015")
                // Đảm bảo MALH có ít nhất 3 ký tự (LH + số)
                if (lastLienHe.Length >= 3 && lastLienHe.StartsWith("LH"))
                {
                    string numberPartString = lastLienHe.Substring(2); // Lấy phần số sau "LH"

                    if (int.TryParse(numberPartString, out int currentNumber))
                    {
                        // 3. Tăng số và định dạng lại thành chuỗi 3 chữ số (D3)
                        int nextNumber = currentNumber + 1;
                        string nextNumberString = nextNumber.ToString("D3"); // Ví dụ: 1 -> "001", 15 -> "015"
                        MALH = "LH" + nextNumberString;
                    }
                    // Nếu không thể phân tích được phần số, giữ nguyên MALH = "LH001" (hoặc nên ném lỗi)
                }
            }
            // --- KẾT THÚC SINH MÃ MALH ---

            // Gán mã vừa sinh cho đối tượng mới
            newLienHe.MALH = MALH;

            // Tạo và lưu liên hệ mới
            db.LIENHE.Add(newLienHe);

            try
            {
                db.SaveChanges();

                TempData["SuccessMessage"] = "Gửi liên hệ thành công!";
                ViewBag.Msg = "Thông tin của bạn đã được lưu lại!!! <3";
                return View();
            }
            catch (Exception ex)
            {
                // Ghi log lỗi chi tiết hơn nếu có thể
                // Logger.Error(ex, "Lỗi khi lưu liên hệ mới.");

                ViewBag.Msg = "Gửi liên hệ thất bại do lỗi hệ thống. Vui lòng thử lại sau.";
                // Trả về thông tin chi tiết về lỗi chỉ trong môi trường phát triển (Dev)
                // ViewBag.ErrorDetail = ex.Message; 
                return View();
            }
        }
        public ActionResult About()
        {
            return View();
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