using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Transaction_Service.Data.Entities;
using Transaction_Service.Interfaces;
using Transaction_Service.Models;

namespace Transaction_Service.Controllers
{
    [ApiController]
    [Route("api/v1/transaction")]
    public class TransactionController : ControllerBase
    {

        private readonly IServiceKafka _kafkaService;
        private readonly IServiceTransaction _serviceTransaction;

        public TransactionController(IServiceKafka kafkaService, IServiceTransaction serviceTransaction)
        {
            _kafkaService = kafkaService;
            _serviceTransaction = serviceTransaction;
        }

        [Authorize]
        [HttpGet("{transactionId}", Name = "GetTransaction")]
        public ActionResult<IEnumerable<TransactionEntity>> GetTransaction(Guid transactionId)
        {
            var res = _serviceTransaction.GetTransaction(transactionId);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("list/{walletId}", Name = "GetTransactionByWalletId")]
        public ActionResult<IEnumerable<TransactionEntity>> GetTransactionList(Guid walletId)
        {
            var res = _serviceTransaction.GetTransactions(walletId);
            return Ok(res);
        }

        
        [Authorize]
        [HttpPost("core/deposit", Name = "Post Deposit CDP")]
        public async Task<IActionResult> PostDepositCDP([FromBody] DepositModel model)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);


                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim not found in token.");
                }

                string userId = userIdClaim.Value;

                model.CreatedBy = userId;

                // var res = await _serviceTransaction.DepositTransaction(model);
                var res = await _serviceTransaction.DepositTransactionNew(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize]
        [HttpPost("core/deposit/wallet", Name = "Post Deposit CDP")]
        public async Task<IActionResult> PostDepositCDPW([FromBody] DepositModel model)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);


                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim not found in token.");
                }

                string userId = userIdClaim.Value;

                model.CreatedBy = userId;

                var res = await _serviceTransaction.DepositTransactionWallet(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize]
        [HttpPost("core/deposit/kafka", Name = "Post Deposit CDP")]
        public async Task<IActionResult> PostDepositCDPK([FromBody] DepositModel model)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);


                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim not found in token.");
                }

                string userId = userIdClaim.Value;

                model.CreatedBy = userId;

                var res = await _serviceTransaction.DepositTransactionKafka(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("core/withdrawal", Name = "Post Withdrawal ACC")]
        public async Task<IActionResult> PostWithdrawalACC([FromBody] WithdrawModel model)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);


                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim not found in token.");
                }

                string userId = userIdClaim.Value;

                model.CreatedBy = userId;

                var res = await _serviceTransaction.WithdrawTransaction(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost("core/transfer", Name = "Post Transfer  TRI to TRO")]
        public async Task<IActionResult> PostTransfer([FromBody] TransferModel model)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);


                if (userIdClaim == null)
                {
                    return Unauthorized("User ID claim not found in token.");
                }

                string userId = userIdClaim.Value;

                model.CreatedBy = userId;

                var res = await _serviceTransaction.TransferTransaction(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize]
        [HttpGet("type", Name = "GetTransactionType")]
        public ActionResult<IEnumerable<TransactionTypeEntity>> GetTransactionType()
        {
            var res = _serviceTransaction.GetTransactionType();
            return Ok(res);
        }

        [Authorize]
        [HttpPost("type", Name = "PostTransactionType")]
        public IActionResult Post([FromBody] TransactionTypeModel model)
        {
            try
            {
                var res = _serviceTransaction.CreateTransactionType(model);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        

        [HttpGet("TestKafkaAsync")]
        public async Task<IActionResult> GetTestKafkaAsync()
        {
            var results = await _kafkaService.ProduceAsync("test", "Weather forecast request received");
            return Ok(results);
        }

    }
}
