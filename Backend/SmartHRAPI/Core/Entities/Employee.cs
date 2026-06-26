namespace SmartHRAPI.Core.Entities
{
    public class Employee : BaseEntity
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? ProfilePicturePath { get; set; }
        
        // Organization Info
        public int DepartmentId { get; set; }
        public int JobTitleId { get; set; }
        public int? DirectManagerId { get; set; }
        public int BranchId { get; set; }
        
        // Employment Details
        public DateTime HiringDate { get; set; }
        public string ContractType { get; set; } = "Full-Time"; // Full-Time, Part-Time, Contract
        public string EmploymentStatus { get; set; } = "Active"; // Active, On Leave, Suspended, Resigned
        public DateTime? ResignationDate { get; set; }
        
        // Salary Info
        public decimal BasicSalary { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? FileNumber { get; set; }
        
        // Navigation Properties
        public Department Department { get; set; } = null!;
        public JobTitle JobTitle { get; set; } = null!;
        public Employee? DirectManager { get; set; }
        public ICollection<Employee> Subordinates { get; set; } = new List<Employee>();
        public ICollection<EmployeeDocument> Documents { get; set; } = new List<EmployeeDocument>();
        public ICollection<EmployeeSalaryDetails> SalaryDetails { get; set; } = new List<EmployeeSalaryDetails>();
        public ICollection<EmployeeBankAccount> BankAccounts { get; set; } = new List<EmployeeBankAccount>();
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public ICollection<LeaveBalance> LeaveBalances { get; set; } = new List<LeaveBalance>();
        public ICollection<PermissionRequest> PermissionRequests { get; set; } = new List<PermissionRequest>();
        public ICollection<Salary> Salaries { get; set; } = new List<Salary>();
        public ICollection<EmployeeSchedule> Schedules { get; set; } = new List<EmployeeSchedule>();
    }

    public class EmployeeDocument : BaseEntity
    {
        public int EmployeeId { get; set; }
        public string DocumentName { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty; // Contract, ID, Certificate, etc.
        public string FilePath { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime? ExpiryDate { get; set; }

        // Navigation Properties
        public Employee Employee { get; set; } = null!;
    }

    public class EmployeeSalaryDetails : BaseEntity
    {
        public int EmployeeId { get; set; }
        public decimal HousingAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal MealAllowance { get; set; }
        public decimal NatureOfWorkAllowance { get; set; }
        public decimal OtherAllowances { get; set; }
        public decimal InsuranceDeduction { get; set; }
        public decimal TaxDeduction { get; set; }
        public decimal OtherDeductions { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Navigation Properties
        public Employee Employee { get; set; } = null!;
    }

    public class EmployeeBankAccount : BaseEntity
    {
        public int EmployeeId { get; set; }
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string IBAN { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public string AccountHolderName { get; set; } = string.Empty;

        // Navigation Properties
        public Employee Employee { get; set; } = null!;
    }
}