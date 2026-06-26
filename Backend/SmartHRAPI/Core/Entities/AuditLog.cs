namespace SmartHRAPI.Core.Entities
{
    public class AuditLog : BaseEntity
    {
        public string Action { get; set; } = string.Empty; // Create, Update, Delete
        public string EntityName { get; set; } = string.Empty;
        public int EntityId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime OperationDate { get; set; } = DateTime.UtcNow;
        public string? OldValues { get; set; } // JSON format
        public string? NewValues { get; set; } // JSON format
        public string? IPAddress { get; set; }
        public string? DeviceName { get; set; }
        public string? BrowserInfo { get; set; }
    }
}