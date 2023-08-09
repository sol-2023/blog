using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts;


public class RegisterViewModel
{
    [Required (ErrorMessage = "O nome é obrigatório.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage ="O email é obrigatório.")]
    [EmailAddress( ErrorMessage = "Email invalido.")]
    public string Email { get; set; } = string.Empty;

}
