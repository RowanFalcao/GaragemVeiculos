using System.ComponentModel.DataAnnotations;

namespace GaragemVeiculos.Models;

public class Pessoa
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [RegularExpression(@"^\d{3}\.?\d{3}\.?\d{3}\-?\d{2}$", ErrorMessage = "Informe um CPF válido.")]
    public string CPF { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Informe um telefone válido.")]
    public string? Telefone { get; set; }
}
