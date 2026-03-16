namespace AppLogic.Services;

public class PayPalCapture
{
    public bool Success { get; set; }
    public decimal Amount { get; set; } 
    public string Currency { get; set; } 
    public string? PayPalCaptureId { get; set; } 
    public string? FailureMessage { get; set; } 
}