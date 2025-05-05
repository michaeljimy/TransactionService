using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using Transaction_Service.Data.Entities;

namespace Transaction_Service.Models
{
    public class DepositModel
    {
        [Required]
        public Guid OutUserId { get; set; }

        [Required]
        public Guid OutWalletId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }  // e.g., "Lotto", "Machine", etc.

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
