using System.Text.Json.Serialization;

namespace SmartHRAPI.Core.DTOs
{
    // Auth DTOs
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public List<string> Permissions { get; set; } = new();
    }

    // Employee DTOs
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string JobTitleName { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public string EmploymentStatus { get; set; } = string.Empty;
        public DateTime HiringDate { get; set; }
        public string? ProfilePicturePath { get; set; }
    }

    public class CreateEmployeeRequest
    {
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public int JobTitleId { get; set; }
        public int BranchId { get; set; }
        public DateTime HiringDate { get; set; }
        public string ContractType { get; set; } = "Full-Time";
        public decimal BasicSalary { get; set; }
    }

    // Attendance DTOs
    public class AttendanceRecordDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public int LateMinutes { get; set; }
        public decimal OvertimeHours { get; set; }
    }

    // Salary DTOs
    public class SalaryDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalBonuses { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class SalaryReportDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }
    }

    // Dashboard DTOs
    public class DashboardSummaryDto
    {
        public int TotalEmployees { get; set; }
        public int PresentToday { get; set; }
        public int AbsentToday { get; set; }
        public int LateToday { get; set; }
        public decimal TotalOvertimeHours { get; set; }
        public decimal TotalPayableThisMonth { get; set; }
        public int OnLeaveToday { get; set; }
        public List<TopLateComersDto> TopLateComers { get; set; } = new();
    }

    public class TopLateComersDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int TotalLateDays { get; set; }
        public int AverageLateMinutes { get; set; }
    }

    // Report DTOs
    public class AttendanceReportDto
    {
        public DateTime Date { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public decimal AverageLateMinutes { get; set; }
    }

    public class DepartmentReportDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public int TotalEmployees { get; set; }
        public decimal AverageSalary { get; set; }
        public int AttendanceRate { get; set; }
        public decimal AverageOvertimeHours { get; set; }
    }
}
