using Microsoft.EntityFrameworkCore;
using SmartHRAPI.Core.Entities;
using SmartHRAPI.Core.Interfaces;
using SmartHRAPI.Infrastructure.Data;

namespace SmartHRAPI.Application.Services
{
    public class SalaryService : ISalaryService
    {
        private readonly ApplicationDbContext _context;

        public SalaryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SalaryDto> CalculateSalaryAsync(int employeeId, int month, int year)
        {
            var employee = await _context.Employees
                .Include(e => e.SalaryDetails)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null)
                throw new KeyNotFoundException("الموظف غير موجود");

            // Get attendance records for the month
            var attendanceRecords = await _context.AttendanceRecords
                .Where(a => a.EmployeeId == employeeId &&
                       a.CheckInTime.Year == year &&
                       a.CheckInTime.Month == month)
                .ToListAsync();

            // Get leave records for the month
            var leaveRequests = await _context.LeaveRequests
                .Where(l => l.EmployeeId == employeeId &&
                       l.Status == "Approved" &&
                       l.StartDate.Year == year &&
                       l.StartDate.Month == month)
                .ToListAsync();

            // Calculate components
            var basicSalary = employee.BasicSalary;
            var salaryDetails = employee.SalaryDetails.OrderByDescending(s => s.EffectiveDate).FirstOrDefault();

            decimal housingAllowance = salaryDetails?.HousingAllowance ?? 0;
            decimal transportAllowance = salaryDetails?.TransportAllowance ?? 0;
            decimal mealAllowance = salaryDetails?.MealAllowance ?? 0;
            decimal natureOfWorkAllowance = salaryDetails?.NatureOfWorkAllowance ?? 0;
            decimal otherAllowances = salaryDetails?.OtherAllowances ?? 0;

            var totalAllowances = housingAllowance + transportAllowance + mealAllowance + natureOfWorkAllowance + otherAllowances;

            // Calculate deductions based on late and absence
            var lateDeduction = CalculateLateDeduction(attendanceRecords, basicSalary);
            var absenceDeduction = CalculateAbsenceDeduction(leaveRequests, basicSalary);

            // Get bonuses and advances for the month
            var bonuses = await _context.Bonuses
                .Where(b => b.EmployeeId == employeeId &&
                       b.EffectiveDate.Year == year &&
                       b.EffectiveDate.Month == month &&
                       b.Status == "Approved")
                .ToListAsync();

            var totalBonuses = bonuses.Sum(b => b.Amount);

            var advances = await _context.Advances
                .Where(a => a.EmployeeId == employeeId &&
                       a.ApprovalDate.Year == year &&
                       a.ApprovalDate.Month == month)
                .ToListAsync();

            decimal advanceAmount = 0;
            if (advances.Any())
            {
                var advance = advances.First();
                advanceAmount = advance.Amount / advance.NumberOfInstallments;
            }

            var insuranceDeduction = salaryDetails?.InsuranceDeduction ?? 0;
            var taxDeduction = salaryDetails?.TaxDeduction ?? 0;

            var totalDeductions = lateDeduction + absenceDeduction + advanceAmount + insuranceDeduction + taxDeduction;

            var grossAmount = basicSalary + totalAllowances + totalBonuses;
            var netAmount = grossAmount - totalDeductions;

            var salary = new Salary
            {
                EmployeeId = employeeId,
                Month = month,
                Year = year,
                BasicSalary = basicSalary,
                HousingAllowance = housingAllowance,
                TransportAllowance = transportAllowance,
                MealAllowance = mealAllowance,
                NatureOfWorkAllowance = natureOfWorkAllowance,
                OtherAllowances = otherAllowances,
                TotalAllowances = totalAllowances,
                BonusAmount = totalBonuses,
                TotalBonuses = totalBonuses,
                LateDeduction = lateDeduction,
                AbsenceDeduction = absenceDeduction,
                AdvanceAmount = advanceAmount,
                InsuranceDeduction = insuranceDeduction,
                TaxDeduction = taxDeduction,
                TotalDeductions = totalDeductions,
                GrossAmount = grossAmount,
                NetAmount = netAmount,
                Status = "Calculated",
                CalculationDate = DateTime.UtcNow
            };

            _context.Salaries.Add(salary);
            await _context.SaveChangesAsync();

            return MapToDto(salary);
        }

        public async Task<SalaryDto> GetSalaryAsync(int salaryId)
        {
            var salary = await _context.Salaries
                .Include(s => s.Employee)
                .FirstOrDefaultAsync(s => s.Id == salaryId);

            if (salary == null)
                throw new KeyNotFoundException("الراتب غير موجود");

            return MapToDto(salary);
        }

        public async Task<IEnumerable<SalaryDto>> GetEmployeeSalariesAsync(int employeeId)
        {
            var salaries = await _context.Salaries
                .Include(s => s.Employee)
                .Where(s => s.EmployeeId == employeeId)
                .OrderByDescending(s => s.Year)
                .ThenByDescending(s => s.Month)
                .ToListAsync();

            return salaries.Select(s => MapToDto(s));
        }

        public async Task<IEnumerable<SalaryReportDto>> GenerateSalaryReportAsync(DateTime startDate, DateTime endDate, int? departmentId = null)
        {
            var query = _context.Salaries
                .Include(s => s.Employee)
                .ThenInclude(e => e.Department)
                .AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(s => s.Employee.DepartmentId == departmentId);

            var salaries = await query
                .Where(s => s.CalculationDate >= startDate && s.CalculationDate <= endDate)
                .ToListAsync();

            return salaries.Select(s => new SalaryReportDto
            {
                EmployeeId = s.EmployeeId,
                EmployeeName = $"{s.Employee.FirstName} {s.Employee.LastName}",
                Department = s.Employee.Department?.Name ?? "N/A",
                StartDate = startDate,
                EndDate = endDate,
                TotalSalary = s.GrossAmount,
                TotalDeductions = s.TotalDeductions,
                NetPay = s.NetAmount
            });
        }

        public async Task<bool> ApproveSalaryAsync(int salaryId)
        {
            var salary = await _context.Salaries.FindAsync(salaryId);
            if (salary == null)
                return false;

            salary.Status = "Approved";
            salary.ApprovalDate = DateTime.UtcNow;
            _context.Salaries.Update(salary);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> FinalizeSalaryAsync(int salaryId)
        {
            var salary = await _context.Salaries.FindAsync(salaryId);
            if (salary == null)
                return false;

            salary.Status = "Finalized";
            salary.PaymentDate = DateTime.UtcNow;
            _context.Salaries.Update(salary);
            await _context.SaveChangesAsync();

            return true;
        }

        private decimal CalculateLateDeduction(List<AttendanceRecord> records, decimal basicSalary)
        {
            var totalLateMinutes = records.Sum(r => r.LateMinutes);
            var dailyRate = basicSalary / 30;
            var hourlyRate = dailyRate / 8;
            var minuteRate = hourlyRate / 60;
            return totalLateMinutes * minuteRate;
        }

        private decimal CalculateAbsenceDeduction(List<LeaveRequest> leaves, decimal basicSalary)
        {
            var unpaidLeaveDays = leaves.Where(l => !l.LeaveType.IsPaid).Sum(l => l.DaysRequested);
            var dailyRate = basicSalary / 30;
            return unpaidLeaveDays * dailyRate;
        }

        private SalaryDto MapToDto(Salary salary)
        {
            return new SalaryDto
            {
                Id = salary.Id,
                EmployeeId = salary.EmployeeId,
                EmployeeName = $"{salary.Employee.FirstName} {salary.Employee.LastName}",
                Month = salary.Month,
                Year = salary.Year,
                BasicSalary = salary.BasicSalary,
                TotalAllowances = salary.TotalAllowances,
                TotalBonuses = salary.TotalBonuses,
                TotalDeductions = salary.TotalDeductions,
                GrossAmount = salary.GrossAmount,
                NetAmount = salary.NetAmount,
                Status = salary.Status
            };
        }
    }
}