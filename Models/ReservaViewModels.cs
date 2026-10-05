using GaragemVeiculos.Models;

namespace GaragemVeiculos.Models;

public class ReservaItemViewModel
{
    public int Id { get; set; }
    public int VeiculoId { get; set; }
    public int PessoaId { get; set; }
    public string NomePessoa { get; set; } = string.Empty;
    public string ModeloVeiculo { get; set; } = string.Empty;
    public string PlacaVeiculo { get; set; } = string.Empty;
    public DateTime DataInicio { get; set; }
    public DateTime DataFim { get; set; }
    public bool EstaAtivaHoje { get; set; }
}

public class VeiculoStatusViewModel
{
    public Veiculo Veiculo { get; set; } = new();
    public bool ReservadoHoje { get; set; }
    public string? ReservadoPara { get; set; }
}

public class ReservaIndexViewModel
{
    public List<ReservaItemViewModel> Reservas { get; set; } = new();
    public List<VeiculoStatusViewModel> VeiculosStatus { get; set; } = new();
}
