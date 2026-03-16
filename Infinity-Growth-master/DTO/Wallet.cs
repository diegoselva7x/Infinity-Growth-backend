using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class Wallet
    {
        public int IdWallet { get; set; }
        
        // Tipo de fondo (true para depósito, false para retiro)
        public bool TipoFondo { get; set; }
        
        public int IdUsuario { get; set; }
        
        // Monto de la transacción
        public decimal Monto { get; set; }
        
        // Fecha de la transacción
        public DateTime Fecha { get; set; }
    }
    
    // DTO para transacciones del wallet (depósitos/retiros)
    public class WalletTransaction
    {
        public int IdUsuario { get; set; }
        public decimal Monto { get; set; }
        public bool TipoFondo { get; set; } // true = depósito, false = retiro
        public string? Concepto { get; set; } // Descripción de la transacción
    }
    
    // DTO para obtener el balance del wallet
    public class WalletBalance
    {
        public int IdUsuario { get; set; }
        public decimal Balance { get; set; }
    }
}