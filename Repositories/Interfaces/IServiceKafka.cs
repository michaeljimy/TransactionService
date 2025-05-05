using Transaction_Service.Data.Entities;
using Transaction_Service.Models.DTOs.Requests;

namespace Transaction_Service.Interfaces
{
    public interface IServiceKafka
    {
        Task<string> ProduceAsync(string topic, string message);
        void UpdateWalletAsync(string topic, WalletUpdateRequest request);
    }
}