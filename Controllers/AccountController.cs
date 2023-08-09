

using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace Blog.Controllers;

//[Authorize] // atributo : possibilita acesso a qualquer aplicação, controler ou metodo
[ApiController] 
    
public class AccountController : ControllerBase

{
    //[AllowAnonymous] // permite q o login não precise de autorização[Authorize]
    [HttpPost("v1/accounts/")]
    //quando vários ActionResult tipos de retorno são possíveis em uma ação. 
    public async Task<IActionResult> Post(
        [FromBody] RegisterViewModel model,
        [FromServices] EmailService emailService,
        [FromServices] BlogDataContext context    )
    {
        if (!ModelState.IsValid)   
            return  BadRequest (new ResultViewModel<string>(ModelState.GetErrors()));
        var user = new User
        {
            Name = model.Name,
            Email= model.Email,
            Slug = model.Email.Replace(oldValue : "@", newValue : "-" ).Replace(oldValue: ".", newValue: "-")

        };
        // gerar senha forte
        var password = PasswordGenerator.Generate(length: 25 );
        user.PasswordHash = PasswordHasher.Hash(password);
        try
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            emailService.Send (
                user.Name,
                user.Email,
                subject: "Bem-vindo ao blog",
                body: $"Sua senha é <strng>{password}<strong>"  );

            // dynamic: quando não quer criar um result model estatico./duravel.
            return Ok(new ResultViewModel<dynamic>( new {
                user = user.Email, password     // vai retornar o email e o password p/ tela- não recomendado mas é so p/ teste
            }));
        
        }catch (DbUpdateException)
        {
            return StatusCode(400, new ResultViewModel <string> ("05X99 - Este  E-maoil já existe."));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel <string> ("05X04 - Falha interna no servidor."));
        }

    }

    [HttpPost("v1/accounts/login")]    
    public  async Task <IActionResult> Login(    // 
        [FromBody] LoginViewModel model ,    
        [FromServices] BlogDataContext context, 
        [FromServices] TokenService tokenService)
    {
        if (!ModelState.IsValid)   
            return  BadRequest (new ResultViewModel<string>(ModelState.GetErrors()));

        // tem q ler do banco os dados da pessoa 
        var user = await context   // DbDataContext
            .Users          // DbSet<User>
            .AsNoTracking()              
            .Include( navigationPropertyPath: x => x.Roles)
            .FirstOrDefaultAsync(x => x.Email == model.Email);         
        
        if (user == null)
            return StatusCode(401, new ResultViewModel <string> ("Usuário ou senha invalidos."));

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel <string> ("Usuário ou senha invalidos."));
                
        try         // gera um token
        {
            var token = tokenService.GenerateToken(user);
                    // retorna o token, se não colocar o null, ele pensa que o erro é o token
            return Ok(new ResultViewModel <string> (token, null));

        }catch
        {
            return StatusCode(500, new ResultViewModel <string> ("05X100 - Falha interna no servidor."));
        }
    
    }

  // Upload de imagens - arquivos staticos - no API
    [Authorize]     // só pode alterar a propria imagem, não tem acesso a outros usuarios
    [HttpPost("v1/accounts/upload-image")]
    public async Task<IActionResult> UploadImage(
        [FromBody] UploadImageViewModel model,
        [FromServices] BlogDataContext context )
    {
            var filename = $"{Guid.NewGuid().ToString()}.jpg";// gerando um nome via guid- garante unicidade
            var data = new Regex(@"^data:imageV[a-z]+;base64,") // formato do front-end. geralmente vem com essa info
                        .Replace(model.Base64Image, "");        // esta substituindo por vazio

            // C# precisa da imagem como um array de bytes[]
            var bytes = Convert.FromBase64String(data);

            try     // salvando a imagem
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/image/{filename}", bytes);
            
            }catch
            {
                return StatusCode(500, new ResultViewModel<string> ("05x04 - Falha interna no servidor"));
            }
            var email = User.Identity?.Name;
            // atualizando o usuario
            var user = await context // blog data context 
                        .Users
                        .FirstOrDefaultAsync(x=>x.Email == email);// usuario logado/ver RoleClaimsExtension
            if (user == null)     
                return NotFound(new ResultViewModel<Category>("Usuario não encontrado") );

            user.Image = $"https://localhost:0000/images/{filename}";
           // salvando o usuario com a imagem alterada
            try
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();

            }catch
            {
                return StatusCode(500, new ResultViewModel<string> ("05x04 - Falha interna no servidor"));
            }
        
            return Ok(new ResultViewModel<string> ("Imagem alterada com sucesso", null));
    }

}




