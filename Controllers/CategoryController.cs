using Blog.Data;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Blog.ViewModels;
using Blog.ViewModels.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Blog.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : ControllerBase
    {
        [HttpGet("v1/categories")] // convenção de padronização, sempre minusculo e plural
                                   // url: localhost:PORTA/v1/categories refere-se a [Category]Controller
                                   // v1 = versão - previne quebra de app devido a manutenção
        //public async Task<IActionResult> GetAsync( 
        public IActionResult GetAsync( // Tasl/async/await : melhora performance
            [FromServices] IMemoryCache cache,
            [FromServices] BlogDataContext context)
        {
            try 
            {
                //var categories = await context.Categories.ToListAsync();// Lista vira task

                // pega ou cria memoria cache
                var categories = cache.GetOrCreate(key: "CategoriesCache", factory: entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return GetCategories(context);
                });

                return Ok(new ResultViewModel<List<Category>>(categories));

            }
            catch
            {
                return StatusCode(500, new ResultViewModel<List<Category>>("05X02 - Falha interna no servidor"));
            }
        }
        
        // usar a memoria cache em categories
        private List<Category> GetCategories(BlogDataContext context)
        {
            return context.Categories.ToList();

        }

        [HttpGet("v1/categories/{id:int}")]                           
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices] BlogDataContext context)
        {
           try
           {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                {
                    return NotFound(new ResultViewModel<Category>("Conteudo não encontrado"));
                }
                return Ok(new ResultViewModel<Category> (category));
           }           
           catch 
           {
                return StatusCode(500, new ResultViewModel<Category>("05X05 - Falha interna no servidor"));
           }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid) // como tem diretiva de suprimnir validação automatica, este codigo
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors())); // se torna obrigatorio.
                

            try
            {
                var category = new Category
                { 
                    Id=0,
                    Name=model.Name,
                    Slug=model.Slug.ToLower(),
                    
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", category);
            }
            catch (DbUpdateException)// pode ter varios catchs
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE9 - Não foi possivel incluir a categoria "));
            }
            catch 
            {
                return StatusCode(500,  new ResultViewModel<Category>("05X10 - Falha interna no servidor"));
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel model,
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context
                .Categories
                .FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>("Conteudo não encontrado"));

                category.Name = model.Name;
                category.Slug = model.Slug.ToLower();

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ) // pode ter varios catchs
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE8 - Não foi possivel atualizar a categoria "));
            }
            catch 
            {
                return StatusCode(500, new ResultViewModel<Category>("05X11 - Falha interna no servidor"));
            }
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,           
            [FromServices] BlogDataContext context)
        {
            try
            {
                var category = await context
                .Categories
                .FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>("Conteudo não encontrado"));

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ) // pode ter varios catchs
            {
                return StatusCode(500, new ResultViewModel<Category>("05XE7 - Não foi possivel excluir a categoria "));
            }
            catch 
            {
                return StatusCode(500,new ResultViewModel<Category>( "05X12 - Falha interna no servidor"));
            }
        }

    }
}

