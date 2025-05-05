using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Transaction_Service.Data.Entities
{
    [Table("tbl_transaction", Schema = "dbo")]

    [Index(nameof(TransactionId), IsUnique = true)]
    public class TransactionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public Guid TransactionId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid OutUserId { get; set; }

        [Required]
        public Guid OutWalletId { get; set; }

        public string Tags { get; set; } 

        [Required]
        public Guid InUserId { get; set; }

        [Required]
        public Guid InWalletId { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public decimal Amount { get; set; }

        public decimal AmountConfirmed { get; set; }

        [Required]
        public Guid TransactionTypeId { get; set; }
        
        public TransactionTypeEntity TransactionType { get; set; } // e.g., "Deposit", "Withdrawal", etc. bbeb5607-2138-4767-84a7-61645a814e6c // Deposit


        [StringLength(20)]
        public string Status { get; set; } // e.g., "Pending", "Completed", "Failed"

        [StringLength(50)]
        public string PaymentMethod { get; set; }  // e.g., "CreditCard", "PayPal", etc.

        public string? Description { get; set; }

        public string? TransactionReference { get; set; }

        public string? PaymentReference { get; set; }

        [AllowNull]
        public string? ErrorMessage { get; set; }

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