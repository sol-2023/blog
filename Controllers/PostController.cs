using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
public class PostController : ControllerBase
{
    [HttpGet("v1/posts")]
    public async Task<IActionResult> GetAsync(
        [FromServices] BlogDataContext context, 
        [FromQuery] int page = 0,           // ** paginação
        [FromQuery] int pageSize = 25
    )
    {   
        // conta quantidade de registros - isso causa uma operação a mais de banco. 
        var count = await context.Posts.AsNoTracking().CountAsync();
        try
        {
            // post é uma lista de anonimo
            var posts = await context
                .Posts
                .AsNoTracking()
                .Include(x=> x.Category)
                .Include(x=> x.Author)
                .Select(x=> new ListPostsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = $"{x.Author.Name} ({x.Author.Email})"
                })// seleciono só o q interessa 
                .Skip(page*pageSize)            // ** paginação
                .Take(pageSize)
                .OrderByDescending(x=>x.LastUpdateDate)
                .ToListAsync();
                // ** ReferenceHandler.IgnoreCycles no program, evita q Post -chama category - q chama posts
                // tem q ser dinamico pq o objeto é anonimo
            return Ok(new ResultViewModel<dynamic>( new
                {
                    total = count,
                    page,
                    pageSize,
                    posts
                }));
        }catch
        {
            return StatusCode(500, new ResultViewModel<List<Post>> ("05x04 - Falha interna no servidor."));
        }
        
    }

    [HttpGet("v1/posts/{id:int}")]
    public async Task<IActionResult> DetailsAsync(
        [FromServices] BlogDataContext context,
        [FromRoute] int id )
        {
            try
            {
            System.Linq.Expressions.Expression<Func<Post, User>> navigationPropertyPath = x => x.Author;
            var post = await context.Posts      //DbSet<Post>
                    .AsNoTracking() // IQueryable                              
                    .Include(x =>x.Author)
                    .ThenInclude(x => x.Roles)
                    .Include( x => x.Category)                      
                    .FirstOrDefaultAsync( x=> x.Id == id);

                if (post == null)
                    return NotFound(new ResultViewModel <Post> ("Conteudo não encontrado."));

                return Ok (new ResultViewModel<Post>(post));

            }catch(Exception ex)
            {

                //return StatusCode(500, new ResultViewModel<Post>("05X04x - Falha interna no servidor."));
                return StatusCode(500, new ResultViewModel<Exception>(ex.Message));

            }          
                
            
        }

    [HttpGet("v1/posts/category/{category}")]
    public async Task<IActionResult> GetByCategoryAsync(
        [FromRoute] string category,
        [FromServices] BlogDataContext context,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 25 )
    {
        try
        {
            var count = await context.Posts.AsNoTracking().CountAsync();

            var posts = await context
                .Posts
                .AsNoTracking()
                .Include(x=> x.Author)
                .Include(x=> x.Category)
                .Where (x=> x.Category.Slug == category)
                .Select(x=> new ListPostsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Slug = x.Slug,
                    LastUpdateDate = x.LastUpdateDate,
                    Category = x.Category.Name,
                    Author = $"{x.Author.Name } ({x.Author.Email})"
                })
                .Skip(page*pageSize)
                .Take(pageSize)
                .OrderByDescending(x=> x.LastUpdateDate)
                .ToListAsync();
            return Ok(new ResultViewModel<dynamic> (new
                {
                    total = count,
                    page,
                    pageSize,
                    posts

                }));

        }catch
        {
            return StatusCode(500, new ResultViewModel<List<Post>> ("05x04a - Falha interna no servidor."));
        }

    }

}


