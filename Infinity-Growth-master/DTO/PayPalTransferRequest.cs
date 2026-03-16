using System;
using System.ComponentModel.DataAnnotations;

namespace DTO
{
    public class PayPalTransferRequest
    {
        [Required]
        public int IdUsuario { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser positivo.")]
        public decimal Monto { get; set; }
        
        [Required]
        [EmailAddress]
        public string PayPalEmail { get; set; }
        
        // Nota o concepto de la transferencia (opcional)
        public string Concepto { get; set; }
    }
    
    public class PayPalTransferResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }
} 