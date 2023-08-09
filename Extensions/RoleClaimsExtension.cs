
using System.Security.Claims;
using Blog.Models;

namespace Blog.Extensions;

// pega alista de roles/perfil do usuario e transforma/converte numa lista de claims
// claims: possibilitam as verificações de user/role necessário para o login
public static class  RoleClaimsExtension    // extensão : sempre classe estatica
{
    public static IEnumerable <Claim> GetClaims(this  User user)
    {
        // cria uma lista de claims 
        var result = new List<Claim>
        {
            new (ClaimTypes.Name, user.Email) // os claims são sempre strings
        };
        // pega todos os roles/perfis do usuario, faz o select e retorna na forma de lista de claims
        result.AddRange(
            user.Roles.Select(role=> new Claim(ClaimTypes.Role, role.Slug)));


        return(result);
    }
}