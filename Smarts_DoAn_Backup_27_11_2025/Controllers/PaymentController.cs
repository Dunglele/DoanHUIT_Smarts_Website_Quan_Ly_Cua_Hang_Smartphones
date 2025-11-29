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
    public class PaymentController : Controller
    {
        private SMARTS_TESTEntities db = new SMARTS_TESTEntities();
        // GET: Payment
        public ActionResult Cart()
        {
            return View();
        }
        public ActionResult Checkout()
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
        public ActionResult Checkout(string HO, string TEN, string EMAIL, string SODIENTHOAI, string DIACHI, string PHUONGTHUCTHANHTOAN, string TINHTRANG, string MASP, string TENSP, int GIA, int SOLUONG, int TAMTINH, int THANHTIEN)
        {
            // 1. Get last order
            var lastHoaDon = db.DATHANG
                .OrderByDescending(h => h.MADH)
                .FirstOrDefault();

            // 2. Extract number part
            string lastMa = lastHoaDon != null ? lastHoaDon.MADH : "DH001";
            string numberPartString = lastMa.Substring(2);

            // 3. Parse and increment
            int nextNumber = 1;
            if (int.TryParse(numberPartString, out int currentNumber))
            {
                nextNumber = currentNumber + 1;
            }
            string nextNumberString = nextNumber.ToString("D3");
            var MADH = "DH" + nextNumberString;

            // 4. Create new DATHANG
            var newDatHang = new DATHANG
            {
                MADH = MADH,
                HO = HO,
                TEN = TEN,
                EMAIL = EMAIL,
                SODIENTHOAI = SODIENTHOAI,
                DIACHI = DIACHI,
                PHUONGTHUCTHANHTOAN = PHUONGTHUCTHANHTOAN
            };
            db.DATHANG.Add(newDatHang);

            //====================================================================

            // 1. Get last order
            var lastHoaDon1 = db.HOADON
                .OrderByDescending(h => h.MAHD)
                .FirstOrDefault();

            // 2. Extract number part
            string lastMa1 = lastHoaDon1 != null ? lastHoaDon1.MAHD : "HD001";
            string numberPartString1 = lastMa1.Substring(2);

            // 3. Parse and increment
            int nextNumber1 = 1;
            if (int.TryParse(numberPartString1, out int currentNumber1))
            {
                nextNumber1 = currentNumber1 + 1;
            }
            string nextNumberString1 = nextNumber1.ToString("D3");
            var MAHD = "HD" + nextNumberString1;

            // 4. Create new HOADON
            var newHoaDon = new HOADON
            {
                MAHD = MAHD, // Assuming MAHD is same as MADH for simplicity
                MADH = MADH,
                TINHTRANG = TINHTRANG
            };
            db.HOADON.Add(newHoaDon);

            //====================================================================

            // Create new CHITIETHOADON
            var newChiTietHoaDon = new CHITIETHOADON
            {
                MAHD = MAHD,
                MASP = MASP,
                TENSP = TENSP,
                GIA = GIA,
                SOLUONG = SOLUONG,
                TAMTINH = TAMTINH,
                THANHTIEN = THANHTIEN
            };
            db.CHITIETHOADON.Add(newChiTietHoaDon);

            var upslsp = db.SANPHAM.FirstOrDefault(h => h.MASP == MASP);
            if(upslsp != null)
            {
                upslsp.SOLUONG = upslsp.SOLUONG - SOLUONG;
            }

            var upsldm = db.DANHMUC.FirstOrDefault(u => u.MADM == upslsp.MADM);
            if(upsldm != null)
            {
                upsldm.SOLUONG_SP = upsldm.SOLUONG_SP - SOLUONG;
            }
            // 7. Save changes
            db.SaveChanges();

            return RedirectToAction("Index","Home");
        }

    }
}