using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AssetManager.ActionFilers.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var error = new ErrorModel
            (
                500,
                context.Exception.Message,
                context.Exception.StackTrace?.ToString()
            );

            context.Result = new JsonResult(error);
        }
        public class ErrorModel
        {
            public int StatusCode { get; set; }
            public string? Message { get; set; }
            public string? Details { get; set; }

            public ErrorModel(int statusCode, string? message, string? details = null)
            {
                StatusCode = statusCode;
                Message = message;
                Details = details;
            }
        }
    }
}
