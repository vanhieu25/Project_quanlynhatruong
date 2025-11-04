-- =============================================
-- Stored Procedures for QLHoctap Database
-- =============================================

USE QLHoctap
GO

-- =============================================
-- Dashboard Procedures
-- =============================================

-- Get dashboard statistics
IF OBJECT_ID('sp_GetDashboardStats', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetDashboardStats
GO

CREATE PROCEDURE sp_GetDashboardStats
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        'Teachers' AS Category,
        COUNT(*) AS Total
    FROM giangvien
    
    UNION ALL
    
    SELECT 
        'Students' AS Category,
        COUNT(*) AS Total
    FROM sinhvien
    
    UNION ALL
    
    SELECT 
        'Subjects' AS Category,
        COUNT(*) AS Total
    FROM monhoc
    
    UNION ALL
    
    SELECT 
      'Classes' AS Category,
        COUNT(*) AS Total
    FROM lophocphan;
END
GO

-- =============================================
-- Teacher Procedures
-- =============================================

-- Get all teachers
IF OBJECT_ID('sp_GetAllTeachers', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllTeachers
GO

CREATE PROCEDURE sp_GetAllTeachers
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT magv, hotengv, ngaysinh, sdt, email, gioitinh, lophuongdan, bomon
    FROM giangvien
    ORDER BY magv;
END
GO

-- Get teacher by ID
IF OBJECT_ID('sp_GetTeacherById', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetTeacherById
GO

CREATE PROCEDURE sp_GetTeacherById
    @magv NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT *
    FROM giangvien
    WHERE magv = @magv;
END
GO

-- Insert new teacher
IF OBJECT_ID('sp_InsertTeacher', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertTeacher
GO

CREATE PROCEDURE sp_InsertTeacher
    @magv NVARCHAR(50),
    @hotengv NVARCHAR(100),
    @ngaysinh DATE,
    @sdt NVARCHAR(20),
  @email NVARCHAR(100),
    @gioitinh NVARCHAR(10),
    @lophuongdan NVARCHAR(50),
    @bomon NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO giangvien (magv, hotengv, ngaysinh, sdt, email, gioitinh, lophuongdan, bomon)
    VALUES (@magv, @hotengv, @ngaysinh, @sdt, @email, @gioitinh, @lophuongdan, @bomon);
END
GO

-- Update teacher
IF OBJECT_ID('sp_UpdateTeacher', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateTeacher
GO

CREATE PROCEDURE sp_UpdateTeacher
    @magv NVARCHAR(50),
    @hotengv NVARCHAR(100),
    @ngaysinh DATE,
    @sdt NVARCHAR(20),
 @email NVARCHAR(100),
  @gioitinh NVARCHAR(10),
    @lophuongdan NVARCHAR(50),
    @bomon NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE giangvien 
    SET hotengv = @hotengv, 
        ngaysinh = @ngaysinh,
        sdt = @sdt, 
     email = @email, 
 gioitinh = @gioitinh, 
        lophuongdan = @lophuongdan, 
    bomon = @bomon
 WHERE magv = @magv;
END
GO

-- Check teacher constraints
IF OBJECT_ID('sp_CheckTeacherConstraints', 'P') IS NOT NULL
    DROP PROCEDURE sp_CheckTeacherConstraints
GO

CREATE PROCEDURE sp_CheckTeacherConstraints
    @magv NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
     (SELECT COUNT(*) FROM sinhvien WHERE magvhd = @magv) AS StudentCount,
  (SELECT COUNT(*) FROM lophocphan WHERE magv = @magv) AS ClassCount;
END
GO

-- Remove teacher references
IF OBJECT_ID('sp_RemoveTeacherReferences', 'P') IS NOT NULL
    DROP PROCEDURE sp_RemoveTeacherReferences
GO

CREATE PROCEDURE sp_RemoveTeacherReferences
    @magv NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        UPDATE sinhvien SET magvhd = NULL WHERE magvhd = @magv;
        UPDATE lophocphan SET magv = NULL WHERE magv = @magv;
    
   COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
    THROW;
    END CATCH
END
GO

-- Delete teacher
IF OBJECT_ID('sp_DeleteTeacher', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteTeacher
GO

CREATE PROCEDURE sp_DeleteTeacher
    @magv NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    DELETE FROM giangvien WHERE magv = @magv;
END
GO

-- =============================================
-- Student Procedures
-- =============================================

-- Get all students
IF OBJECT_ID('sp_GetAllStudents', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllStudents
GO

CREATE PROCEDURE sp_GetAllStudents
AS
BEGIN
    SET NOCOUNT ON;
    
  SELECT s.masv, s.hoten, s.lophoc, s.chuyennganh, s.diemtb,
    s.gioitinh, s.dienthoai, g.hotengv
    FROM sinhvien s 
    LEFT JOIN giangvien g ON s.magvhd = g.magv
    ORDER BY s.masv;
END
GO

-- Get student by ID
IF OBJECT_ID('sp_GetStudentById', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetStudentById
GO

CREATE PROCEDURE sp_GetStudentById
    @masv NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
 SELECT *
    FROM sinhvien
    WHERE masv = @masv;
END
GO

-- Get all advisors
IF OBJECT_ID('sp_GetAllAdvisors', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetAllAdvisors
GO

CREATE PROCEDURE sp_GetAllAdvisors
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT magv, hotengv 
    FROM giangvien 
    ORDER BY magv;
END
GO

-- Insert new student
IF OBJECT_ID('sp_InsertStudent', 'P') IS NOT NULL
    DROP PROCEDURE sp_InsertStudent
GO

CREATE PROCEDURE sp_InsertStudent
    @masv NVARCHAR(50),
    @hoten NVARCHAR(100),
    @ngaysinh DATE,
    @gioitinh NVARCHAR(10),
    @dienthoai NVARCHAR(20),
    @email NVARCHAR(100),
    @lophoc NVARCHAR(50),
    @chuyennganh NVARCHAR(100),
    @bacdaotao NVARCHAR(50),
    @diemtb FLOAT = NULL,
    @magvhd NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO sinhvien (masv, hoten, ngaysinh, gioitinh, dienthoai, email, 
     lophoc, chuyennganh, bacdaotao, anhdaidien, diemtb, magvhd)
  VALUES (@masv, @hoten, @ngaysinh, @gioitinh, @dienthoai, @email, 
       @lophoc, @chuyennganh, @bacdaotao, NULL, @diemtb, @magvhd);
END
GO

-- Update student
IF OBJECT_ID('sp_UpdateStudent', 'P') IS NOT NULL
    DROP PROCEDURE sp_UpdateStudent
GO

CREATE PROCEDURE sp_UpdateStudent
    @masv NVARCHAR(50),
    @hoten NVARCHAR(100),
    @ngaysinh DATE,
    @gioitinh NVARCHAR(10),
    @dienthoai NVARCHAR(20),
    @email NVARCHAR(100),
    @lophoc NVARCHAR(50),
    @chuyennganh NVARCHAR(100),
    @bacdaotao NVARCHAR(50),
    @diemtb FLOAT = NULL,
    @magvhd NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE sinhvien 
    SET hoten = @hoten, 
        ngaysinh = @ngaysinh,
        gioitinh = @gioitinh, 
  dienthoai = @dienthoai, 
        email = @email,
        lophoc = @lophoc, 
        chuyennganh = @chuyennganh, 
     bacdaotao = @bacdaotao,
        diemtb = @diemtb, 
        magvhd = @magvhd
    WHERE masv = @masv;
END
GO

-- Delete student
IF OBJECT_ID('sp_DeleteStudent', 'P') IS NOT NULL
    DROP PROCEDURE sp_DeleteStudent
GO

CREATE PROCEDURE sp_DeleteStudent
    @masv NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
      DELETE FROM ketqua WHERE masv = @masv;
        DELETE FROM sinhvien WHERE masv = @masv;
   
      COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- Get student GPA statistics
IF OBJECT_ID('sp_GetStudentGPAStats', 'P') IS NOT NULL
    DROP PROCEDURE sp_GetStudentGPAStats
GO

CREATE PROCEDURE sp_GetStudentGPAStats
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT diemtb 
    FROM sinhvien 
    WHERE diemtb IS NOT NULL;
END
GO

PRINT 'All stored procedures created successfully!'
GO
