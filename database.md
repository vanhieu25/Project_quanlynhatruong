use QLHoctap


-- 2️⃣ Bảng giangvien

create table giangvien (

    magv char(10) primary key,

    hotengv nvarchar(50),

    ngaysinh date,

    sdt varchar(15),

    email nvarchar(100),

    gioitinh nvarchar(10),        

    lophuongdan nvarchar(20),

    bomon nvarchar(50)

);


-- 3️⃣ Bảng users

create table users (

    userid char(10) primary key,

    username nvarchar(50),

    password nvarchar(50),

    role nvarchar(20)

);


-- 5️⃣ Bảng monhoc

create table monhoc (

    mamon char(10) primary key,

    tenmon nvarchar(100),

    sotinchi int,

    hocky int

);


-- ---

-- Các bảng có khóa ngoại

-- ---


-- 1️⃣ Bảng sinhvien (Tham chiếu đến giangvien)

create table sinhvien (

    masv char(10) primary key,

    hoten nvarchar(50),

    ngaysinh date,

    gioitinh nvarchar(10),

    dienthoai varchar(15),

    email nvarchar(100),

    lophoc nvarchar(20),

    chuyennganh nvarchar(100),

    bacdaotao nvarchar(50),

    anhdaidien nvarchar(200),

    diemtb float,

    magvhd char(10),

    foreign key (magvhd) references giangvien(magv)

);


-- 7️⃣ Bảng lophocphan (Tham chiếu đến monhoc, giangvien)

create table lophocphan (

    malophp char(10) primary key,

    mamon char(10),

    magv char(10),

    sotinchi int,

    hocky_nienhoc nvarchar(50),

    foreign key (mamon) references monhoc(mamon),

    foreign key (magv) references giangvien(magv)

);


-- 6️⃣ Bảng ketqua (Tham chiếu đến sinhvien, monhoc)

create table ketqua (

    masv char(10),

    mamon char(10),

    diemtx float,

    diemck float,

    diemtongket float,

    primary key (masv, mamon),

    foreign key (masv) references sinhvien(masv),

    foreign key (mamon) references monhoc(mamon)

);



INSERT INTO giangvien (magv, hotengv, ngaysinh, sdt, email, gioitinh, lophuongdan, bomon)

VALUES

('GV001', N'Nguyễn Văn A', '1980-05-10', '0901234567', 'nva@example.com', N'Nam', 'CNTT01-K65', N'Công nghệ phần mềm'),

('GV002', N'Trần Thị B', '1985-11-20', '0912345678', 'ttb@example.com', N'Nữ', 'KHMT01-K66', N'Khoa học máy tính'),

('GV003', N'Đinh Văn C', '1979-02-15', '0902234567', 'dvc@example.com', N'Nam', 'CNTT02-K65', N'Công nghệ phần mềm'),

('GV004', N'Hoàng Thị D', '1988-07-30', '0913345678', 'htd@example.com', N'Nữ', 'ATTT01-K67', N'An toàn thông tin'),

('GV005', N'Lê Văn E', '1982-12-01', '0904456789', 'lve@example.com', N'Nam', 'KHMT02-K66', N'Khoa học máy tính'),

('GV006', N'Phạm Thị F', '1990-03-25', '0915567890', 'ptf@example.com', N'Nữ', 'CNTT01-K67', N'Mạng máy tính'),

('GV007', N'Vũ Văn G', '1986-09-14', '0906678901', 'vvg@example.com', N'Nam', 'KTPM01-K65', N'Công nghệ phần mềm');


INSERT INTO users (userid, username, password, role)

VALUES

('admin01', 'admin', 'adminpass123', 'Admin'),

('SV001', 'svvanan', 'svpass123', 'SinhVien'),

('GV001', 'gvvana', 'gvpass123', 'GiangVien'),

('SV002', 'svthicuc', 'svpass456', 'SinhVien'),

('GV002', 'gvthib', 'gvpass456', 'GiangVien'),

('SV003', 'svvanbinh', 'svpass789', 'SinhVien'),

('GV003', 'gvvanc', 'gvpass789', 'GiangVien'),

('SV004', 'svthihoa', 'svpass101', 'SinhVien');



INSERT INTO monhoc (mamon, tenmon, sotinchi, hocky)

VALUES

('MH001', N'Cơ sở dữ liệu', 3, 1),

('MH002', N'Lập trình hướng đối tượng', 4, 1),

('MH003', N'Mạng máy tính', 3, 2),

('MH004', N'Cấu trúc dữ liệu & Giải thuật', 4, 1),

('MH005', N'Hệ điều hành', 3, 2),

('MH006', N'An toàn thông tin', 3, 3),

('MH007', N'Phát triển ứng dụng Web', 4, 3),

('MH008', N'Trí tuệ nhân tạo', 3, 4);



INSERT INTO sinhvien (masv, hoten, ngaysinh, gioitinh, dienthoai, email, lophoc, chuyennganh, bacdaotao, anhdaidien, diemtb, magvhd)

VALUES

('SV001', N'Nguyễn Văn An', '2003-01-15', N'Nam', '0987654321', 'nva.sv@example.com', 'CNTT01-K65', N'Công nghệ phần mềm', N'Đại học', 'avatar/sv001.png', 3.2, 'GV001'),

('SV002', N'Lê Thị Cúc', '2003-08-22', N'Nữ', '0912345678', 'ltc.sv@example.com', 'KHMT01-K66', N'Khoa học máy tính', N'Đại học', 'avatar/sv002.png', 3.5, 'GV002'),

('SV003', N'Trần Văn Bình', '2003-03-10', N'Nam', '0912223333', 'tvb.sv@example.com', 'CNTT02-K65', N'Công nghệ phần mềm', N'Đại học', 'avatar/sv003.png', 3.0, 'GV003'),

('SV004', N'Nguyễn Thị Hoa', '2004-11-05', N'Nữ', '0988776655', 'nth.sv@example.com', 'ATTT01-K67', N'An toàn thông tin', N'Đại học', 'avatar/sv004.png', 3.6, 'GV004'),

('SV005', N'Phạm Văn Tuấn', '2003-06-20', N'Nam', '0905123456', 'pvt.sv@example.com', 'KHMT02-K66', N'Khoa học máy tính', N'Đại học', 'avatar/sv005.png', 2.8, 'GV005'),

('SV006', N'Đặng Thị Lan', '2003-09-18', N'Nữ', '0977112233', 'dtl.sv@example.com', 'CNTT01-K65', N'Công nghệ phần mềm', N'Đại học', 'avatar/sv006.png', 3.1, 'GV001'),

('SV007', N'Hoàng Văn Mạnh', '2004-12-30', N'Nam', '0966554433', 'hvm.sv@example.com', 'KTPM01-K65', N'Công nghệ phần mềm', N'Đại học', 'avatar/sv007.png', 2.9, 'GV007');



INSERT INTO lophocphan (malophp, mamon, magv, sotinchi, hocky_nienhoc)

VALUES

('LHP001', 'MH001', 'GV001', 3, N'HK1 (2023-2024)'),

('LHP002', 'MH002', 'GV002', 4, N'HK1 (2023-2024)'),

('LHP003', 'MH001', 'GV001', 3, N'HK1 (2023-2024)'),

('LHP004', 'MH003', 'GV003', 3, N'HK2 (2023-2024)'),

('LHP005', 'MH004', 'GV004', 4, N'HK1 (2023-2024)'),

('LHP006', 'MH005', 'GV005', 3, N'HK2 (2023-2024)'),

('LHP007', 'MH006', 'GV006', 3, N'HK1 (2024-2025)'),

('LHP008', 'MH007', 'GV007', 4, N'HK1 (2024-2025)');



INSERT INTO ketqua (masv, mamon, diemtx, diemck, diemtongket)

VALUES

('SV001', 'MH001', 8.0, 7.5, 7.7),

('SV001', 'MH002', 9.0, 8.5, 8.7),

('SV002', 'MH001', 7.0, 6.5, 6.7),

('SV002', 'MH002', 8.5, 9.0, 8.8),

('SV003', 'MH001', 6.0, 7.0, 6.6),

('SV003', 'MH003', 7.5, 8.0, 7.8),

('SV004', 'MH004', 9.0, 9.5, 9.3),

('SV005', 'MH005', 5.0, 6.0, 5.6);

