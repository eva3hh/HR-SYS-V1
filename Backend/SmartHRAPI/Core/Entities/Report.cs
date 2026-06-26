namespace SmartHRAPI.Core.Entities
{
    public class Report : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty; // Attendance, Salary, Performance, etc.
        public string Description { get; set; } = string.Empty;
        public string? QueryTemplate { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }

        // Navigation Properties
        public ICollection<ReportParameter> Parameters { get; set; } = new List<ReportParameter>();
    }

    public class ReportParameter : BaseEntity
    {
        public int ReportId { get; set; }
        public string ParameterName { get; set; } = string.Empty;
        public string ParameterType { get; set; } = string.Empty; // DateRange, Employee, Department, etc.
        public bool IsRequired { get; set; }
        public string? DefaultValue { get; set; }

        // Navigation Properties
        public Report Report { get; set; } = null!;
    }
}