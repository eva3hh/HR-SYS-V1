namespace SmartHRAPI.Core.Entities
{
    public class Salary : BaseEntity
    {
        public int EmployeeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BasicSalary { get; set; }
        
        // Allowances
        public decimal HousingAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal MealAllowance { get; set; }
        public decimal NatureOfWorkAllowance { get; set; }
        public decimal OtherAllowances { get; set; }
        public decimal TotalAllowances { get; set; }
        
        // Bonuses & Incentives
        public decimal BonusAmount { get; set; }
        public decimal CommissionAmount { get; set; }
        public decimal IncentiveAmount { get; set; }
        public decimal RewardAmount { get; set; }
        public decimal TotalBonuses { get; set; }
        
        // Deductions
        public decimal LateDeduction { get; set; }
        public decimal AbsenceDeduction { get; set; }
        public decimal PenaltyAmount { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal TotalDeductions { get; set; }
        
        // Tax & Insurance
        public decimal InsuranceDeduction { get; set; }
        public decimal TaxDeduction { get; set; }
        public decimal GovernmentDeduction { get; set; }
        
        // Advances & Installments
        public decimal AdvanceAmount { get; set; }
        public decimal InstallmentAmount { get; set; }
        
        // Calculation Basis
        public int WorkingDays { get; set; }
        public decimal WorkingHours { get; set; }
        public decimal OvertimeHours { get; set; }
        public decimal OvertimeAmount { get; set; }
        
        // Final
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        
        // Status
        public string Status { get; set; } = "Draft"; // Draft, Calculated, Approved, Finalized
        public DateTime CalculationDate { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Notes { get; set; }

        // Navigation Properties
        public Employee Employee { get; set; } = null!;
        public ICollection<SalaryComponent> Components { get; set; } = new List<SalaryComponent>();
    }

    public class SalaryComponent : BaseEntity
    {
        public int SalaryId { get; set; }
        public string ComponentName { get; set; } = string.Empty;
        public string ComponentType { get; set; } = string.Empty; // Allowance, Bonus, Deduction, etc.
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;

        // Navigation Properties
        public Salary Salary { get; set; } = null!;
    }

    public class Allowance : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsPercentage { get; set; }
        public string ApplicableTo { get; set; } = "All"; // All, Specific Department, Specific Job
        public bool IsActive { get; set; } = true;
    }

    public class Deduction : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public bool IsPercentage { get; set; }
        public string DeductionType { get; set; } = string.Empty; // Tax, Insurance, Loan, etc.
        public bool IsActive { get; set; } = true;
    }

    public class Bonus : BaseEntity
    {
        public int EmployeeId { get; set; }
        public string BonusType { get; set; } = string.Empty; // Performance, Festival, Annual, etc.
        public decimal Amount { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Paid
        public string? Notes { get; set; }
    }

    public class Advance : BaseEntity
    {
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public int NumberOfInstallments { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime ApprovalDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Partially Paid, Fully Paid
        public decimal PaidAmount { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

    public class Penalty : BaseEntity
    {
        public int EmployeeId { get; set; }
        public string PenaltyType { get; set; } = string.Empty; // Late, Absence, Misconduct, etc.
        public decimal Amount { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Applied
        public string? Notes { get; set; }
    }
}