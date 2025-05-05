using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Transaction_Service.Data.Entities
{
    [Table("tbl_transaction_event", Schema = "dbo")]
    public class TransactionEventEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public Guid EventId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TransactionId { get; set; }

        public TransactionEntity Transaction { get; set; }

        [StringLength(50)]
        public string EventType { get; set; } // e.g., "Created", "PaymentConfirmed", "Refunded", etc.

        // Store additional event details as needed. JSON can be a flexible option.
        public string EventData { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}