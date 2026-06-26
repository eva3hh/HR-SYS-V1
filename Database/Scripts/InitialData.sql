# Smart HR ERP - Database Setup Script

## إنشاء قاعدة البيانات

```sql
CREATE DATABASE SmartHRDB;
GO

USE SmartHRDB;
GO
```

## إنشاء المستخدم والصلاحيات

```sql
CREATE LOGIN HRUser WITH PASSWORD = 'ComplexPassword@123';
CREATE USER HRUser FOR LOGIN HRUser;
ALTER ROLE db_owner ADD MEMBER HRUser;
GO
```

## التهجير (Migration)

```bash
cd Backend/SmartHRAPI
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## إضافة بيانات اولية

```sql
-- إدراج الأدوار
INSERT INTO Roles (Name, NameAr, Description, IsActive, DisplayOrder)
VALUES 
  ('Admin', 'مسؤول', 'مسؤول النظام', 1, 1),
  ('HR', 'موارد بشرية', 'إدارة الموارد البشرية', 1, 2),
  ('Accountant', 'محاسب', 'إدارة الرواتب والمحاسبة', 1, 3),
  ('Manager', 'مدير', 'مدير الإدارة', 1, 4),
  ('Supervisor', 'مشرف', 'مشرف الموظفين', 1, 5),
  ('Employee', 'موظف', 'موظف عادي', 1, 6);
GO

-- إدراج الصلاحيات
INSERT INTO Permissions (Name, Description, Module, Action, IsActive)
VALUES 
  ('View Employees', 'عرض الموظفين', 'Employees', 'View', 1),
  ('Create Employee', 'إضافة موظف', 'Employees', 'Create', 1),
  ('Edit Employee', 'تعديل الموظف', 'Employees', 'Edit', 1),
  ('Delete Employee', 'حذف الموظف', 'Employees', 'Delete', 1),
  ('View Salaries', 'عرض الرواتب', 'Salaries', 'View', 1),
  ('Calculate Salary', 'حساب الراتب', 'Salaries', 'Calculate', 1),
  ('Approve Salary', 'اعتماد الراتب', 'Salaries', 'Approve', 1),
  ('View Attendance', 'عرض الحضور', 'Attendance', 'View', 1),
  ('Record Attendance', 'تسجيل الحضور', 'Attendance', 'Record', 1),
  ('View Reports', 'عرض التقارير', 'Reports', 'View', 1),
  ('Export Reports', 'تصدير التقارير', 'Reports', 'Export', 1);
GO

-- ربط الأدوار بالصلاحيات
DECLARE @AdminRoleId INT, @HRRoleId INT, @AccountantRoleId INT;
SELECT @AdminRoleId = Id FROM Roles WHERE Name = 'Admin';
SELECT @HRRoleId = Id FROM Roles WHERE Name = 'HR';
SELECT @AccountantRoleId = Id FROM Roles WHERE Name = 'Accountant';

INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT @AdminRoleId, Id FROM Permissions WHERE IsActive = 1;

INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT @HRRoleId, Id FROM Permissions WHERE Module IN ('Employees', 'Attendance', 'Reports');

INSERT INTO RolePermissions (RoleId, PermissionId)
SELECT @AccountantRoleId, Id FROM Permissions WHERE Module IN ('Salaries', 'Reports');
GO

-- إضافة شركة افتراضية
INSERT INTO Companies (Name, NameAr, Address, Email, PhoneNumber, CRNumber, TaxNumber, FoundedDate)
VALUES ('Smart Company', 'شركة ذكية', 'الرياض', 'info@smartcompany.com', '966112345678', '1010123456', '300123456700003', GETDATE());
GO

-- إضافة فروع
DECLARE @CompanyId INT;
SELECT @CompanyId = Id FROM Companies WHERE Name = 'Smart Company';

INSERT INTO Branches (CompanyId, Name, NameAr, Address, City, PhoneNumber, Manager, IsActive)
VALUES 
  (@CompanyId, 'Head Office', 'المقر الرئيسي', 'الرياض', 'الرياض', '966112345678', 'Ahmed Al-Dosari', 1),
  (@CompanyId, 'Jeddah Branch', 'فرع جدة', 'جدة', 'جدة', '966122345678', 'Fatima Al-Ghamdi', 1),
  (@CompanyId, 'Dammam Branch', 'فرع الدمام', 'الدمام', 'الدمام', '966132345678', 'Mohammed Al-Shammari', 1);
GO

-- إضافة الأقسام
DECLARE @BranchId INT;
SELECT @BranchId = Id FROM Branches WHERE Name = 'Head Office';

INSERT INTO Departments (BranchId, Name, NameAr, Code, Manager, IsActive)
VALUES 
  (@BranchId, 'Human Resources', 'الموارد البشرية', 'HR', 'Ahmed', 1),
  (@BranchId, 'Finance', 'المالية', 'FIN', 'Fatima', 1),
  (@BranchId, 'IT', 'تقنية المعلومات', 'IT', 'Mohammed', 1),
  (@BranchId, 'Sales', 'المبيعات', 'SALES', 'Sara', 1),
  (@BranchId, 'Operations', 'العمليات', 'OPS', 'Hassan', 1);
GO

-- إضافة المسميات الوظيفية
INSERT INTO JobTitles (Name, NameAr, Description, SalaryBandId, IsActive)
VALUES 
  ('Manager', 'مدير', 'مدير الإدارة', 1, 1),
  ('Senior Staff', 'موظف أول', 'موظف أول', 2, 1),
  ('Staff', 'موظف', 'موظف عادي', 3, 1),
  ('Junior Staff', 'موظف مبتدئ', 'موظف مبتدئ', 4, 1),
  ('Intern', 'متدرب', 'متدرب', 5, 1);
GO

-- إضافة أنواع الإجازات
INSERT INTO LeaveTypes (Name, NameAr, Description, AnnualEntitlement, IsPaid, RequiresApproval, MaxDaysPerRequest, IsCarryForward, MaxCarryForwardDays, IsActive)
VALUES 
  ('Annual Leave', 'إجازة سنوية', 'إجازة سنوية مدفوعة', 30, 1, 1, 15, 1, 5, 1),
  ('Sick Leave', 'إجازة مرضية', 'إجازة مرضية مدفوعة', 10, 1, 1, 3, 0, 0, 1),
  ('Emergency Leave', 'إجازة طارئة', 'إجازة بدون راتب', 5, 0, 1, 3, 0, 0, 1),
  ('Maternity Leave', 'إجازة الأمومة', 'إجازة الأمومة مدفوعة', 60, 1, 1, 60, 0, 0, 1),
  ('Unpaid Leave', 'إجازة بدون راتب', 'إجازة بدون راتب', 0, 0, 1, 30, 0, 0, 1);
GO

-- إضافة أنواع الأذونات
INSERT INTO PermissionTypes (Name, NameAr, Description, MaxDaysPerYear, IsPaid, RequiresApproval, IsActive)
VALUES 
  ('Late Arrival', 'تأخير', 'إذن التأخير', 10, 1, 0, 1),
  ('Early Departure', 'خروج مبكر', 'إذن الخروج المبكر', 10, 1, 0, 1),
  ('Temporary Leave', 'خروج مؤقت', 'إذن الخروج المؤقت', 10, 1, 1, 1),
  ('Excuse', 'استئذان', 'استئذان', 20, 1, 1, 1);
GO

-- إضافة الشفتات (نوبات العمل)
INSERT INTO Shifts (Name, NameAr, StartTime, EndTime, CheckInTolerance, CheckOutTolerance, IsNightShift, MinimumHoursPerDay, OvertimeStartHour, WeeklyRestDay, IsActive)
VALUES 
  ('Morning Shift', 'الشفت الصباحي', '08:00:00', '16:00:00', '00:15:00', '00:15:00', 0, 8, 8, 4, 1),
  ('Evening Shift', 'الشفت المسائي', '16:00:00', '00:00:00', '00:15:00', '00:15:00', 0, 8, 8, 4, 1),
  ('Night Shift', 'الشفت الليلي', '00:00:00', '08:00:00', '00:15:00', '00:15:00', 1, 8, 8, 4, 1);
GO
```
