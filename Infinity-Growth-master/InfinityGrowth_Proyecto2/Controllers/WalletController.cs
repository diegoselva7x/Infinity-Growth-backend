using AppLogic;
using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfinityGrowth_Proyecto2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly WalletManager _walletManager;

        public WalletController(WalletManager walletManager)
        {
            _walletManager = walletManager;
        }

        /// <summary>
        /// Obtiene el balance del wallet de un usuario
        /// </summary>
        [HttpGet("balance/{idUsuario}")]
        public IActionResult GetBalance(int idUsuario)
        {
            try
            {
                var balance = _walletManager.GetWalletBalance(idUsuario);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener el balance: " + ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el historial de transacciones del wallet de un usuario
        /// </summary>
        [HttpGet("transactions/{idUsuario}")]
        public IActionResult GetTransactions(int idUsuario)
        {
            try
            {
                var transactions = _walletManager.GetWalletTransactionsByUser(idUsuario);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener las transacciones: " + ex.Message);
            }
        }

        /// <summary>
        /// Registra un depósito en el wallet
        /// </summary>
        [HttpPost("deposit")]
        public IActionResult Deposit([FromBody] WalletTransaction transaction)
        {
            try
            {
                // Forzar que sea un depósito
                transaction.TipoFondo = true;
                
                _walletManager.RegisterDeposit(
                    transaction.IdUsuario,
                    transaction.Monto
                );
                
                return Ok(new { Message = "Depósito registrado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al registrar el depósito: " + ex.Message);
            }
        }

        /// <summary>
        /// Registra un retiro en el wallet
        /// </summary>
        [HttpPost("withdrawal")]
        public IActionResult Withdrawal([FromBody] WalletTransaction transaction)
        {
            try
            {
                // Forzar que sea un retiro
                transaction.TipoFondo = false;
                
                bool success = _walletManager.RegisterWithdrawal(
                    transaction.IdUsuario,
                    transaction.Monto
                );
                
                if (success)
                {
                    return Ok(new { Message = "Retiro registrado correctamente" });
                }
                else
                {
                    return BadRequest(new { Message = "Fondos insuficientes para realizar el retiro" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al registrar el retiro: " + ex.Message);
            }
        }

        /// <summary>
        /// Endpoint para compra de acciones (retiro de fondos)
        /// </summary>
        [HttpPost("purchase-stock")]
        public IActionResult PurchaseStock([FromBody] WalletTransaction transaction)
        {
            try
            {
                bool success = _walletManager.WithdrawForStockPurchase(
                    transaction.IdUsuario,
                    transaction.Monto
                );
                
                if (success)
                {
                    return Ok(new { Message = "Fondos retirados correctamente para la compra de acciones" });
                }
                else
                {
                    return BadRequest(new { Message = "Fondos insuficientes para realizar la compra" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al procesar la compra: " + ex.Message);
            }
        }

        /// <summary>
        /// Endpoint para venta de acciones (depósito de fondos)
        /// </summary>
        [HttpPost("sell-stock")]
        public IActionResult SellStock([FromBody] WalletTransaction transaction)
        {
            try
            {
                _walletManager.DepositFromStockSale(
                    transaction.IdUsuario,
                    transaction.Monto
                );
                
                return Ok(new { Message = "Fondos depositados correctamente por la venta de acciones" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al procesar la venta: " + ex.Message);
            }
        }
    }
}
