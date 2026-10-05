using System.Text.Json;
using GaragemVeiculos.Models;

namespace GaragemVeiculos.Repositories;

public class VeiculoRepository : IVeiculoRepository
{
    private readonly string _filePath;

    public VeiculoRepository(IWebHostEnvironment environment)
    {
        var dataFolder = Path.Combine(environment.ContentRootPath, "Data");
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        _filePath = Path.Combine(dataFolder, "veiculos.json");
        GarantirArquivo();
    }

    private void GarantirArquivo()
    {
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    private List<Veiculo> LerArquivo()
    {
        GarantirArquivo();
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Veiculo>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Veiculo>();
    }

    private void GravarArquivo(List<Veiculo> veiculos)
    {
        var json = JsonSerializer.Serialize(veiculos, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public List<Veiculo> ObterTodos() => LerArquivo();

    public Veiculo? ObterPorId(int id) => LerArquivo().FirstOrDefault(v => v.Id == id);

    public void Adicionar(Veiculo veiculo)
    {
        var veiculos = LerArquivo();
        var placaLimpa = veiculo.Placa.Trim().ToUpper().Replace("-", "");

        if (veiculos.Any(v => v.Placa.Trim().ToUpper().Replace("-", "") == placaLimpa))
            throw new InvalidOperationException("Placa já cadastrada para outro veículo.");

        veiculo.Placa = veiculo.Placa.Trim().ToUpper();
        veiculo.Id = veiculos.Any() ? veiculos.Max(v => v.Id) + 1 : 1;
        veiculos.Add(veiculo);
        GravarArquivo(veiculos);
    }

    public void Atualizar(Veiculo veiculo)
    {
        var veiculos = LerArquivo();
        var existente = veiculos.FirstOrDefault(v => v.Id == veiculo.Id);
        if (existente == null)
            throw new KeyNotFoundException("Veículo não encontrado.");

        var placaLimpa = veiculo.Placa.Trim().ToUpper().Replace("-", "");
        if (veiculos.Any(v => v.Id != veiculo.Id && v.Placa.Trim().ToUpper().Replace("-", "") == placaLimpa))
            throw new InvalidOperationException("Placa já cadastrada para outro veículo.");

        existente.Placa = veiculo.Placa.Trim().ToUpper();
        existente.Marca = veiculo.Marca;
        existente.Modelo = veiculo.Modelo;
        existente.Ano = veiculo.Ano;
        existente.Cor = veiculo.Cor;

        GravarArquivo(veiculos);
    }

    public void Remover(int id)
    {
        var veiculos = LerArquivo();
        var veiculo = veiculos.FirstOrDefault(v => v.Id == id);
        if (veiculo != null)
        {
            veiculos.Remove(veiculo);
            GravarArquivo(veiculos);
        }
    }
}
