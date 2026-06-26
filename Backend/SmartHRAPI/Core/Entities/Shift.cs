namespace SmartHRAPI.Core.Entities
{
    public class Shift : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan CheckInTolerance { get; set; } // سماحية الدخول
        public TimeSpan CheckOutTolerance { get; set; } // سماحية الانصراف
        public bool IsNightShift { get; set; }
        public decimal MinimumHoursPerDay { get; set; }
        public decimal OvertimeStartHour { get; set; }
        public int WeeklyRestDay { get; set; } // 0=Sunday, 1=Monday, etc.
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<ShiftDetail> ShiftDetails { get; set; } = new List<ShiftDetail>();
    }

    public class ShiftDetail : BaseEntity
    {
        public int ShiftId { get; set; }
        public int DayOfWeek { get; set; } // 0=Sunday, 1=Monday, etc.
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsWorkingDay { get; set; }

        // Navigation Properties
        public Shift Shift { get; set; } = null!;
    }

    public class EmployeeSchedule : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int ShiftId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int RotationType { get; set; } = 0; // 0=Fixed, 1=Rotating
        public int RotationCycleDays { get; set; }

        // Navigation Properties
        public Employee Employee { get; set; } = null!;
    }

    public class DailySchedule : BaseEntity
    {
        public int EmployeeId { get; set; }
        public DateTime ScheduleDate { get; set; }
        public TimeSpan PlannedStartTime { get; set; }
        public TimeSpan PlannedEndTime { get; set; }
        public string ScheduleType { get; set; } = "Regular"; // Regular, Leave, Permission, Holiday
        public string? Notes { get; set; }
    }
}