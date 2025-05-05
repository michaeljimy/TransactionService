using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Transaction_Service.Models.DTOs
{
    public class WalletDto
    {
        public Guid WalletId { get; set; }

        public Guid WalletTypeId { get; set; }

        public Guid UserId { get; set; }


        public string Name { get; set; }

        public decimal Balance { get; set; }

        public decimal Frozen { get; set; }

        public decimal Available => Balance - Frozen; // Calculated on demand

        public decimal Reserved { get; set; }

        public decimal Total => Reserved + Balance; // Calculated on demand

        public decimal Withdrawal { get; set; }

        public decimal Deposit { get; set; }

        public decimal Fee { get; set; }


        public bool IsVerified { get; set; }

        public string VerificationBy { get; set; }

        public string VerificatuionAt { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsLocked { get; set; }

        public string LockReason { get; set; }

        public string LockNote { get; set; }

        public string LockBy { get; set; }

        public string LockAt { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedAt { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedAt { get; set; }

        public string DeletedBy { get; set; }

        public string DeletedAt { get; set; }

        public string DeletedReason { get; set; }
    }
}
