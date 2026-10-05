using GaragemVeiculos.Models;

namespace GaragemVeiculos.Repositories;

public interface IReservaRepository
{
    List<Reserva> ObterTodas();
    Reserva? ObterPorId(int id);
    void Adicionar(Reserva reserva);
    void Atualizar(Reserva reserva);
    void Remover(int id);
    bool ExisteConflito(int veiculoId, DateTime dataInicio, DateTime dataFim, int? reservaIdIgnorar = null);
}
