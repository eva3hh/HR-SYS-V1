using Microsoft.EntityFrameworkCore;
using SmartHRAPI.Core.Entities;
using SmartHRAPI.Core.Interfaces;
using SmartHRAPI.Infrastructure.Data;

namespace SmartHRAPI.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeService(ApplicationDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesAsync(int page, int pageSize)
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Where(e => !e.IsDeleted)
                .OrderBy(e => e.FirstName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return employees.Select(e => MapToDto(e));
        }

        public async Task<EmployeeDto> GetEmployeeByIdAsync(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Include(e => e.DirectManager)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (employee == null)
                throw new KeyNotFoundException($"الموظف برقم {id} غير موجود");

            return MapToDto(employee);
        }

        public async Task<EmployeeDto> CreateEmployeeAsync(CreateEmployeeRequest request)
        {
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeCode == request.EmployeeCode);

            if (existingEmployee != null)
                throw new InvalidOperationException($"الرقم الوظيفي {request.EmployeeCode} موجود بالفعل");

            var employee = new Employee
            {
                EmployeeCode = request.EmployeeCode,
                FirstName = request.FirstName,
                LastName = request.LastName,
                NationalId = request.NationalId,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                DepartmentId = request.DepartmentId,
                JobTitleId = request.JobTitleId,
                BranchId = request.BranchId,
                HiringDate = request.HiringDate,
                ContractType = request.ContractType,
                BasicSalary = request.BasicSalary,
                EmploymentStatus = "Active"
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return MapToDto(employee);
        }

        public async Task<EmployeeDto> UpdateEmployeeAsync(int id, CreateEmployeeRequest request)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                throw new KeyNotFoundException($"الموظف برقم {id} غير موجود");

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.PhoneNumber = request.PhoneNumber;
            employee.Address = request.Address;
            employee.DepartmentId = request.DepartmentId;
            employee.JobTitleId = request.JobTitleId;
            employee.BasicSalary = request.BasicSalary;
            employee.UpdatedAt = DateTime.UtcNow;

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return MapToDto(employee);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return false;

            employee.IsDeleted = true;
            employee.DeletedAt = DateTime.UtcNow;
            employee.EmploymentStatus = "Resigned";

            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<EmployeeDto>> SearchEmployeesAsync(string searchTerm)
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.JobTitle)
                .Where(e => !e.IsDeleted && (
                    e.FirstName.Contains(searchTerm) ||
                    e.LastName.Contains(searchTerm) ||
                    e.EmployeeCode.Contains(searchTerm) ||
                    e.Email.Contains(searchTerm)
                ))
                .ToListAsync();

            return employees.Select(e => MapToDto(e));
        }

        private EmployeeDto MapToDto(Employee employee)
        {
            return new EmployeeDto
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                DepartmentName = employee.Department?.Name ?? string.Empty,
                JobTitleName = employee.JobTitle?.Name ?? string.Empty,
                BasicSalary = employee.BasicSalary,
                EmploymentStatus = employee.EmploymentStatus,
                HiringDate = employee.HiringDate,
                ProfilePicturePath = employee.ProfilePicturePath
            };
        }
    }
}