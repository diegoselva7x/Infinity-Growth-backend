using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Crud;
using DTO;

namespace AppLogic
{
    public class WalletManager
    {
        private readonly WalletCrud _walletCrud;

        public WalletManager(WalletCrud walletCrud)
        {
            _walletCrud = walletCrud;
        }

        /// <summary>
        /// Obtiene el balance del wallet de un usuario
        /// </summary>
        public WalletBalance GetWalletBalance(int idUsuario)
        {
            return _walletCrud.GetWalletBalance(idUsuario);
        }

        /// <summary>
        /// Obtiene las transacciones del wallet de un usuario
        /// </summary>
        public List<Wallet> GetWalletTransactionsByUser(int idUsuario)
        {
            return _walletCrud.GetWalletTransactionsByUser(idUsuario);
        }

        /// <summary>
        /// Registra un depósito en el wallet
        /// </summary>
        public void RegisterDeposit(int idUsuario, decimal monto)
        {
            var transaction = new WalletTransaction
            {
                IdUsuario = idUsuario,
                Monto = monto,
                TipoFondo = true, // Depósito
            };

            _walletCrud.RegisterWalletTransaction(transaction);
        }

        /// <summary>
        /// Registra un retiro en el wallet
        /// </summary>
        public bool RegisterWithdrawal(int idUsuario, decimal monto)
        {
            // Verificar que haya fondos suficientes
            var balance = GetWalletBalance(idUsuario);
            if (balance.Balance < monto)
            {
                return false; // Fondos insuficientes
            }

            var transaction = new WalletTransaction
            {
                IdUsuario = idUsuario,
                Monto = monto,
                TipoFondo = false, // Retiro
            };

            _walletCrud.RegisterWalletTransaction(transaction);
            return true;
        }

        /// <summary>
        /// Registra un retiro para compra de acciones
        /// </summary>
        public bool WithdrawForStockPurchase(int idUsuario, decimal monto)
        {
            return RegisterWithdrawal(idUsuario, monto);
        }

        /// <summary>
        /// Registra un depósito por venta de acciones
        /// </summary>
        public void DepositFromStockSale(int idUsuario, decimal monto)
        {
            RegisterDeposit(idUsuario, monto);
        }
        
        /// <summary>
        /// Verifica si un usuario tiene saldo suficiente para una transacción
        /// </summary>
        public bool HasSufficientBalance(int idUsuario, decimal monto)
        {
            var balance = GetWalletBalance(idUsuario);
            return balance.Balance >= monto;
        }
        
        /// <summary>
        /// Realiza un retiro para transferencia a PayPal
        /// </summary>
        public bool WithdrawForPayPalTransfer(int idUsuario, decimal monto, string transactionId)
        {
            // Verificar que haya fondos suficientes
            var balance = GetWalletBalance(idUsuario);
            if (balance.Balance < monto)
            {
                return false; // Fondos insuficientes
            }

            var transaction = new WalletTransaction
            {
                IdUsuario = idUsuario,
                Monto = monto,
                TipoFondo = false, // Retiro
                // Guardamos el concepto en el objeto, aunque el SP no lo use actualmente
                // Esto permite que sea más fácil añadir esta funcionalidad en el futuro
                Concepto = $"Transferencia a PayPal: {transactionId}"
            };

            _walletCrud.RegisterWalletTransaction(transaction);
            return true;
        }
    }
}
