using Transaction_Service.Data.Entities;
using Transaction_Service.Models;

namespace Transaction_Service.Interfaces
{
    public interface IServiceTransaction
    {
        TransactionTypeEntity CreateTransactionType(TransactionTypeModel model);
        Task<TransactionEntity> DepositTransaction(DepositModel model);
        Task<TransactionEntity> DepositTransactionNew(DepositModel model);
        Task<TransactionEntity> DepositTransactionKafka(DepositModel model);
        Task<TransactionEntity> DepositTransactionWallet(DepositModel model);
        IEnumerable<TransactionEntity> GetTransaction(Guid TransactionId);
        IEnumerable<TransactionEntity> GetTransactions(Guid walletId);
        IEnumerable<TransactionTypeEntity> GetTransactionType();
        Task<TransactionEntity> TransferTransaction(TransferModel transferModel);
        Task<TransactionEntity> TransferTransactionNew(TransferModel transferModel);
        Task<TransactionEntity> WithdrawTransaction(WithdrawModel model);
    }
}
