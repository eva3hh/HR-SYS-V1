using Microsoft.EntityFrameworkCore;
using SmartHRAPI.Core.Entities;
using SmartHRAPI.Core.Interfaces;
using SmartHRAPI.Infrastructure.Data;

namespace SmartHRAPI.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var today = DateTime.UtcNow.Date;
            var totalEmployees = await _context.Employees.CountAsync(e => !e.IsDeleted && e.EmploymentStatus == "Active");

            var todayAttendance = await _context.AttendanceRecords
                .Where(a => a.CheckInTime.Date == today)
                .ToListAsync();

            var presentToday = todayAttendance.Count(a => a.Status == "Present" || a.Status == "Late");
            var absentToday = totalEmployees - presentToday;
            var lateToday = todayAttendance.Count(a => a.Status == "Late");
            var totalOvertimeHours = todayAttendance.Sum(a => a.OvertimeHours);

            var onLeaveToday = await _context.LeaveRequests
                .CountAsync(l => l.Status == "Approved" &&
                           l.StartDate.Date <= today &&
                           l.EndDate.Date >= today);

            var currentMonth = today.Month;
            var currentYear = today.Year;
            var totalPayableThisMonth = await _context.Salaries
                .Where(s => s.Month == currentMonth && s.Year == currentYear)
                .SumAsync(s => s.NetAmount);

            var topLateComers = await _context.AttendanceRecords
                .Include(a => a.Employee)
                .Where(a => a.Status == "Late")
                .GroupBy(a => a.EmployeeId)
                .Select(g => new TopLateComersDto
                {
                    EmployeeId = g.Key,
                    EmployeeName = $"{g.First().Employee.FirstName} {g.First().Employee.LastName}",
                    TotalLateDays = g.Count(),
                    AverageLateMinutes = (int)g.Average(a => a.LateMinutes)
                })
                .OrderByDescending(t => t.TotalLateDays)
                .Take(10)
                .ToListAsync();

            return new DashboardSummaryDto
            {
                TotalEmployees = totalEmployees,
                PresentToday = presentToday,
                AbsentToday = absentToday,
                LateToday = lateToday,
                TotalOvertimeHours = totalOvertimeHours,
                TotalPayableThisMonth = totalPayableThisMonth,
                OnLeaveToday = onLeaveToday,
                TopLateComers = topLateComers
            };
        }

        public async Task<IEnumerable<AttendanceReportDto>> GenerateAttendanceReportAsync(DateTime startDate, DateTime endDate, int? departmentId = null)
        {
            var query = _context.AttendanceRecords
                .Include(a => a.Employee)
                .AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(a => a.Employee.DepartmentId == departmentId);

            var records = await query
                .Where(a => a.CheckInTime.Date >= startDate.Date && a.CheckInTime.Date <= endDate.Date)
                .GroupBy(a => a.CheckInTime.Date)
                .Select(g => new AttendanceReportDto
                {
                    Date = g.Key,
                    PresentCount = g.Count(a => a.Status == "Present" || a.Status == "Late"),
                    AbsentCount = g.Count(a => a.Status == "Absent"),
                    LateCount = g.Count(a => a.Status == "Late"),
                    AverageLateMinutes = g.Where(a => a.Status == "Late").Average(a => a.LateMinutes)
                })
                .ToListAsync();

            return records;
        }

        public async Task<IEnumerable<DepartmentReportDto>> GenerateDepartmentReportAsync()
        {
            var departments = await _context.Departments
                .Include(d => d.Employees)
                .Where(d => !d.IsDeleted && d.IsActive)
                .ToListAsync();

            var reports = new List<DepartmentReportDto>();

            foreach (var dept in departments)
            {
                var totalEmployees = dept.Employees.Count(e => !e.IsDeleted);
                var averageSalary = dept.Employees.Where(e => !e.IsDeleted).Average(e => e.BasicSalary);

                var today = DateTime.UtcNow.Date;
                var presentToday = await _context.AttendanceRecords
                    .CountAsync(a => a.Employee.DepartmentId == dept.Id &&
                           a.CheckInTime.Date == today &&
                           (a.Status == "Present" || a.Status == "Late"));

                var attendanceRate = totalEmployees > 0 ? (presentToday * 100) / totalEmployees : 0;

                var averageOvertimeHours = await _context.AttendanceRecords
                    .Where(a => a.Employee.DepartmentId == dept.Id)
                    .AverageAsync(a => a.OvertimeHours);

                reports.Add(new DepartmentReportDto
                {
                    DepartmentName = dept.Name,
                    TotalEmployees = totalEmployees,
                    AverageSalary = averageSalary,
                    AttendanceRate = attendanceRate,
                    AverageOvertimeHours = averageOvertimeHours
                });
            }

            return reports;
        }

        public async Task<byte[]> ExportReportToPdfAsync(string reportName, Dictionary<string, object> parameters)
        {
            // Placeholder for PDF export logic
            // In real implementation, would use a library like iTextSharp or SelectPdf
            await Task.Delay(100); // Simulate processing
            return new byte[] { };
        }

        public async Task<byte[]> ExportReportToExcelAsync(string reportName, Dictionary<string, object> parameters)
        {
            // Placeholder for Excel export logic
            // In real implementation, would use a library like EPPlus
            await Task.Delay(100); // Simulate processing
            return new byte[] { };
        }
    }
}