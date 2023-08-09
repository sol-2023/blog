using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Categories
{
    // mesmo modelo para criar e atualizar. portanto deixa de ser create passa
    // a ser Editor
    public class EditorCategoryViewModel
    {   // s� preciso do name e slug p/ criar categoria.N�o precisa id, nem do post

        [Required(ErrorMessage = "O nome é obrigatório")] // valida��o
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Este campo deve conter entre 3 e 40 caracteres.")]

        public string Name { get; set; }  = string.Empty;
        [Required(ErrorMessage = "O slug é obrigatório")]
        public string Slug { get; set; }  = string.Empty;

    }

}


