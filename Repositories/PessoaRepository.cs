using System.Text.Json;
using GaragemVeiculos.Models;

namespace GaragemVeiculos.Repositories;

public class PessoaRepository : IPessoaRepository
{
    private readonly string _filePath;

    public PessoaRepository(IWebHostEnvironment environment)
    {
        var dataFolder = Path.Combine(environment.ContentRootPath, "Data");
        if (!Directory.Exists(dataFolder))
        {
            Directory.CreateDirectory(dataFolder);
        }

        _filePath = Path.Combine(dataFolder, "pessoas.json");
        GarantirArquivo();
    }

    private void GarantirArquivo()
    {
        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    private List<Pessoa> LerArquivo()
    {
        GarantirArquivo();
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<Pessoa>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Pessoa>();
    }

    private void GravarArquivo(List<Pessoa> pessoas)
    {
        var json = JsonSerializer.Serialize(pessoas, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_filePath, json);
    }

    public List<Pessoa> ObterTodas() => LerArquivo();

    public Pessoa? ObterPorId(int id) => LerArquivo().FirstOrDefault(p => p.Id == id);

    public void Adicionar(Pessoa pessoa)
    {
        var pessoas = LerArquivo();

        var cpfLimpo = pessoa.CPF.Trim().Replace(".", "").Replace("-", "");
        if (pessoas.Any(p => p.CPF.Trim().Replace(".", "").Replace("-", "") == cpfLimpo))
            throw new InvalidOperationException("CPF já cadastrado.");

        pessoa.Id = pessoas.Any() ? pessoas.Max(p => p.Id) + 1 : 1;
        pessoas.Add(pessoa);
        GravarArquivo(pessoas);
    }

    public void Atualizar(Pessoa pessoa)
    {
        var pessoas = LerArquivo();
        var existente = pessoas.FirstOrDefault(p => p.Id == pessoa.Id);
        if (existente == null)
            throw new KeyNotFoundException("Pessoa não encontrada.");

        var cpfLimpo = pessoa.CPF.Trim().Replace(".", "").Replace("-", "");
        if (pessoas.Any(p => p.Id != pessoa.Id && p.CPF.Trim().Replace(".", "").Replace("-", "") == cpfLimpo))
            throw new InvalidOperationException("CPF já cadastrado para outra pessoa.");

        existente.Nome = pessoa.Nome;
        existente.CPF = pessoa.CPF;
        existente.Email = pessoa.Email;
        existente.Telefone = pessoa.Telefone;

        GravarArquivo(pessoas);
    }

    public void Remover(int id)
    {
        var pessoas = LerArquivo();
        var pessoa = pessoas.FirstOrDefault(p => p.Id == id);
        if (pessoa != null)
        {
            pessoas.Remove(pessoa);
            GravarArquivo(pessoas);
        }
    }
}
