
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Blog.Attributes;

/* não sera utilizado nesse projeto, foi só para conhecimento */

// verifica se a ckave Api-key esta presente no requisição.

// atrributo valido para classe e/ou metodo
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
// atributo é um filtro de ação. ele vai interromper a ececução da ação para verificar se 
// a key está presente na requisição
public class ApiKeyAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, 
        ActionExecutionDelegate next)
    {
        if (!context.HttpContext.Request.Query.TryGetValue
            (Configuration.ApiKeyName, out var extractedApikey))
            {
                context.Result = new ContentResult ()
                {
                    StatusCode = 401,
                    Content= "Apikey não encontrada"
                };            
                return;
            }
            

        if (!Configuration.ApiKey.Equals(extractedApikey))
            {
                context.Result = new ContentResult ()
                {
                    StatusCode = 403,
                    Content = "Acesso não autorizado"
                };            
                return;
            }

       await next();
        
    }
}