using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AppLogic.Services; 
using DTO;
using AppLogic;
using Microsoft.AspNetCore.Cors;
using System.Globalization;

[EnableCors("AllowAll")]
[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly StockManager _stockManager;
    private readonly PriceManager _priceManager;

    public StockController(StockManager stockManager, PriceManager priceManager)
    {
        _stockManager = stockManager;
        _priceManager = priceManager;
    }

    [HttpGet("{symbol}")]
    public async Task<ActionResult<TwelveDataResponse>> GetStockData(string symbol, [FromQuery] string range = "1day")
    {
        try
        {
            string rangoConvertido = MapearRango(range);
            var data = await _stockManager.ObtenerStockAsync(symbol, rangoConvertido);
            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Error al obtener datos del mercado.", detalle = ex.Message });
        }
    }



    [HttpGet("lastprice/{symbol}")]
    public async Task<ActionResult<string>> GetLastPriceInLastMinute(string symbol)
    {
        try
        {
            var lastPrice = await _priceManager.ObtenerPrecioAsync(symbol);  // Esto devolverá un string con el precio

            if (!string.IsNullOrEmpty(lastPrice))
            {
                return Ok(lastPrice);  // Regresamos el precio de la acción
            }

            return NotFound($"No se encontraron datos para el símbolo {symbol}.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }


    private string MapearRango(string rango)
    {
        return rango.ToLower() switch
        {
            "1day" => "1day",
            "7day" => "1week",
            "1month" => "1month",
            "1year" => "1year",
            "max" => "5year", // TwelveData no tiene "max", pero 5year es lo más largo
            _ => "1day" // Valor por defecto
        };
        }

    [HttpGet("StockSuggestion")]
    public async Task<ActionResult<object>> GetStockSuggestion()
    {
        var symbols = new[] { "TSLA", "NVDA", "NFLX", "GOOGL", "MSFT", "AAPL", };
        var results = new List<StockPerformance>();

        foreach (var symbol in symbols)
        {
            var stockData = await _stockManager.ObtenerStockAsync(symbol, "1day");

            if (stockData == null || stockData.Values == null || stockData.Values.Count == 0)
            {
                continue;
            }

            var lastValue = stockData.Values.FirstOrDefault();

            if (lastValue == null || string.IsNullOrWhiteSpace(lastValue.Open))
            {
                continue;
            }

            decimal openPrice;
            bool parsedOpen = decimal.TryParse(lastValue.Open, NumberStyles.Any, CultureInfo.InvariantCulture, out openPrice);
            if (!parsedOpen)
            {
                continue;
            }

            var lastPriceStr = await _priceManager.ObtenerPrecioAsync(symbol);

            if (string.IsNullOrWhiteSpace(lastPriceStr))
            {
                continue;
            }

            decimal currentPrice;
            bool parsedCurrent = decimal.TryParse(lastPriceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out currentPrice);
            if (!parsedCurrent)
            {
                continue;
            }

            var performance = new StockPerformance
            {
                Symbol = symbol,
                OpenPrice = openPrice,
                CurrentPrice = currentPrice
            };

            results.Add(performance);
        }

        if (results.Count == 0)
        {
            return NotFound("No se pudo calcular el rendimiento de las acciones.");
        }

        StockPerformance best = null;
        decimal bestChange = decimal.MinValue;

        foreach (var stock in results)
        {
            decimal change = stock.GetPercentageChange();

            // Solo considerar si el cambio fue positivo
            if (change > 0 && change > bestChange)
            {
                best = stock;
                bestChange = change;
            }
        }

        if (best == null)
        {
            return Ok(new
            {
                message = "Hoy ninguna de las acciones tuvo un rendimiento positivo.",
                symbol = (string)null,
                percentage = 0,
                openPrice = 0,
                currentPrice = 0
            });
        }

        return Ok(new
        {
            message = $" Recomendación del día: {best.Symbol} ha subido {best.GetPercentageChange():F2}%.",
            symbol = best.Symbol,
            percentage = best.GetPercentageChange(),
            openPrice = best.OpenPrice,
            currentPrice = best.CurrentPrice
        });
    }



}