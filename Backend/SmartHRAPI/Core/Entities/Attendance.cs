namespace SmartHRAPI.Core.Entities
{
    public class AttendanceDevice : BaseEntity
    {
        public string DeviceName { get; set; } = string.Empty;
        public string DeviceType { get; set; } = string.Empty; // ZKTeco, Suprema, HikVision, Anviz, FingerTec
        public string DeviceModel { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public int Port { get; set; } = 4370;
        public string ConnectionType { get; set; } = "TCP/IP"; // TCP/IP, USB
        public int BranchId { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime? LastSyncTime { get; set; }
        public string? AdminPassword { get; set; }

        // Navigation Properties
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
    }

    public class AttendanceRecord : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int DeviceId { get; set; }
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string Status { get; set; } = "Present"; // Present, Absent, Late, Early, Excused
        public int LateMinutes { get; set; }
        public int EarlyMinutes { get; set; }
        public decimal OvertimeHours { get; set; }
        public string? Notes { get; set; }
        public bool IsApproved { get; set; }

        // Navigation Properties
        public Employee Employee { get; set; } = null!;
        public AttendanceDevice Device { get; set; } = null!;
    }

    public class AttendanceRawRecord : BaseEntity
    {
        public int DeviceId { get; set; }
        public string EnrollNumber { get; set; } = string.Empty;
        public DateTime RecordTime { get; set; }
        public int VerifyMethod { get; set; } = 0; // 0=Face, 1=Fingerprint, 2=PIN, 3=Card, etc.
        public string RawData { get; set; } = string.Empty;
        public bool IsProcessed { get; set; }
        public int? MatchedEmployeeId { get; set; }
    }
}