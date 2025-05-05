namespace Transaction_Service.Models.DTOs
{
    public class WalletTypeDto
    {
        public Guid WalletTypeId { get; set; }

        public string Symbol { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string Tags { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public string UpdatedBy { get; set; }

        public string UpdatedAt { get; set; }

        public string DeletedBy { get; set; }
        public string DeletedAt { get; set; }
    }
}
