namespace SmartHRAPI.Core.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<bool> ValidateTokenAsync(string token);
        string GenerateToken(User user, List<string> roles);
    }

    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(int page, int pageSize);
        Task<EmployeeDto> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request);
        Task<EmployeeDto> UpdateEmployeeAsync(int id, CreateEmployeeRequest request);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchTerm);
    }

    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceRecordDto>> GetAttendanceAsync(DateTime date, int? employeeId = null);
        Task<AttendanceRecordDto> RecordAttendanceAsync(int employeeId, DateTime checkIn, DateTime? checkOut);
        Task<IEnumerable<AttendanceReportDto>> GetAttendanceReportAsync(DateTime startDate, DateTime endDate);
        Task SyncDeviceAsync(int deviceId);
    }

    public interface ISalaryService
    {
        Task<SalaryDto> CalculateSalaryAsync(int employeeId, int month, int year);
        Task<SalaryDto> GetSalaryAsync(int salaryId);
        Task<IEnumerable<SalaryDto>> GetEmployeeSalariesAsync(int employeeId);
        Task<IEnumerable<SalaryReportDto>> GenerateSalaryReportAsync(DateTime startDate, DateTime endDate, int? departmentId = null);
        Task<bool> ApproveSalaryAsync(int salaryId);
        Task<bool> FinalizeSalaryAsync(int salaryId);
    }

    public interface IReportService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<IEnumerable<AttendanceReportDto>> GenerateAttendanceReportAsync(DateTime startDate, DateTime endDate, int? departmentId = null);
        Task<IEnumerable<DepartmentReportDto>> GenerateDepartmentReportAsync();
        Task<byte[]> ExportReportToPdfAsync(string reportName, Dictionary<string, object> parameters);
        Task<byte[]> ExportReportToExcelAsync(string reportName, Dictionary<string, object> parameters);
    }

    public interface ILeaveService
    {
        Task<LeaveRequest> RequestLeaveAsync(int employeeId, int leaveTypeId, DateTime startDate, DateTime endDate, string reason);
        Task<bool> ApproveLeaveAsync(int leaveRequestId, int approvedBy);
        Task<bool> RejectLeaveAsync(int leaveRequestId, string reason);
        Task<LeaveBalance> GetLeaveBalanceAsync(int employeeId, int year);
    }

    public interface IUnitOfWork
    {
        IGenericRepository<Employee> EmployeeRepository { get; }
        IGenericRepository<Salary> SalaryRepository { get; }
        IGenericRepository<AttendanceRecord> AttendanceRepository { get; }
        IGenericRepository<LeaveRequest> LeaveRepository { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }

    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync(int page = 1, int pageSize = 10);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    }
}
