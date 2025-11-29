using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smarts_DoAn_Backup_27_11_2025.Models
{
    public class ProductListVM
    {
        public IEnumerable<SANPHAM> SanPhams { get; set; }
        public IEnumerable<DANHMUC> DanhMucs { get; set; }
    }
}