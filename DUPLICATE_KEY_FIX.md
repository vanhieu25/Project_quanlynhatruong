# Fix for PRIMARY KEY Constraint Violation

## Problem
You were getting the error:
```
Violation of PRIMARY KEY constraint 'PK__sinhvien__7A21767CBE8E8296'
Cannot insert duplicate key in object 'dbo.sinhvien'
The duplicate key value is (23)
```

This happens when trying to add a student or teacher with an ID that already exists in the database.

## Solution Applied

### StudentForm.cs
Added validation before inserting a new student to check if the student ID (masv) already exists:
- The system now checks the database before attempting to insert
- If a duplicate is found, it shows a user-friendly error message
- The focus returns to the student ID field so you can enter a different ID

### TeacherForm.cs
Added the same validation for teacher IDs (magv):
- Prevents duplicate teacher IDs from being inserted
- Shows clear error message when duplicate is detected
- Helps user correct the issue immediately

## How It Works

When you click "Thêm m?i" (Add New):
1. The system validates that all required fields are filled
2. **NEW**: It checks if the ID already exists in the database
3. If duplicate is found:
 - Shows message: "Mã sinh viên 'XX' ?ã t?n t?i trong h? th?ng! Vui lòng s? d?ng mã sinh viên khác."
   - Focus returns to the ID field
   - No database insert is attempted
4. If ID is unique, proceeds with insertion

## Current Student IDs in Database
Based on your database.md file, these student IDs are already taken:
- SV001
- SV002
- SV003
- SV004
- SV005
- SV006
- SV007

## Current Teacher IDs in Database
These teacher IDs are already taken:
- GV001
- GV002
- GV003
- GV004
- GV005
- GV006
- GV007

## How to Add New Students/Teachers

When adding a new student, use IDs like:
- SV008, SV009, SV010, etc.

When adding a new teacher, use IDs like:
- GV008, GV009, GV010, etc.

## Testing the Fix

1. Run your application
2. Try to add a new student with ID "SV001" (which already exists)
3. You should see the validation message instead of the database error
4. Change the ID to "SV008" (or any unused ID)
5. The student should be added successfully

## Additional Notes

- The validation uses the existing `sp_GetStudentById` and `sp_GetTeacherById` stored procedures
- No database changes were needed
- The fix is client-side validation that prevents the database error
- The original database constraints remain in place as a final safety check
