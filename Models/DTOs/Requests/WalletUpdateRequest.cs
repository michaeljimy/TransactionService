using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Transaction_Service.Data.Entities;

namespace Transaction_Service.Models.DTOs.Requests
{
    public class WalletUpdateRequest
    {
        public long Id { get; set; }

        public string Tags { get; set; }

        public Guid TransactionId { get; set; } = Guid.NewGuid();

        public Guid OutUserId { get; set; }

        public Guid OutWalletId { get; set; }

        public decimal OutBalance { get; set; } = 0;

        public Guid InUserId { get; set; }

        public Guid InWalletId { get; set; }

        public decimal InBalance { get; set; } = 0;

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public decimal Amount { get; set; }

        public decimal AmountConfirmed { get; set; }

        public Guid TransactionTypeId { get; set; }

        public string Status { get; set; } // e.g., "Pending", "Completed", "Failed"

        public string PaymentMethod { get; set; }  // e.g., "CreditCard", "PayPal", etc.

        public string? Description { get; set; }

        public string? TransactionReference { get; set; }

        public string? PaymentReference { get; set; }

        public string? ErrorMessage { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public static explicit operator WalletUpdateRequest(TransactionEntity transaction)
        {
            return new WalletUpdateRequest
            {
                Id = transaction.Id,
                Tags = transaction.Tags,
                TransactionId = transaction.TransactionId,
                OutUserId = transaction.OutUserId,
                OutWalletId = transaction.OutWalletId,
                InUserId = transaction.InUserId,
                InWalletId = transaction.InWalletId,
                TransactionDate = transaction.TransactionDate,
                Amount = transaction.Amount,
                AmountConfirmed = transaction.AmountConfirmed,
                TransactionTypeId = transaction.TransactionTypeId,
                Status = transaction.Status,
                PaymentMethod = transaction.PaymentMethod,
                Description = transaction.Description,
                TransactionReference = transaction.TransactionReference,
                PaymentReference = transaction.PaymentReference,
                ErrorMessage = transaction.ErrorMessage,
                CreatedBy = transaction.CreatedBy,
                CreatedAt = transaction.CreatedAt,
                UpdatedBy = transaction.UpdatedBy,
                UpdatedAt = transaction.UpdatedAt
            };
        }
    }
}
