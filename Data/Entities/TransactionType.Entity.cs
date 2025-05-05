using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Transaction_Service.Data.Entities
{
    [Table("tbl_transaction_type", Schema = "dbo")]
    [Index(nameof(TransactionTypeName), IsUnique = true)]
    [Index(nameof(TransactionTypeCode), IsUnique = true)]
    public class TransactionTypeEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public Guid TransactionTypeId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(50)]
        public string TransactionTypeName { get; set; }

        [AllowNull]
        [StringLength(100)]
        public string TransactionTypeDescription { get; set; }

        [Required]
        [StringLength(20)]
        public string TransactionTypeCode { get; set; }

        [AllowNull]
        [HiddenInput]
        public bool IsActive { get; set; } = true;

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