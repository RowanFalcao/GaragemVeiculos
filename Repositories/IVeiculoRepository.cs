using GaragemVeiculos.Models;

namespace GaragemVeiculos.Repositories;

public interface IVeiculoRepository
{
    List<Veiculo> ObterTodos();
    Veiculo? ObterPorId(int id);
    void Adicionar(Veiculo veiculo);
    void Atualizar(Veiculo veiculo);
    void Remover(int id);
}
