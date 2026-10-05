using System.Text.Json;
using GaragemVeiculos.Models;

namespace GaragemVeiculos.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly string _filePath;

    public ReservaRepository(IWebHostEnvironment environment)
    {
        var dataFolder = Path.Combine(environment.ContentRootPath, "Data");
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        _filePath = Path.Combine(dataFolder, "reservas.json");
        GarantirArquivo();
    }

    private void GarantirArquivo()
    {
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    private List<Reserva> LerArquivo()
    {
        GarantirArquivo();
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Reserva>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Reserva>();
    }

    private void GravarArquivo(List<Reserva> reservas)
    {
        var json = JsonSerializer.Serialize(reservas, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public List<Reserva> ObterTodas() => LerArquivo();

    public Reserva? ObterPorId(int id) => LerArquivo().FirstOrDefault(r => r.Id == id);

    public bool ExisteConflito(int veiculoId, DateTime dataInicio, DateTime dataFim, int? reservaIdIgnorar = null)
    {
        var reservas = LerArquivo();
        var inicio = dataInicio.Date;
        var fim = dataFim.Date;

        return reservas.Any(r =>
            r.VeiculoId == veiculoId &&
            (!reservaIdIgnorar.HasValue || r.Id != reservaIdIgnorar.Value) &&
            inicio <= r.DataFim.Date && fim >= r.DataInicio.Date
        );
    }

    public void Adicionar(Reserva reserva)
    {
        if (reserva.DataFim.Date < reserva.DataInicio.Date)
            throw new InvalidOperationException("A data final não pode ser anterior à data inicial.");

        if (ExisteConflito(reserva.VeiculoId, reserva.DataInicio, reserva.DataFim))
            throw new InvalidOperationException("Conflito de período: o veículo selecionado já possui reserva ativa neste período.");

        var reservas = LerArquivo();
        reserva.Id = reservas.Any() ? reservas.Max(r => r.Id) + 1 : 1;
        reservas.Add(reserva);
        GravarArquivo(reservas);
    }

    public void Atualizar(Reserva reserva)
    {
        var reservas = LerArquivo();
        var existente = reservas.FirstOrDefault(r => r.Id == reserva.Id);
        if (existente == null)
            throw new KeyNotFoundException("Reserva não encontrada.");

        if (reserva.DataFim.Date < reserva.DataInicio.Date)
            throw new InvalidOperationException("A data final não pode ser anterior à data inicial.");

        if (ExisteConflito(reserva.VeiculoId, reserva.DataInicio, reserva.DataFim, reserva.Id))
            throw new InvalidOperationException("Conflito de período: o veículo selecionado já possui reserva ativa neste período.");

        existente.VeiculoId = reserva.VeiculoId;
        existente.PessoaId = reserva.PessoaId;
        existente.DataInicio = reserva.DataInicio;
        existente.DataFim = reserva.DataFim;

        GravarArquivo(reservas);
    }

    public void Remover(int id)
    {
        var reservas = LerArquivo();
        var reserva = reservas.FirstOrDefault(r => r.Id == id);
        if (reserva != null)
        {
            reservas.Remove(reserva);
            GravarArquivo(reservas);
        }
    }
}
