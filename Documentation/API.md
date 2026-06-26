# Smart HR ERP - API Documentation

## نظرة عامة

واجهة برمجية (API) كاملة لنظام إدارة الموارد البشرية الاحترافي بناءً على ASP.NET Core 9.

## المتطلبات

- .NET 9.0 SDK أو أحدث
- SQL Server 2019 أو أحدث
- Node.js 18+ (للـ Frontend)

## البدء السريع

### 1. إعداد قاعدة البيانات

```bash
cd Backend/SmartHRAPI
dotnet ef database update
```

### 2. تشغيل الـ Backend

```bash
dotnet run
```

الـ API سيكون متاحاً على `http://localhost:5000`

### 3. تشغيل الـ Frontend

```bash
cd Frontend/smart-hr-app
npm install
npm run dev
```

التطبيق سيكون متاحاً على `http://localhost:5173`

## التشغيل باستخدام Docker

```bash
docker-compose up -d
```

## Swagger Documentation

بعد تشغيل الخادم، اذهب إلى:

```
http://localhost:5000/swagger
```

## المصادقة (Authentication)

### تسجيل الدخول

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "password123"
}
```

**الاستجابة:**

```json
{
  "userId": 1,
  "username": "admin",
  "email": "admin@smarthr.com",
  "firstName": "Ahmed",
  "lastName": "Al-Dosari",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "roles": ["Admin"],
  "permissions": ["View_Employees", "Create_Employee", ...]
}
```

## الموظفون (Employees)

### الحصول على جميع الموظفين

```http
GET /api/employees?page=1&pageSize=10
Authorization: Bearer {token}
```

### الحصول على موظف واحد

```http
GET /api/employees/{id}
Authorization: Bearer {token}
```

### إضافة موظف

```http
POST /api/employees
Authorization: Bearer {token}
Content-Type: application/json

{
  "employeeCode": "EMP001",
  "firstName": "محمد",
  "lastName": "السلمي",
  "nationalId": "1234567890",
  "email": "mohammed@smarthr.com",
  "phoneNumber": "966501234567",
  "address": "الرياض",
  "departmentId": 1,
  "jobTitleId": 1,
  "branchId": 1,
  "hiringDate": "2024-01-15T00:00:00Z",
  "contractType": "Full-Time",
  "basicSalary": 5000
}
```

### تعديل موظف

```http
PUT /api/employees/{id}
Authorization: Bearer {token}
Content-Type: application/json

{
  "firstName": "محمد",
  "lastName": "السلمي",
  "email": "mohammed.new@smarthr.com",
  "phoneNumber": "966501234567",
  "address": "الرياض",
  "departmentId": 1,
  "jobTitleId": 1,
  "basicSalary": 5500
}
```

### حذف موظف

```http
DELETE /api/employees/{id}
Authorization: Bearer {token}
```

### البحث عن موظفين

```http
GET /api/employees/search/محمد
Authorization: Bearer {token}
```

## الحضور والانصراف (Attendance)

### الحصول على سجلات الحضور

```http
GET /api/attendance?date=2024-06-26&employeeId=1
Authorization: Bearer {token}
```

### تسجيل الحضور

```http
POST /api/attendance/record?employeeId=1&checkIn=2024-06-26T08:30:00&checkOut=2024-06-26T16:30:00
Authorization: Bearer {token}
```

### الحصول على تقرير الحضور

```http
GET /api/attendance/report?startDate=2024-06-01&endDate=2024-06-30
Authorization: Bearer {token}
```

### مزامنة جهاز الحضور

```http
POST /api/attendance/sync-device/{deviceId}
Authorization: Bearer {token}
```

## الرواتب (Salaries)

### حساب الراتب

```http
POST /api/salaries/calculate/{employeeId}?month=6&year=2024
Authorization: Bearer {token}
```

### الحصول على راتب

```http
GET /api/salaries/{salaryId}
Authorization: Bearer {token}
```

### الحصول على رواتب الموظف

```http
GET /api/salaries/employee/{employeeId}
Authorization: Bearer {token}
```

### اعتماد الراتب

```http
POST /api/salaries/approve/{salaryId}
Authorization: Bearer {token}
```

### إنهاء الراتب

```http
POST /api/salaries/finalize/{salaryId}
Authorization: Bearer {token}
```

### تقرير الرواتب

```http
GET /api/salaries/report?startDate=2024-06-01&endDate=2024-06-30&departmentId=1
Authorization: Bearer {token}
```

## التقارير (Reports)

### لوحة التحكم

```http
GET /api/reports/dashboard
Authorization: Bearer {token}
```

### تقرير الحضور

```http
GET /api/reports/attendance?startDate=2024-06-01&endDate=2024-06-30&departmentId=1
Authorization: Bearer {token}
```

### تقرير الأقسام

```http
GET /api/reports/departments
Authorization: Bearer {token}
```

### تصدير إلى PDF

```http
POST /api/reports/export-pdf?reportName=SalaryReport
Authorization: Bearer {token}
Content-Type: application/json

{
  "startDate": "2024-06-01",
  "endDate": "2024-06-30",
  "departmentId": 1
}
```

### تصدير إلى Excel

```http
POST /api/reports/export-excel?reportName=SalaryReport
Authorization: Bearer {token}
Content-Type: application/json

{
  "startDate": "2024-06-01",
  "endDate": "2024-06-30",
  "departmentId": 1
}
```

## معاملات الخطأ (Error Handling)

### استجابة الخطأ العامة

```json
{
  "message": "رسالة الخطأ",
  "errors": [
    {
      "field": "email",
      "message": "البريد الإلكتروني غير صحيح"
    }
  ]
}
```

### رموز الحالة الشائعة

- `200 OK` - نجحت العملية
- `201 Created` - تم الإنشاء بنجاح
- `400 Bad Request` - طلب غير صحيح
- `401 Unauthorized` - غير مصرح
- `403 Forbidden` - محظور
- `404 Not Found` - غير موجود
- `500 Internal Server Error` - خطأ في الخادم

## الصلاحيات والأدوار (Roles & Permissions)

### الأدوار المتاحة

- **Admin**: مسؤول النظام - وصول كامل
- **HR**: الموارد البشرية - إدارة الموظفين والحضور
- **Accountant**: المحاسب - إدارة الرواتب والمحاسبة
- **Manager**: المدير - إشراف على الموظفين
- **Supervisor**: المشرف - تسجيل الحضور والأذونات
- **Employee**: الموظف - عرض البيانات الشخصية

## الملاحظات المهمة

1. جميع التواريخ بصيغة ISO 8601 (YYYY-MM-DDTHH:mm:ss)
2. جميع المبالغ المالية برقم عشري مع منزلتين بعد الفاصلة
3. يجب تضمين رمز المصادقة (Token) في رأس كل طلب
4. الـ API يستخدم RTL للنصوص العربية

## الدعم والمساعدة

للمزيد من المعلومات، يرجى الاتصال بفريق الدعم أو زيارة الموقع الرسمي.
