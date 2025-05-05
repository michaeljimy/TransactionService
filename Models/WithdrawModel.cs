using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Transaction_Service.Models
{
    public class WithdrawModel
    {
        [Required]
        public Guid OutUserId { get; set; }

        [Required]
        public Guid OutWalletId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }  // e.g., "CreditCard", "PayPal", etc.

        public string Tags { get; set; } // Fed, Point, Credit

        [Required]
        public Guid InUserId { get; set; }

        [Required]
        public Guid InWalletId { get; set; }

        [AllowNull]
        [HiddenInput]
        public string? CreatedBy { get; set; }
    }
}
