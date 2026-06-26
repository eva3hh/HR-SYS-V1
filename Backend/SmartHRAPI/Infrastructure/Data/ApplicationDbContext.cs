using Microsoft.EntityFrameworkCore;
using SmartHRAPI.Core.Entities;

namespace SmartHRAPI.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Users & Security
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }

        // Organization
        public DbSet<Company> Companies { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<Division> Divisions { get; set; }

        // Employees
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeDocument> EmployeeDocuments { get; set; }
        public DbSet<EmployeeSalaryDetails> EmployeeSalaryDetails { get; set; }
        public DbSet<EmployeeBankAccount> EmployeeBankAccounts { get; set; }

        // Attendance
        public DbSet<AttendanceDevice> AttendanceDevices { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<AttendanceRawRecord> AttendanceRawRecords { get; set; }

        // Shifts & Schedule
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<ShiftDetail> ShiftDetails { get; set; }
        public DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }
        public DbSet<DailySchedule> DailySchedules { get; set; }

        // Leave Management
        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<LeaveBalance> LeaveBalances { get; set; }

        // Permissions (Daily)
        public DbSet<PermissionRequest> PermissionRequests { get; set; }
        public DbSet<PermissionType> PermissionTypes { get; set; }

        // Salary Management
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<SalaryComponent> SalaryComponents { get; set; }
        public DbSet<Allowance> Allowances { get; set; }
        public DbSet<Deduction> Deductions { get; set; }
        public DbSet<Bonus> Bonuses { get; set; }
        public DbSet<Advance> Advances { get; set; }
        public DbSet<Penalty> Penalties { get; set; }

        // Reports
        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportParameter> ReportParameters { get; set; }

        // Audit Logs
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints
            ConfigureUserEntities(modelBuilder);
            ConfigureOrganizationEntities(modelBuilder);
            ConfigureEmployeeEntities(modelBuilder);
            ConfigureAttendanceEntities(modelBuilder);
            ConfigureShiftEntities(modelBuilder);
            ConfigureLeaveEntities(modelBuilder);
            ConfigureSalaryEntities(modelBuilder);
        }

        private void ConfigureUserEntities(ModelBuilder modelBuilder)
        {
            // User Roles Relationship
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Role Permissions Relationship
            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureOrganizationEntities(ModelBuilder modelBuilder)
        {
            // Company Branches
            modelBuilder.Entity<Branch>()
                .HasOne(b => b.Company)
                .WithMany(c => c.Branches)
                .HasForeignKey(b => b.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Department relationships
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Branch)
                .WithMany(b => b.Departments)
                .HasForeignKey(d => d.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
                .HasOne(d => d.ParentDepartment)
                .WithMany(d => d.SubDepartments)
                .HasForeignKey(d => d.ParentDepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureEmployeeEntities(ModelBuilder modelBuilder)
        {
            // Employee relationships
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.JobTitle)
                .WithMany(j => j.Employees)
                .HasForeignKey(e => e.JobTitleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.DirectManager)
                .WithMany(e => e.Subordinates)
                .HasForeignKey(e => e.DirectManagerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Documents
            modelBuilder.Entity<EmployeeDocument>()
                .HasOne(ed => ed.Employee)
                .WithMany(e => e.Documents)
                .HasForeignKey(ed => ed.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureAttendanceEntities(ModelBuilder modelBuilder)
        {
            // Attendance Records
            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(ar => ar.Employee)
                .WithMany(e => e.AttendanceRecords)
                .HasForeignKey(ar => ar.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AttendanceRecord>()
                .HasOne(ar => ar.Device)
                .WithMany(d => d.AttendanceRecords)
                .HasForeignKey(ar => ar.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ConfigureShiftEntities(ModelBuilder modelBuilder)
        {
            // Shift Details
            modelBuilder.Entity<ShiftDetail>()
                .HasOne(sd => sd.Shift)
                .WithMany(s => s.ShiftDetails)
                .HasForeignKey(sd => sd.ShiftId)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee Schedules
            modelBuilder.Entity<EmployeeSchedule>()
                .HasOne(es => es.Employee)
                .WithMany(e => e.Schedules)
                .HasForeignKey(es => es.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureLeaveEntities(ModelBuilder modelBuilder)
        {
            // Leave Requests
            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.Employee)
                .WithMany(e => e.LeaveRequests)
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LeaveRequest>()
                .HasOne(lr => lr.LeaveType)
                .WithMany(lt => lt.LeaveRequests)
                .HasForeignKey(lr => lr.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Leave Balances
            modelBuilder.Entity<LeaveBalance>()
                .HasOne(lb => lb.Employee)
                .WithMany(e => e.LeaveBalances)
                .HasForeignKey(lb => lb.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        private void ConfigureSalaryEntities(ModelBuilder modelBuilder)
        {
            // Salary
            modelBuilder.Entity<Salary>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.Salaries)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Salary Components
            modelBuilder.Entity<SalaryComponent>()
                .HasOne(sc => sc.Salary)
                .WithMany(s => s.Components)
                .HasForeignKey(sc => sc.SalaryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}