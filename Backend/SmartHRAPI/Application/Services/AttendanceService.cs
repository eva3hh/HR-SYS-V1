using Microsoft.EntityFrameworkCore;
using SmartHRAPI.Core.Entities;
using SmartHRAPI.Core.Interfaces;
using SmartHRAPI.Infrastructure.Data;

namespace SmartHRAPI.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendanceRecordDto>> GetAttendanceAsync(DateTime date, int? employeeId = null)
        {
            var query = _context.AttendanceRecords
                .Include(a => a.Employee)
                .Where(a => a.CheckInTime.Date == date.Date)
                .AsQueryable();

            if (employeeId.HasValue)
                query = query.Where(a => a.EmployeeId == employeeId);

            var records = await query.ToListAsync();

            return records.Select(r => new AttendanceRecordDto
            {
                Id = r.Id,
                EmployeeName = $"{r.Employee.FirstName} {r.Employee.LastName}",
                CheckInTime = r.CheckInTime,
                CheckOutTime = r.CheckOutTime,
                Status = r.Status,
                LateMinutes = r.LateMinutes,
                OvertimeHours = r.OvertimeHours
            });
        }

        public async Task<AttendanceRecordDto> RecordAttendanceAsync(int employeeId, DateTime checkIn, DateTime? checkOut)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
                throw new KeyNotFoundException("الموظف غير موجود");

            var existingRecord = await _context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.CheckInTime.Date == checkIn.Date);

            var status = DetermineAttendanceStatus(checkIn, employee);
            var lateMinutes = status == "Late" ? CalculateLateMinutes(checkIn, employee) : 0;
            var overtimeHours = checkOut.HasValue ? CalculateOvertime(checkIn, checkOut.Value, employee) : 0;

            if (existingRecord != null)
            {
                existingRecord.CheckOutTime = checkOut;
                existingRecord.OvertimeHours = overtimeHours;
                existingRecord.Status = status;
                _context.AttendanceRecords.Update(existingRecord);
            }
            else
            {
                var record = new AttendanceRecord
                {
                    EmployeeId = employeeId,
                    CheckInTime = checkIn,
                    CheckOutTime = checkOut,
                    Status = status,
                    LateMinutes = lateMinutes,
                    OvertimeHours = overtimeHours
                };
                _context.AttendanceRecords.Add(record);
            }

            await _context.SaveChangesAsync();

            var updatedRecord = await _context.AttendanceRecords
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.CheckInTime.Date == checkIn.Date);

            return new AttendanceRecordDto
            {
                Id = updatedRecord!.Id,
                EmployeeName = $"{employee.FirstName} {employee.LastName}",
                CheckInTime = updatedRecord.CheckInTime,
                CheckOutTime = updatedRecord.CheckOutTime,
                Status = updatedRecord.Status,
                LateMinutes = updatedRecord.LateMinutes,
                OvertimeHours = updatedRecord.OvertimeHours
            };
        }

        public async Task<IEnumerable<AttendanceReportDto>> GetAttendanceReportAsync(DateTime startDate, DateTime endDate)
        {
            var records = await _context.AttendanceRecords
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

        public async Task SyncDeviceAsync(int deviceId)
        {
            var device = await _context.AttendanceDevices.FindAsync(deviceId);
            if (device == null)
                throw new KeyNotFoundException("الجهاز غير موجود");

            // This is a placeholder for device sync logic
            // In real implementation, this would connect to the device and sync records
            device.LastSyncTime = DateTime.UtcNow;
            _context.AttendanceDevices.Update(device);
            await _context.SaveChangesAsync();
        }

        private string DetermineAttendanceStatus(DateTime checkIn, Employee employee)
        {
            // Get the shift for the employee on this date
            var schedule = _context.EmployeeSchedules
                .FirstOrDefault(es => es.EmployeeId == employee.Id);

            if (schedule == null)
                return "Present";

            // Simple logic - in real implementation, would be more complex
            return DateTime.UtcNow > checkIn.AddMinutes(15) ? "Late" : "Present";
        }

        private int CalculateLateMinutes(DateTime checkIn, Employee employee)
        {
            // Placeholder - would calculate based on shift
            return 0;
        }

        private decimal CalculateOvertime(DateTime checkIn, DateTime checkOut, Employee employee)
        {
            var duration = checkOut - checkIn;
            var hours = duration.TotalHours;
            return hours > 8 ? (decimal)(hours - 8) : 0;
        }
    }
}