using Transaction_Service.Interfaces;
using Transaction_Service.Data.Entities;
using Transaction_Service.Data.Contexts;
using Transaction_Service.Models;
using Transaction_Service.Models.DTOs;
using Transaction_Service.Repositories.Interfaces;
using Transaction_Service.Models.DTOs.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;

namespace Transaction_Service.Services
{
    public class TransactionService : IServiceTransaction
    {
        private readonly ServiceDbContext _serviceDbContext;

        private readonly IServiceWallet _serviceWallet;

        private readonly IServiceKafka _serviceKafka;

        private readonly ConnectionStringsModel _connection;

        public TransactionService(ServiceDbContext serviceDbContext, IServiceWallet serviceWallet, IServiceKafka serviceKafka, IOptions<ConnectionStringsModel> optionsCon)
        {
            _serviceDbContext = serviceDbContext;
            _serviceWallet = serviceWallet;
            _serviceKafka = serviceKafka;
            _connection = optionsCon.Value;
        }

        public IEnumerable<TransactionEntity> GetTransactions(Guid walletId)
        {
            return _serviceDbContext.Transactions.Where((x) => x.InWalletId == walletId || x.OutWalletId == walletId);
        }

        public IEnumerable<TransactionEntity> GetTransaction(Guid TransactionId)
        {
            return _serviceDbContext.Transactions.Where((x) => x.TransactionId == TransactionId);
        }

        public async Task<TransactionEntity> DepositTransaction(DepositModel model)
        {
            WalletWithTypeDto walletOutWithType = await _serviceWallet.GetWalletAsync(model.OutWalletId);



            TransactionTypeEntity transactionType = GetTransactionType("ACC");
            //WalletDto walletIn = await _serviceWallet.GetSentWalletAsync(model.InWalletId);
            TransactionTypeEntity transactionTypeIn = GetTransactionType("CDP");
            WalletDto walletIn = new WalletDto()
            {
                WalletId = model.InWalletId,
                UserId = model.InUserId,
                Balance = 1000000000000,
            };

            if (walletIn == null)
            {
                throw new Exception("In Wallet not found");
            }

            if (transactionTypeIn == null)
            {
                throw new Exception("In Transaction Type not found");
            }

            if (walletOutWithType == null)
            {
                throw new Exception("Out Wallet not found");
            }

            WalletDto walletOut = walletOutWithType.Wallet;

            WalletTypeDto walletTypeDto = walletOutWithType.WalletType;

            if (walletOut == null)
            {
                throw new Exception("Out Wallet not found");
            }

            if (walletTypeDto == null)
            {
                throw new Exception("Out Wallet Type not found");
            }

            if (walletTypeDto.Tags.Contains("System") && walletIn.WalletId == walletOut.WalletId)
            {

            }
            else
            {
                if (model.Amount > walletOut.Balance)
                {
                    throw new Exception("Insufficient balance");
                }
            }

            if (model.Amount <= 0)
            {
                throw new Exception("Invalid amount");
            }

            if (model.InUserId != walletIn.UserId)
            {
                throw new Exception("Invalid user");
            }

            if (model.PaymentMethod == null)
            {
                throw new Exception("Invalid payment method");
            }

            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            decimal BalanceOut = 0;

            if (transactionType.TransactionTypeCode == "CDP")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "ACC")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRO")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRI")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }

            decimal BalanceIn = 0;


            if (transactionTypeIn.TransactionTypeCode == "CDP")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "ACC")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRO")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRI")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }

            TransactionEntity mTransaction = new TransactionEntity()
            {
                Tags = model.Tags,
                TransactionDate = DateTime.Now,
                AmountConfirmed = model.Amount,
                OutUserId = model.OutUserId,
                OutWalletId = model.OutWalletId,
                InUserId = model.InUserId,
                InWalletId = model.InWalletId,
                Amount = model.Amount,
                Status = "Completed",
                TransactionTypeId = transactionTypeIn.TransactionTypeId,
                PaymentMethod = model.PaymentMethod,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now,
            };
            try
            {
                await Transaction(mTransaction);
            }
            catch (DbUpdateException dbEx)
            {
                // Inspect or log the inner exception message for more details:
                var innerMessage = dbEx.InnerException?.Message;
                throw new Exception($"Error saving changes: {innerMessage}", dbEx);
            }

            WalletUpdateRequest KafTransfer = (WalletUpdateRequest)mTransaction;
            KafTransfer.InBalance = BalanceIn;
            if (walletTypeDto.Tags.Contains("System") && walletIn.WalletId == walletOut.WalletId)
            {
                KafTransfer.OutBalance = 0;
            }
            else
            {
                KafTransfer.OutBalance = BalanceOut;
            }
            UpdateWallet("Transaction_Deposit", KafTransfer);

            return mTransaction;
        }
        public async Task<TransactionEntity> DepositTransactionNew(DepositModel model)
        {
            WalletWithTypeDto walletOutWithType = await _serviceWallet.GetWalletAsync(model.OutWalletId);



            TransactionTypeEntity transactionType = GetTransactionType("ACC");
            //WalletDto walletIn = await _serviceWallet.GetSentWalletAsync(model.InWalletId);
            TransactionTypeEntity transactionTypeIn = GetTransactionType("CDP");
            WalletDto walletIn = new WalletDto()
            {
                WalletId = model.InWalletId,
                UserId = model.InUserId,
                Balance = 1000000000000,
            };

            if (walletIn == null)
            {
                throw new Exception("In Wallet not found");
            }

            if (transactionTypeIn == null)
            {
                throw new Exception("In Transaction Type not found");
            }

            if (walletOutWithType == null)
            {
                throw new Exception("Out Wallet not found");
            }

            WalletDto walletOut = walletOutWithType.Wallet;

            WalletTypeDto walletTypeDto = walletOutWithType.WalletType;

            if (walletOut == null)
            {
                throw new Exception("Out Wallet not found");
            }

            if (walletTypeDto == null)
            {
                throw new Exception("Out Wallet Type not found");
            }

            if (walletTypeDto.Tags.Contains("System") && walletIn.WalletId == walletOut.WalletId)
            {

            }
            else
            {
                if (model.Amount > walletOut.Balance)
                {
                    throw new Exception("Insufficient balance");
                }
            }

            if (model.Amount <= 0)
            {
                throw new Exception("Invalid amount");
            }

            if (model.InUserId != walletIn.UserId)
            {
                throw new Exception("Invalid user");
            }

            if (model.PaymentMethod == null)
            {
                throw new Exception("Invalid payment method");
            }

            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            decimal BalanceOut = 0;

            if (transactionType.TransactionTypeCode == "CDP")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "ACC")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRO")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRI")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }

            decimal BalanceIn = 0;


            if (transactionTypeIn.TransactionTypeCode == "CDP")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "ACC")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRO")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRI")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }

            TransactionEntity mTransaction = new TransactionEntity()
            {
                Tags = model.Tags,
                TransactionDate = DateTime.Now,
                AmountConfirmed = model.Amount,
                OutUserId = model.OutUserId,
                OutWalletId = model.OutWalletId,
                InUserId = model.InUserId,
                InWalletId = model.InWalletId,
                Amount = model.Amount,
                Status = "Completed",
                TransactionTypeId = transactionTypeIn.TransactionTypeId,
                PaymentMethod = model.PaymentMethod,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now,
            };
            try
            {
                using (SqlConnection con = new SqlConnection(_connection.ServiceDbContext))
                {
                    con.Open();
                    InsertTransaction(con, mTransaction);
                }
                // await Transaction(mTransaction);
            }
            catch (DbUpdateException dbEx)
            {
                // Inspect or log the inner exception message for more details:
                var innerMessage = dbEx.InnerException?.Message;
                throw new Exception($"Error saving changes: {innerMessage}", dbEx);
            }
            WalletUpdateRequest KafTransfer = (WalletUpdateRequest)mTransaction;
            KafTransfer.InBalance = BalanceIn;
            if (walletTypeDto.Tags.Contains("System") && walletIn.WalletId == walletOut.WalletId)
            {
                KafTransfer.OutBalance = 0;
            }
            else
            {
                KafTransfer.OutBalance = BalanceOut;
            }

            var _thread = new Thread(() => UpdateWallet("Transaction_Deposit", KafTransfer));
            _thread.Start();

            return mTransaction;
        }

        public async Task<TransactionEntity> DepositTransactionWallet(DepositModel model)
        {
            WalletWithTypeDto walletOutWithType = await _serviceWallet.GetWalletAsync(model.OutWalletId);



            TransactionTypeEntity transactionType = GetTransactionType("ACC");
            WalletDto walletIn = await _serviceWallet.GetSentWalletAsync(model.InWalletId);
            TransactionTypeEntity transactionTypeIn = GetTransactionType("CDP");
            //WalletDto walletIn = new WalletDto()
            //{
            //    WalletId = model.InWalletId,
            //    UserId = model.InUserId,
            //    Balance = 1000000000000,
            //};

            if (walletIn == null)
            {
                throw new Exception("In Wallet not found");
            }

            if (transactionTypeIn == null)
            {
                throw new Exception("In Transaction Type not found");
            }

            if (walletOutWithType == null)
            {
                throw new Exception("Out Wallet not found");
            }

            WalletDto walletOut = walletOutWithType.Wallet;

            WalletTypeDto walletTypeDto = walletOutWithType.WalletType;

            if (walletOut == null)
            {
                throw new Exception("Out Wallet not found");
            }

            if (walletTypeDto == null)
            {
                throw new Exception("Out Wallet Type not found");
            }

            if (walletTypeDto.Tags.Contains("System") && walletIn.WalletId == walletOut.WalletId)
            {

            }
            else
            {
                if (model.Amount > walletOut.Balance)
                {
                    throw new Exception("Insufficient balance");
                }
            }

            if (model.Amount <= 0)
            {
                throw new Exception("Invalid amount");
            }

            if (model.InUserId != walletIn.UserId)
            {
                throw new Exception("Invalid user");
            }

            if (model.PaymentMethod == null)
            {
                throw new Exception("Invalid payment method");
            }

            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            decimal BalanceOut = 0;

            if (transactionType.TransactionTypeCode == "CDP")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "ACC")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRO")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRI")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }

            decimal BalanceIn = 0;


            if (transactionTypeIn.TransactionTypeCode == "CDP")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "ACC")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRO")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRI")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }

            TransactionEntity mTransaction = new TransactionEntity()
            {
                Tags = model.Tags,
                TransactionDate = DateTime.Now,
                AmountConfirmed = model.Amount,
                OutUserId = model.OutUserId,
                OutWalletId = model.OutWalletId,
                InUserId = model.InUserId,
                InWalletId = model.InWalletId,
                Amount = model.Amount,
                Status = "Completed",
                TransactionTypeId = transactionTypeIn.TransactionTypeId,
                PaymentMethod = model.PaymentMethod,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now,
            };
            try
            {
                await Transaction(mTransaction);
            }
            catch (DbUpdateException dbEx)
            {
                // Inspect or log the inner exception message for more details:
                var innerMessage = dbEx.InnerException?.Message;
                throw new Exception($"Error saving changes: {innerMessage}", dbEx);
            }

            return mTransaction;
        }


        public async Task<TransactionEntity> DepositTransactionKafka(DepositModel model)
        {

            TransactionEntity mTransaction = new TransactionEntity()
            {
                Tags = model.Tags,
                TransactionDate = DateTime.Now,
                AmountConfirmed = model.Amount,
                OutUserId = model.OutUserId,
                OutWalletId = model.OutWalletId,
                InUserId = model.InUserId,
                InWalletId = model.InWalletId,
                Amount = model.Amount,
                Status = "Completed",
                TransactionTypeId = new Guid("bf64b948-0c9b-48a6-9946-29bbe2553c3c"),
                PaymentMethod = model.PaymentMethod,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now,
            };
            try
            {
                await Transaction(mTransaction);
            }
            catch (DbUpdateException dbEx)
            {
                // Inspect or log the inner exception message for more details:
                var innerMessage = dbEx.InnerException?.Message;
                throw new Exception($"Error saving changes: {innerMessage}", dbEx);
            }

            WalletUpdateRequest KafTransfer = (WalletUpdateRequest)mTransaction;
            KafTransfer.InBalance = 0;
            KafTransfer.OutBalance = 0;

            
            UpdateWallet("Transaction_Deposit", KafTransfer);

            return mTransaction;
        }

        public async Task<TransactionEntity> WithdrawTransaction(WithdrawModel model)
        {
            WalletWithTypeDto walletOutWithType = await _serviceWallet.GetWalletAsync(model.OutWalletId);

            TransactionTypeEntity transactionType = GetTransactionType("ACC");
            WalletDto walletIn = await _serviceWallet.GetSentWalletAsync(model.InWalletId);
            TransactionTypeEntity transactionTypeIn = GetTransactionType("TRO");

            if (walletIn == null)
            {
                throw new Exception("In Wallet not found");
            }

            if (transactionTypeIn == null)
            {
                throw new Exception("In Transaction Type not found");
            }

            if (walletOutWithType == null)
            {
                throw new Exception("Out Wallet not found");
            }

            WalletDto walletOut = walletOutWithType.Wallet;

            if (walletOut == null)
            {
                throw new Exception("Out Wallet not found");
            }

            if (model.Amount > walletOut.Balance)
            {
                throw new Exception("Insufficient balance");
            }

            if (model.Amount <= 0)
            {
                throw new Exception("Invalid amount");
            }

            if (model.InUserId != walletIn.UserId)
            {
                throw new Exception("Invalid user");
            }

            if (model.PaymentMethod == null)
            {
                throw new Exception("Invalid payment method");
            }

            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            decimal BalanceOut = 0;

            if (transactionType.TransactionTypeCode == "CDP")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "ACC")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRO")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRI")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }

            decimal BalanceIn = 0;


            if (transactionTypeIn.TransactionTypeCode == "CDP")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "ACC")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRO")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRI")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }

            TransactionEntity mTransaction = new TransactionEntity()
            {
                Tags = model.Tags,
                TransactionDate = DateTime.Now,
                AmountConfirmed = model.Amount,
                OutUserId = model.OutUserId,
                OutWalletId = model.OutWalletId,
                InUserId = model.InUserId,
                InWalletId = model.InWalletId,
                Amount = model.Amount,
                Status = "Completed",
                TransactionTypeId = transactionType.TransactionTypeId,
                PaymentMethod = model.PaymentMethod,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now,
            };
            try
            {
                await Transaction(mTransaction);
            }
            catch (DbUpdateException dbEx)
            {
                // Inspect or log the inner exception message for more details:
                var innerMessage = dbEx.InnerException?.Message;
                throw new Exception($"Error saving changes: {innerMessage}", dbEx);
            }

            WalletUpdateRequest KafTransfer = (WalletUpdateRequest)mTransaction;
            KafTransfer.InBalance = BalanceIn;
            KafTransfer.OutBalance = BalanceOut;
            UpdateWallet("Transaction_Withdraw", KafTransfer);

            return mTransaction;
        }

        public async Task<TransactionEntity> TransferTransaction(TransferModel model)
        {
            WalletWithTypeDto walletOutWithType = await _serviceWallet.GetWalletAsync(model.OutWalletId);

            TransactionTypeEntity transactionType = GetTransactionType("TRI");
            WalletDto walletIn = await _serviceWallet.GetSentWalletAsync(model.InWalletId);
            TransactionTypeEntity transactionTypeIn = GetTransactionType("TRO");

            if (walletIn == null)
            {
                throw new Exception("In Wallet not found");
            }

            if (transactionTypeIn == null)
            {
                throw new Exception("In Transaction Type not found");
            }

            if (walletOutWithType == null)
            {
                throw new Exception("Out Wallet not found");
            }

            WalletDto walletOut = walletOutWithType.Wallet;

            if (walletOut == null)
            {
                throw new Exception("Out Wallet not found");
            }

            if (model.Amount > walletOut.Balance)
            {
                throw new Exception("Insufficient balance");
            }

            if (model.Amount <= 0)
            {
                throw new Exception("Invalid amount");
            }

            if (model.InUserId != walletIn.UserId)
            {
                throw new Exception("Invalid user");
            }

            if (model.PaymentMethod == null)
            {
                throw new Exception("Invalid payment method");
            }

            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            decimal BalanceOut = 0;

            if (transactionType.TransactionTypeCode == "CDP")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "ACC")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRO")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRI")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }

            decimal BalanceIn = 0;


            if (transactionTypeIn.TransactionTypeCode == "CDP")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "ACC")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRO")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRI")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }

            TransactionEntity mTransaction = new TransactionEntity()
            {
                Tags = model.Tags,
                TransactionDate = DateTime.Now,
                AmountConfirmed = model.Amount,
                OutUserId = model.OutUserId,
                OutWalletId = model.OutWalletId,
                InUserId = model.InUserId,
                InWalletId = model.InWalletId,
                Amount = model.Amount,
                Status = "Completed",
                TransactionTypeId = transactionType.TransactionTypeId,
                PaymentMethod = model.PaymentMethod,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now,

            };

            try
            {
                await Transaction(mTransaction);
            }
            catch (DbUpdateException dbEx)
            {
                // Inspect or log the inner exception message for more details:
                var innerMessage = dbEx.InnerException?.Message;
                throw new Exception($"Error saving changes: {innerMessage}", dbEx);
            }

            WalletUpdateRequest KafTransfer = (WalletUpdateRequest)mTransaction;
            KafTransfer.InBalance = BalanceIn;
            KafTransfer.OutBalance = BalanceOut;
            UpdateWallet("Transaction_Transfer", KafTransfer);

            return mTransaction;
        }

        public async Task<TransactionEntity> TransferTransactionNew(TransferModel model)
        {
            WalletWithTypeDto walletOutWithType = await _serviceWallet.GetWalletAsync(model.OutWalletId);

            TransactionTypeEntity transactionType = GetTransactionType("TRI");
            WalletDto walletIn = await _serviceWallet.GetSentWalletAsync(model.InWalletId);
            TransactionTypeEntity transactionTypeIn = GetTransactionType("TRO");

            if (walletIn == null)
            {
                throw new Exception("In Wallet not found");
            }

            if (transactionTypeIn == null)
            {
                throw new Exception("In Transaction Type not found");
            }

            if (walletOutWithType == null)
            {
                throw new Exception("Out Wallet not found");
            }

            WalletDto walletOut = walletOutWithType.Wallet;

            if (walletOut == null)
            {
                throw new Exception("Out Wallet not found");
            }

            if (model.Amount > walletOut.Balance)
            {
                throw new Exception("Insufficient balance");
            }

            if (model.Amount <= 0)
            {
                throw new Exception("Invalid amount");
            }

            if (model.InUserId != walletIn.UserId)
            {
                throw new Exception("Invalid user");
            }

            if (model.PaymentMethod == null)
            {
                throw new Exception("Invalid payment method");
            }

            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            if (transactionType == null)
            {
                throw new Exception("Invalid transaction type");
            }
            decimal BalanceOut = 0;

            if (transactionType.TransactionTypeCode == "CDP")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "ACC")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRO")
            {
                BalanceOut = walletOut.Balance + model.Amount;
            }
            else if (transactionType.TransactionTypeCode == "TRI")
            {
                BalanceOut = walletOut.Balance - model.Amount;
            }

            decimal BalanceIn = 0;


            if (transactionTypeIn.TransactionTypeCode == "CDP")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "ACC")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRO")
            {
                BalanceIn = walletIn.Balance + model.Amount;
            }
            else if (transactionTypeIn.TransactionTypeCode == "TRI")
            {
                BalanceIn = walletIn.Balance - model.Amount;
            }

            TransactionEntity mTransaction = new TransactionEntity()
            {
                Tags = model.Tags,
                TransactionDate = DateTime.Now,
                AmountConfirmed = model.Amount,
                OutUserId = model.OutUserId,
                OutWalletId = model.OutWalletId,
                InUserId = model.InUserId,
                InWalletId = model.InWalletId,
                Amount = model.Amount,
                Status = "Completed",
                TransactionTypeId = transactionType.TransactionTypeId,
                PaymentMethod = model.PaymentMethod,
                CreatedBy = model.CreatedBy,
                CreatedAt = DateTime.Now,

            };

            try
            {

                await Transaction(mTransaction);
            }
            catch (DbUpdateException dbEx)
            {
                // Inspect or log the inner exception message for more details:
                var innerMessage = dbEx.InnerException?.Message;
                throw new Exception($"Error saving changes: {innerMessage}", dbEx);
            }

            WalletUpdateRequest KafTransfer = (WalletUpdateRequest)mTransaction;
            KafTransfer.InBalance = BalanceIn;
            KafTransfer.OutBalance = BalanceOut;
            UpdateWallet("Transaction_Transfer", KafTransfer);

            return mTransaction;
        }

        private async Task<TransactionEntity> Transaction(TransactionEntity entity)
        {
            try
            {
                await _serviceDbContext.Transactions.AddAsync(entity);
                await _serviceDbContext.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Inspect or log the inner exception message for more details:
                var innerMessage = dbEx.InnerException?.Message;
                throw new Exception($"Error saving changes: {innerMessage}", dbEx);
            }

            return entity;
        }
        private void InsertTransaction(SqlConnection connection, TransactionEntity entity)
        {
            string sqlcmd =
                @"INSERT INTO tblTransaction VALUES(
                    @Tags,@TransactionDate,@AmountConfirmed,@OutUserId,@OutWalletId,@InUserId,@InWalletId,
                    @Amount,@Status,@TransactionTypeId,@PaymentMethod,@CreatedBy,@CreatedAt,)";
            using (SqlCommand cmd = new SqlCommand(sqlcmd, connection))
            {
                cmd.Parameters.AddWithValue("@Tags", entity.Tags);
                cmd.Parameters.AddWithValue("@TransactionDate", entity.TransactionDate);
                cmd.Parameters.AddWithValue("@AmountConfirmed", entity.AmountConfirmed);
                cmd.Parameters.AddWithValue("@OutUserId", entity.OutUserId);
                cmd.Parameters.AddWithValue("@OutWalletId", entity.OutWalletId);
                cmd.Parameters.AddWithValue("@InUserId", entity.InUserId);
                cmd.Parameters.AddWithValue("@InWalletId", entity.InWalletId);
                cmd.Parameters.AddWithValue("@Amount", entity.Amount);
                cmd.Parameters.AddWithValue("@Status", entity.Status);
                cmd.Parameters.AddWithValue("@TransactionTypeId", entity.TransactionTypeId);
                cmd.Parameters.AddWithValue("@PaymentMethod", entity.PaymentMethod);
                cmd.Parameters.AddWithValue("@CreatedBy", entity.CreatedBy);
                cmd.Parameters.AddWithValue("@CreatedAt", entity.CreatedAt);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateWallet(string topic, WalletUpdateRequest walletRequest)
        {
            try
            {
                _serviceKafka.UpdateWalletAsync(topic, walletRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<TransactionTypeEntity> GetTransactionType()
        {
            return _serviceDbContext.TransactionTypes;
        }

        public TransactionTypeEntity GetTransactionType(string code)
        {
            var transactionType = _serviceDbContext.TransactionTypes
                                      .FirstOrDefault(x => x.TransactionTypeCode == code);

            if (transactionType == null)
            {
                throw new InvalidOperationException($"Transaction type not found for Code: {code}");
            }

            return transactionType;
        }


        public TransactionTypeEntity CreateTransactionType(TransactionTypeModel model)
        {

            try
            {
                TransactionTypeEntity transactionEntity = new TransactionTypeEntity()
                {
                    TransactionTypeName = model.TransactionTypeName,
                    TransactionTypeDescription = model.TransactionTypeDescription,
                    TransactionTypeCode = model.TransactionTypeCode,
                    CreatedBy = "System",
                };

                _serviceDbContext.TransactionTypes.Add(transactionEntity);
                _serviceDbContext.SaveChanges();
                return transactionEntity;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
