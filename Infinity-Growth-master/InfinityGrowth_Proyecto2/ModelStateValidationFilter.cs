using DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InfinityGrowth_Proyecto2
{
    public sealed class ModelStateValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            var message = "Validation failed.";

            if (context.ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault() is { } firstError &&
                !string.IsNullOrWhiteSpace(firstError.ErrorMessage))
            {
                message = firstError.ErrorMessage;
            }

            var response = new API_Response
            {
                Result = "ERROR",
                Data = null,
                Message = message
            };

            context.Result = new JsonResult(response) { StatusCode = StatusCodes.Status400BadRequest };
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}

