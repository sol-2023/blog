using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts;

public class LoginViewModel
{
    

    [Required(ErrorMessage ="O email é obrigatório.")]
    [EmailAddress( ErrorMessage = "Email invalido.")]
    public string Email { get; set; } = string.Empty;

    [Required (ErrorMessage = "Informe a senha.")]
    public string Password { get; set; } = string.Empty;

    
}