using System.ComponentModel.DataAnnotations;

namespace GaragemVeiculos.Models;

public class Reserva
{
    public int Id { get; set; }

    [Required(ErrorMessage = "O veículo é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione um veículo válido.")]
    public int VeiculoId { get; set; }

    [Required(ErrorMessage = "A pessoa é obrigatória.")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecione uma pessoa válida.")]
    public int PessoaId { get; set; }

    [Required(ErrorMessage = "A data de início é obrigatória.")]
    [DataType(DataType.Date)]
    public DateTime DataInicio { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "A data de término é obrigatória.")]
    [DataType(DataType.Date)]
    public DateTime DataFim { get; set; } = DateTime.Today.AddDays(1);
}
