using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Transaction_Service.Data.Entities
{
    [Table("tbl_transaction_log", Schema = "dbo")]
    public class TransactionLogEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public Guid LogId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TransactionId { get; set; }

        public TransactionEntity Transaction { get; set; }

        [Required]
        [StringLength(50)]
        public string Action { get; set; } // e.g., "Created", "Updated", "Confirmed", "Failed"

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Description { get; set; }

        [AllowNull]
        [HiddenInput]
        public string? CreatedBy { get; set; }

        [HiddenInput]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [AllowNull]
        [HiddenInput]
        public string? UpdatedBy { get; set; }

        [AllowNull]
        [HiddenInput]
        public DateTime? UpdatedAt { get; set; }
    }
}