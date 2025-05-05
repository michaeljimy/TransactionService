using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Transaction_Service.Data.Entities
{
    [Table("tbl_refund_transaction", Schema = "dbo")]
    [Index(nameof(RefundId), IsUnique = true)]
    public class RefundTransactionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public Guid RefundId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OriginalTransactionId { get; set; }

        public TransactionEntity OriginalTransaction { get; set; }

        public DateTime RefundDate { get; set; } = DateTime.UtcNow;
        public decimal RefundAmount { get; set; }

        [StringLength(20)]
        public string RefundStatus { get; set; } // e.g., "Pending", "Completed", "Failed"

        public string RefundReason { get; set; }

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