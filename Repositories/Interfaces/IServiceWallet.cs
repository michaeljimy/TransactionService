using Transaction_Service.Models.DTOs;
using Transaction_Service.Models.DTOs.Requests;

namespace Transaction_Service.Repositories.Interfaces
{
    public interface IServiceWallet
    {
        Task<WalletDto> GetSentWalletAsync(Guid walletId);
        Task<WalletWithTypeDto> GetWalletAsync(Guid walletId);
        Task<WalletDto> updateWalletAsync(WalletUpdateRequest request);
    }
}
