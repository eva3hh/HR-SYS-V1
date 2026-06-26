namespace SmartHRAPI.Core.Entities
{
    public class LeaveType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal AnnualEntitlement { get; set; } // عدد أيام الإجازة السنوية
        public bool IsPaid { get; set; }
        public bool RequiresApproval { get; set; }
        public int MaxDaysPerRequest { get; set; }
        public bool IsCarryForward { get; set; } // هل يمكن نقل الأيام غير المستخدمة
        public decimal MaxCarryForwardDays { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
    }

    public class LeaveRequest : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DaysRequested { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Cancelled
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalNotes { get; set; }
        public string? AttachmentPath { get; set; }

        // Navigation Properties
        public Employee Employee { get; set; } = null!;
        public LeaveType LeaveType { get; set; } = null!;
    }

    public class LeaveBalance : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public int Year { get; set; }
        public decimal TotalEntitlement { get; set; }
        public decimal UsedDays { get; set; }
        public decimal RemainingDays { get; set; }
        public decimal CarriedForwardDays { get; set; }
        public DateTime LastUpdated { get; set; }

        // Navigation Properties
        public Employee Employee { get; set; } = null!;
    }

    public class PermissionType : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MaxDaysPerYear { get; set; }
        public bool IsPaid { get; set; }
        public bool RequiresApproval { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PermissionRequest : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int PermissionTypeId { get; set; }
        public DateTime RequestDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        public Employee Employee { get; set; } = null!;
    }
}