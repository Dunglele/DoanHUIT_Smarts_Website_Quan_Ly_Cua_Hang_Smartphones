
--- TAO DATABASE

USE MASTER
CREATE DATABASE SMARTS_TEST
GO
USE SMARTS_TEST
GO

----------------------------

--- TAO CAC TABLE CAN THIET

CREATE TABLE NGUOIDUNG
(
	MAND CHAR(5) PRIMARY KEY,
	EMAIL VARCHAR(30),
	VAITRO VARCHAR(30),
	MATKHAU CHAR(15),
	HOTEN NVARCHAR(60),
	SODIENTHOAI CHAR(11)
)

CREATE TABLE DANHMUC
(
	MADM CHAR(5) PRIMARY KEY,
	TENDM NVARCHAR(30),
	SOLUONG_SP INT
)

CREATE TABLE SANPHAM
(
	MASP CHAR(5) PRIMARY KEY,
	MADM CHAR(5) FOREIGN KEY REFERENCES DANHMUC(MADM),
	TENSP NVARCHAR(120),
	GIA DECIMAL,
	THUONGHIEU VARCHAR(30),
	MOTA NVARCHAR(1000),
	THONGSO NVARCHAR(1000),
	SOLUONG INT,
	ANHSP VARCHAR(500)
)

CREATE TABLE GIOHANG
(
	MASP CHAR(5) FOREIGN KEY REFERENCES SANPHAM(MASP),
	MAND CHAR(5) FOREIGN KEY REFERENCES NGUOIDUNG(MAND),
	ANHSP VARCHAR(500),
	TENSP NVARCHAR(120),
	GIA DECIMAL,
	SOLUONG INT,
	TAMTINH DECIMAL,
	PRIMARY KEY (MASP, MAND) -- Thêm Khóa Chính Tổng hợp
)

CREATE TABLE DATHANG
(
	MADH CHAR(5) PRIMARY KEY,
	HO NVARCHAR(30),
	TEN NVARCHAR(30),
	EMAIL NVARCHAR(30),
	SODIENTHOAI CHAR(11),
	DIACHI NVARCHAR(500),
	PHUONGTHUCTHANHTOAN NVARCHAR(300)
)

CREATE TABLE HOADON
(
	MAHD CHAR(5) PRIMARY KEY,
	MADH CHAR(5) FOREIGN KEY REFERENCES DATHANG(MADH),
	TINHTRANG NVARCHAR(50)
)

CREATE TABLE CHITIETHOADON
(
	MAHD CHAR(5) FOREIGN KEY REFERENCES HOADON(MAHD),
	MASP CHAR(5) FOREIGN KEY REFERENCES SANPHAM(MASP),
	TENSP NVARCHAR(120),
	SOLUONG INT,
	GIA DECIMAL,
	TAMTINH DECIMAL,
	THANHTIEN DECIMAL,
	PRIMARY KEY (MAHD, MASP) -- Thêm Khóa Chính Tổng hợp
)

CREATE TABLE LIENHE
(
	MALH CHAR(5) PRIMARY KEY, -- Thêm một cột Khóa Chính tự tăng
	EMAIL VARCHAR(30),
	HOTEN NVARCHAR(60),
	SODIENTHOAI CHAR(11),
	DIACHI NVARCHAR(300)
)

----------------------------

--- THEM DU LIEU VAO BANG

-- ADD Người dùng
INSERT INTO NGUOIDUNG VALUES ('ND001', 'admin@smarts.com', 'Admin', '123', N'Lê Đỗ Quang Dũng', '0981717978'),
('ND002', 'client@smarts.com', 'Client', '123', N'client test', '01234567891')

-- ADD Danh mục
INSERT INTO DANHMUC VALUES ('PHONE', N'Điện thoại di động', 0),
('IPAD', N'Máy tính bảng', 0), ('ACSR', N'Phụ kiện', 0)

-- ADD Sản phẩm
INSERT INTO SANPHAM (MASP, MADM, TENSP, GIA, THUONGHIEU, MOTA, THONGSO, SOLUONG, ANHSP)
VALUES
('P-001', 'PHONE', N'Iphone XS Max', 8000000, N'Apple', N'Test', N'Test', 10, '/img_product/product/20251128143021_h_nh__nh_68_423aa63eb0f243738f837f3aa1d66e9a_large_6f4c46a6e35443dcbc5a9f9562e2fbd8_master.jpeg'),
('P-002', 'PHONE', N'Iphone 13', 13000000, 'Apple', N'Test', N'Test', 5, '/img_product/product/20251128143146_pro_max_blue_6881422e0a9c4ac58941e8188f671802_master.png'),
('P-003', 'PHONE', N'Iphone 17 Pro Max', 38000000, 'Apple', N'Test', N'Test', 6, '/img_product/product/20251128143237_h_nh__nh_1_76ff3efeae0244f8bf4edd401bf98275_master.jpeg'),
('P-004', 'PHONE', N'Samsung ZFlip4', 8000000, 'Samsung', N'Test', N'Test', 7, 'https://picsum.photos/200/300
'),

('I-001', 'IPAD', N'Ipad M1', 8000000, 'Apple', N'Test', N'Test', 7, '/img_product/product/20251128142706_hinh_anh_128921753cb440a194c73aea732d17db_master.jpeg'),
('I-002', 'IPAD', N'Ipad M2', 10000000, 'Apple', N'Test', N'Test', 5, '/img_product/product/20251128142813_ipad-pro-m2-12.9-cpo-1_d9dae41cd040451eab47701c16a5e09d_master.jpg'),
('I-003', 'IPAD', N'Ipad Samsung', 6000000, 'Samsung', N'Test', N'Test', 2, '/img_product/product/20251128142858_rose_gold_0e243545eb664c87af7dd0e6deb8f676_master.png'),

('A-001', 'ACSR', N'Tai nghe Alphawolf 2025', 700000, 'Alphawolf', N'Test', N'Test', 1, '/img_product/product/20251128141847_358076127_580506980911511_9071883232931024958_n.jpg'),
('A-002', 'ACSR', N'Sạc nhanh 30W đầu ra Type C', 50000, 'Base us', N'Test', N'Test', 2, '/img_product/product/20251128142118_598763_ice Crystal PD 30W Lightning 2.jpg'),
('A-003', 'ACSR', N'Sạc dự phòng 5000mah', 200000, 'Base us', N'Test', N'Test', 3, '/img_product/product/20251128142303_magipi-20w-d199-7_9016.jpg');

-- ADD Đơn đặt hàng
INSERT INTO DATHANG VALUES ('DH001', N'Nguyễn Văn', N' A', 'nguyenvana@smarts.com', '09ABCXYZMNF', '40 Nguyễn Đình Chính', N'Thanh toán khi nhận hàng'),
('DH002', N'Nguyễn Thị', N' B', 'nguyenthiB@smarts.com', '09ABCXYZMNX', '40 Phan Xích Long', N'Chuyển khoản ngân hàng')

-- ADD Hóa đơn
INSERT INTO HOADON VALUES ('HD001', 'DH001', N'Đang xử lý'),
('HD002', 'DH002', N'Hoàn thành')

-- ADD Chi tiết hóa đơn
INSERT INTO CHITIETHOADON VALUES ('HD001', 'P-001', 'Iphone XS Max', 1, 8000000, 8000000, 8800000),
('HD001', 'P-002', 'Iphone 13', 1, 13000000, 13000000, 13130000),
('HD002', 'P-001', 'Iphone XS Max', 1, 8000000, 8000000, 8800000),
('HD002', 'P-002', 'Iphone 13', 1, 13000000, 13000000, 13130000)

-- ADD Giỏ hàng
INSERT INTO GIOHANG VALUES ('P-001', 'ND001', 'https://picsum.photos/200/300', 'Iphone XS Max', 8000000, 1, 8000000)

----------------------------

--- Xem dữ liệu

SELECT * FROM NGUOIDUNG
SELECT * FROM SANPHAM
SELECT * FROM DANHMUC
SELECT * FROM DATHANG
SELECT * FROM HOADON
SELECT * FROM GIOHANG
SELECT * FROM CHITIETHOADON
SELECT * FROM LIENHE

----------------------------

--- Cập nhật số lượng sản phẩm bảng danh mục

UPDATE DANHMUC
SET SOLUONG_SP = (SELECT SUM(SOLUONG) FROM SANPHAM, DANHMUC WHERE SANPHAM.MADM = DANHMUC.MADM AND TENDM = N'Điện thoại di động' GROUP BY TENDM)
WHERE MADM = 'PHONE'

UPDATE DANHMUC
SET SOLUONG_SP = (SELECT SUM(SOLUONG) FROM SANPHAM, DANHMUC WHERE SANPHAM.MADM = DANHMUC.MADM AND TENDM = N'Máy tính bảng' GROUP BY TENDM)
WHERE MADM = 'IPAD'

UPDATE DANHMUC
SET SOLUONG_SP = (SELECT SUM(SOLUONG) FROM SANPHAM, DANHMUC WHERE SANPHAM.MADM = DANHMUC.MADM AND TENDM = N'Phụ kiện' GROUP BY TENDM)
WHERE MADM = 'ACSR'

----------------------------
---------- Trigger ---------
----------------------------

--- Trigger cập nhật tổng số lượng sản phẩm trong DANHMUC mỗi khi thêm, xóa, sửa sản phẩm.

CREATE TRIGGER UPDATE_DANHMUC ON SANPHAM
FOR INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Tạo một tập hợp các MADM bị ảnh hưởng từ cả inserted (dữ liệu mới) và deleted (dữ liệu cũ)
    WITH Affected_DANHMUC AS (
        -- Lấy MADM từ các sản phẩm được chèn/cập nhật (dữ liệu mới)
        SELECT MADM FROM inserted
        UNION
        -- Lấy MADM từ các sản phẩm bị xóa/cập nhật (dữ liệu cũ)
        SELECT MADM FROM deleted
    )
    -- 2. Cập nhật bảng DANHMUC cho các MADM bị ảnh hưởng
    UPDATE D
    SET D.SOLUONG_SP = ISNULL(
        (SELECT SUM(S.SOLUONG) 
         FROM SANPHAM S 
         WHERE S.MADM = A.MADM), 0)
    FROM DANHMUC D
    INNER JOIN Affected_DANHMUC A ON D.MADM = A.MADM;

END

-- Trigger không cho số lượng sản phẩm < 0
CREATE TRIGGER TRG_CHECK_SOLUONG ON SANPHAM
FOR INSERT, UPDATE
AS
BEGIN
	IF EXISTS (SELECT * FROM inserted WHERE SOLUONG < 0)
	BEGIN
		ROLLBACK TRAN
		RAISERROR('So luong khong the am!',16,1)
	END
END

----------------------------
--------- Producer ---------
----------------------------
use SMARTS_TEST
-- Proc xóa sản phẩm
CREATE PROCEDURE SP_XOA_SP
	@MASP CHAR(5)
AS
PRINT N'Xoa san pham ' + @MASP + N' thanh cong'
DELETE FROM SANPHAM WHERE MASP=@MASP


DECLARE @masp CHAR(5)
EXEC SP_XOA_SP @masp='P-004'

-- Proc Thêm sản phẩm mới
CREATE PROC SP_THEM_SP
	@masp CHAR(5), @madm CHAR(5), @tensp NVARCHAR(120), @gia DECIMAL, @thuonghieu VARCHAR(30), @mota NVARCHAR(1000), @thongso NVARCHAR(1000), @soluong INT, @anhsp VARCHAR(500)
AS
INSERT INTO SANPHAM (MASP, MADM, TENSP, GIA, THUONGHIEU, MOTA, THONGSO, SOLUONG, ANHSP)
VALUES
(@masp, @madm, @tensp, @gia, @thuonghieu, @mota, @thongso, @soluong, @anhsp)
PRINT N'Thêm sản phẩm ' + @masp + N' thành công'

DECLARE @masp CHAR(5), @madm CHAR(5), @tensp NVARCHAR(120), @gia DECIMAL, @thuonghieu VARCHAR(30), @mota NVARCHAR(1000), @thongso NVARCHAR(1000), @soluong INT, @anhsp VARCHAR(500)
EXEC SP_THEM_SP @masp='P-004', @madm='PHONE', @tensp=N'test', @gia=1, @thuonghieu='TEST', @mota=N'test', @thongso=N'test', @soluong=1, @anhsp='https://picsum.photos/200/300'

SELECT * FROM SANPHAM
