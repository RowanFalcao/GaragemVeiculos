using System.ComponentModel.DataAnnotations;

namespace GaragemVeiculos.Models;

public class Veiculo
{
    public int Id { get; set; }

    [Required(ErrorMessage = "A placa é obrigatória.")]
    [RegularExpression(@"^[A-Z0-9-]{7,8}$", ErrorMessage = "Informe uma placa válida (ex: ABC1234 ou ABC1D23).")]
    public string Placa { get; set; } = string.Empty;

    [Required(ErrorMessage = "A marca é obrigatória.")]
    public string Marca { get; set; } = string.Empty;

    [Required(ErrorMessage = "O modelo é obrigatório.")]
    public string Modelo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O ano é obrigatório.")]
    [Range(1900, 2100, ErrorMessage = "Ano inválido.")]
    public int Ano { get; set; }

    public string? Cor { get; set; }
}
