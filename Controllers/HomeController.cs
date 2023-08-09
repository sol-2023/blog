
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using Blog.Attributes;

namespace Blog.Controllers
{
    [ApiController] // esta informando q é uma api
    [Route ("")]
    public class HomeController : ControllerBase
    {
        // Health check - para saber se o site esta on ou off-line
        [HttpGet("")] // raiz. o status da raiz será sempre OK -  ping no site, retorna ok
                      //        [ApiKey]        // verifica se é um ApiKey - metodo alternativo de autenticação
        public IActionResult Get(
            [FromServices] IConfiguration config)
        {
            var env = config.GetValue <string>("Env");
            return Ok(new
            {
                enviroment = env

            }); 
        }

    }

}
