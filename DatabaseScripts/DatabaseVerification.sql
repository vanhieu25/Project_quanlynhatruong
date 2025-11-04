-- =============================================
-- Quick Setup Script for QLHoctap Database
-- Run this script to verify and prepare your database
-- =============================================

USE QLHoctap
GO

-- =============================================
-- 1. Verify Database Tables Exist
-- =============================================

PRINT '========================================='
PRINT 'Checking Database Tables...'
PRINT '========================================='

IF OBJECT_ID('giangvien', 'U') IS NOT NULL
    PRINT '? Table "giangvien" exists'
ELSE
 PRINT '? ERROR: Table "giangvien" not found!'

IF OBJECT_ID('sinhvien', 'U') IS NOT NULL
    PRINT '? Table "sinhvien" exists'
ELSE
    PRINT '? ERROR: Table "sinhvien" not found!'

IF OBJECT_ID('monhoc', 'U') IS NOT NULL
 PRINT '? Table "monhoc" exists'
ELSE
    PRINT '? ERROR: Table "monhoc" not found!'

IF OBJECT_ID('lophocphan', 'U') IS NOT NULL
    PRINT '? Table "lophocphan" exists'
ELSE
    PRINT '? ERROR: Table "lophocphan" not found!'

IF OBJECT_ID('ketqua', 'U') IS NOT NULL
    PRINT '? Table "ketqua" exists'
ELSE
    PRINT '? ERROR: Table "ketqua" not found!'

GO

-- =============================================
-- 2. Display Table Row Counts
-- =============================================

PRINT ''
PRINT '========================================='
PRINT 'Current Data Statistics:'
PRINT '========================================='

DECLARE @TeacherCount INT, @StudentCount INT, @SubjectCount INT, @ClassCount INT, @ResultCount INT

SELECT @TeacherCount = COUNT(*) FROM giangvien
SELECT @StudentCount = COUNT(*) FROM sinhvien
SELECT @SubjectCount = COUNT(*) FROM monhoc
SELECT @ClassCount = COUNT(*) FROM lophocphan
SELECT @ResultCount = COUNT(*) FROM ketqua

PRINT 'Teachers (giangvien): ' + CAST(@TeacherCount AS VARCHAR)
PRINT 'Students (sinhvien): ' + CAST(@StudentCount AS VARCHAR)
PRINT 'Subjects (monhoc): ' + CAST(@SubjectCount AS VARCHAR)
PRINT 'Classes (lophocphan): ' + CAST(@ClassCount AS VARCHAR)
PRINT 'Results (ketqua): ' + CAST(@ResultCount AS VARCHAR)

GO

-- =============================================
-- 3. Check for Required Columns
-- =============================================

PRINT ''
PRINT '========================================='
PRINT 'Verifying Column Structures...'
PRINT '========================================='

-- Check giangvien table
IF COL_LENGTH('giangvien', 'magv') IS NOT NULL
    AND COL_LENGTH('giangvien', 'hotengv') IS NOT NULL
    AND COL_LENGTH('giangvien', 'ngaysinh') IS NOT NULL
    AND COL_LENGTH('giangvien', 'sdt') IS NOT NULL
    AND COL_LENGTH('giangvien', 'email') IS NOT NULL
    AND COL_LENGTH('giangvien', 'gioitinh') IS NOT NULL
    AND COL_LENGTH('giangvien', 'lophuongdan') IS NOT NULL
    AND COL_LENGTH('giangvien', 'bomon') IS NOT NULL
    PRINT '? Table "giangvien" has all required columns'
ELSE
  PRINT '? WARNING: Table "giangvien" missing some columns'

-- Check sinhvien table
IF COL_LENGTH('sinhvien', 'masv') IS NOT NULL
    AND COL_LENGTH('sinhvien', 'hoten') IS NOT NULL
    AND COL_LENGTH('sinhvien', 'ngaysinh') IS NOT NULL
    AND COL_LENGTH('sinhvien', 'gioitinh') IS NOT NULL
    AND COL_LENGTH('sinhvien', 'dienthoai') IS NOT NULL
    AND COL_LENGTH('sinhvien', 'email') IS NOT NULL
    AND COL_LENGTH('sinhvien', 'lophoc') IS NOT NULL
    AND COL_LENGTH('sinhvien', 'chuyennganh') IS NOT NULL
    AND COL_LENGTH('sinhvien', 'bacdaotao') IS NOT NULL
    AND COL_LENGTH('sinhvien', 'diemtb') IS NOT NULL
    AND COL_LENGTH('sinhvien', 'magvhd') IS NOT NULL
    PRINT '? Table "sinhvien" has all required columns'
ELSE
    PRINT '? WARNING: Table "sinhvien" missing some columns'

GO

-- =============================================
-- 4. Check Foreign Key Relationships
-- =============================================

PRINT ''
PRINT '========================================='
PRINT 'Checking Foreign Key Relationships...'
PRINT '========================================='

-- Check if there are students without valid advisor
IF EXISTS (SELECT 1 FROM sinhvien WHERE magvhd IS NOT NULL 
  AND magvhd NOT IN (SELECT magv FROM giangvien))
BEGIN
    DECLARE @OrphanCount INT
    SELECT @OrphanCount = COUNT(*) FROM sinhvien 
    WHERE magvhd IS NOT NULL AND magvhd NOT IN (SELECT magv FROM giangvien)
    PRINT '? WARNING: ' + CAST(@OrphanCount AS VARCHAR) + ' student(s) have invalid advisor reference'
END
ELSE
    PRINT '? All student advisor references are valid'

-- Check if there are classes without valid teacher
IF EXISTS (SELECT 1 FROM lophocphan WHERE magv IS NOT NULL 
    AND magv NOT IN (SELECT magv FROM giangvien))
BEGIN
  DECLARE @OrphanClassCount INT
    SELECT @OrphanClassCount = COUNT(*) FROM lophocphan 
    WHERE magv IS NOT NULL AND magv NOT IN (SELECT magv FROM giangvien)
    PRINT '? WARNING: ' + CAST(@OrphanClassCount AS VARCHAR) + ' class(es) have invalid teacher reference'
END
ELSE
    PRINT '? All class teacher references are valid'

GO

-- =============================================
-- 5. List Existing Stored Procedures
-- =============================================

PRINT ''
PRINT '========================================='
PRINT 'Current Stored Procedures:'
PRINT '========================================='

SELECT 
    name AS ProcedureName,
    create_date AS CreatedDate,
    modify_date AS ModifiedDate
FROM sys.procedures
WHERE name LIKE 'sp_%'
ORDER BY name

GO

-- =============================================
-- 6. Test Data Sample
-- =============================================

PRINT ''
PRINT '========================================='
PRINT 'Sample Data:'
PRINT '========================================='

PRINT ''
PRINT 'First 3 Teachers:'
SELECT TOP 3 magv, hotengv, bomon, email FROM giangvien ORDER BY magv

PRINT ''
PRINT 'First 3 Students:'
SELECT TOP 3 masv, hoten, lophoc, chuyennganh, diemtb FROM sinhvien ORDER BY masv

GO

-- =============================================
-- 7. Ready to Install Stored Procedures
-- =============================================

PRINT ''
PRINT '========================================='
PRINT 'Database Verification Complete!'
PRINT '========================================='
PRINT ''
PRINT 'Next Step: Run StoredProcedures.sql to install all stored procedures'
PRINT ''

GO
