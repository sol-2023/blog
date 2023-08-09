using System.IdentityModel.Tokens.Jwt;
using Blog.Models;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Blog.Extensions;

namespace Blog.Services;

public class TokenService
{
    public string GenerateToken(User user)
    {
        var TokenHandler = new JwtSecurityTokenHandler(); //manipulador de token
        // Converte a string de key em um array de bits
        var key = Encoding.ASCII.GetBytes( Configuration.JwtKey);
        var claims = user.GetClaims(); // pega a lista de claims gerada na extensão
        var tokenDescriptor = new SecurityTokenDescriptor// descriptor = configurações do token
        {   
            // assuntos. Claims = afirmações
                    /*Subject = new ClaimsIdentity(new Claim []
                    {                             // claims: possibilitam as verificações de user/role
                        new (ClaimTypes.Name, "andre baltieri"), // user.Identity.Name 
                        new (ClaimTypes.Role, "user"),          // user.isInRole
                        new (ClaimTypes.Role, "admin"),         // admin.isInRole
                        new ("fruta", "banana")

                    }),** foi sunstituido pelo comando abaixo    */

            Subject = new ClaimsIdentity(claims),
            // Expira em 8 horas, depois precisa logar de novo
            Expires = DateTime.UtcNow.AddHours(8), 

            //encripta e desencripta. pede a chave  e o tipo de algoritmo p/ encriptar
            SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)

        };

        var token = TokenHandler.CreateToken(tokenDescriptor);    
        // token = tipo security token q retorna string. WriteToken faz a conversão
        return TokenHandler.WriteToken(token) ;

    }
}