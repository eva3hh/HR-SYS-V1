# Smart HR ERP - Architecture Documentation

## البنية المعمارية

تم بناء النظام باستخدام معمارية نظيفة (Clean Architecture) مع اتباع مبادئ SOLID.

## طبقات النظام

### 1. Presentation Layer (طبقة العرض)
- **Controllers**: التحكم في طلبات HTTP
- **Attributes**: صفات التحقق والتفويض
- **Middleware**: معالجة الأخطاء والمصادقة

### 2. Application Layer (طبقة التطبيق)
- **Services**: منطق العمل الأساسي
- **DTOs**: نقل البيانات بين الطبقات
- **Validators**: التحقق من صحة البيانات
- **Mappers**: تحويل البيانات

### 3. Core/Domain Layer (طبقة المنطق)
- **Entities**: كائنات المجال
- **Interfaces**: العقود (الواجهات)
- **Enums**: التعديلات

### 4. Infrastructure Layer (طبقة البنية التحتية)
- **Data Context**: DbContext
- **Repositories**: الوصول إلى البيانات
- **UnitOfWork**: إدارة العمليات
- **External Services**: الخدمات الخارجية

## قاعدة البيانات

### المخطط (Schema)

```
Users
├── User (المستخدم)
├── Role (الدور)
├── UserRole (ربط المستخدم بالدور)
├── Permission (الصلاحية)
└── RolePermission (ربط الدور بالصلاحية)

Organization
├── Company (الشركة)
├── Branch (الفرع)
├── Department (القسم)
├── JobTitle (المسمى الوظيفي)
└── Division (الفئة)

Employees
├── Employee (الموظف)
├── EmployeeDocument (مستندات الموظف)
├── EmployeeSalaryDetails (تفاصيل راتب الموظف)
└── EmployeeBankAccount (حساب بنكي للموظف)

Attendance
├── AttendanceDevice (جهاز الحضور)
├── AttendanceRecord (سجل الحضور)
└── AttendanceRawRecord (سجل خام)

Shifts
├── Shift (الشفت)
├── ShiftDetail (تفاصيل الشفت)
├── EmployeeSchedule (جدول الموظف)
└── DailySchedule (الجدول اليومي)

Leaves
├── LeaveType (نوع الإجازة)
├── LeaveRequest (طلب الإجازة)
├── LeaveBalance (رصيد الإجازة)
├── PermissionType (نوع الإذن)
└── PermissionRequest (طلب الإذن)

Salary
├── Salary (الراتب)
├── SalaryComponent (مكون الراتب)
├── Allowance (البدل)
├── Deduction (الخصم)
├── Bonus (الحافز)
├── Advance (السلفة)
└── Penalty (الجزاء)

Reports
├── Report (التقرير)
└── ReportParameter (معاملات التقرير)

Audit
└── AuditLog (سجل التدقيق)
```

## نمط Repository

النظام يستخدم نمط Repository لفصل منطق الوصول إلى البيانات:

```csharp
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync(int page = 1, int pageSize = 10);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}
```

## نمط Unit of Work

يتم إدارة معاملات قاعدة البيانات من خلال Unit of Work:

```csharp
public interface IUnitOfWork
{
    IGenericRepository<Employee> EmployeeRepository { get; }
    IGenericRepository<Salary> SalaryRepository { get; }
    // ... more repositories
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
```

## حقن التبعيات (Dependency Injection)

```csharp
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ISalaryService, SalaryService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
```

## المصادقة والتفويض (Authentication & Authorization)

### JWT Token

يستخدم النظام JWT للمصادقة:

```csharp
[Authorize]
public class EmployeesController : ControllerBase
{
    [Authorize(Roles = "Admin,HR")]
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> CreateEmployee(CreateEmployeeRequest request)
    {
        // ...
    }
}
```

## المراحل التالية للتحسين

1. **Caching**: إضافة Redis للذاكرة المؤقتة
2. **Background Jobs**: استخدام Hangfire للمهام الخلفية
3. **Notifications**: نظام الإشعارات الفوري (SignalR)
4. **File Upload**: نظام رفع الملفات المتقدم
5. **API Versioning**: إدارة إصدارات API
6. **Rate Limiting**: تحديد معدل الطلبات
7. **API Documentation**: توثيق شامل مع Swagger
