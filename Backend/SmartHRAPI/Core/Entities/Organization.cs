namespace SmartHRAPI.Core.Entities
{
    public class Company : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string LogoPath { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CRNumber { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public DateTime FoundedDate { get; set; }

        // Navigation Properties
        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    }

    public class Branch : BaseEntity
    {
        public int CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Company Company { get; set; } = null!;
        public ICollection<Department> Departments { get; set; } = new List<Department>();
    }

    public class Department : BaseEntity
    {
        public int BranchId { get; set; }
        public int? ParentDepartmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Manager { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Branch Branch { get; set; } = null!;
        public Department? ParentDepartment { get; set; }
        public ICollection<Department> SubDepartments { get; set; } = new List<Department>();
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }

    public class JobTitle : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int SalaryBandId { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }

    public class Division : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}