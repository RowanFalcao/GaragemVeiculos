using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GaragemVeiculos.Models;
using GaragemVeiculos.Repositories;

namespace GaragemVeiculos.Controllers;

public class ReservaController : Controller
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IVeiculoRepository _veiculoRepository;
    private readonly IPessoaRepository _pessoaRepository;

    public ReservaController(
        IReservaRepository reservaRepository,
        IVeiculoRepository veiculoRepository,
        IPessoaRepository pessoaRepository)
    {
        _reservaRepository = reservaRepository;
        _veiculoRepository = veiculoRepository;
        _pessoaRepository = pessoaRepository;
    }

    public IActionResult Index()
    {
        var reservas = _reservaRepository.ObterTodas();
        var veiculos = _veiculoRepository.ObterTodos();
        var pessoas = _pessoaRepository.ObterTodas();

        var hoje = DateTime.Today;

        var items = reservas.Select(r =>
        {
            var p = pessoas.FirstOrDefault(x => x.Id == r.PessoaId);
            var v = veiculos.FirstOrDefault(x => x.Id == r.VeiculoId);
            var ativaHoje = hoje >= r.DataInicio.Date && hoje <= r.DataFim.Date;

            return new ReservaItemViewModel
            {
                Id = r.Id,
                VeiculoId = r.VeiculoId,
                PessoaId = r.PessoaId,
                NomePessoa = p != null ? p.Nome : $"Pessoa #{r.PessoaId} (não encontrada)",
                ModeloVeiculo = v != null ? $"{v.Marca} {v.Modelo}" : $"Veículo #{r.VeiculoId}",
                PlacaVeiculo = v != null ? v.Placa : "N/D",
                DataInicio = r.DataInicio,
                DataFim = r.DataFim,
                EstaAtivaHoje = ativaHoje
            };
        }).OrderByDescending(r => r.DataInicio).ToList();

        var statusVeiculos = veiculos.Select(v =>
        {
            var reservaHoje = reservas.FirstOrDefault(r =>
                r.VeiculoId == v.Id &&
                hoje >= r.DataInicio.Date &&
                hoje <= r.DataFim.Date
            );

            var pessoaNome = reservaHoje != null
                ? pessoas.FirstOrDefault(p => p.Id == reservaHoje.PessoaId)?.Nome
                : null;

            return new VeiculoStatusViewModel
            {
                Veiculo = v,
                ReservadoHoje = reservaHoje != null,
                ReservadoPara = pessoaNome
            };
        }).ToList();

        var vm = new ReservaIndexViewModel
        {
            Reservas = items,
            VeiculosStatus = statusVeiculos
        };

        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        CarregarViewBags();
        var model = new Reserva
        {
            DataInicio = DateTime.Today,
            DataFim = DateTime.Today.AddDays(1)
        };
        return View("ReservaForm", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Reserva reserva)
    {
        ValidarPessoaEVeiculo(reserva);

        if (!ModelState.IsValid)
        {
            CarregarViewBags(reserva.VeiculoId, reserva.PessoaId);
            return View("ReservaForm", reserva);
        }

        try
        {
            _reservaRepository.Adicionar(reserva);
            TempData["Sucesso"] = "Reserva criada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            CarregarViewBags(reserva.VeiculoId, reserva.PessoaId);
            return View("ReservaForm", reserva);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Erro ao salvar reserva: " + ex.Message);
            CarregarViewBags(reserva.VeiculoId, reserva.PessoaId);
            return View("ReservaForm", reserva);
        }
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var reserva = _reservaRepository.ObterPorId(id);
        if (reserva == null)
            return NotFound();

        CarregarViewBags(reserva.VeiculoId, reserva.PessoaId);
        return View("ReservaForm", reserva);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Reserva reserva)
    {
        ValidarPessoaEVeiculo(reserva);

        if (!ModelState.IsValid)
        {
            CarregarViewBags(reserva.VeiculoId, reserva.PessoaId);
            return View("ReservaForm", reserva);
        }

        try
        {
            _reservaRepository.Atualizar(reserva);
            TempData["Sucesso"] = "Reserva atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            CarregarViewBags(reserva.VeiculoId, reserva.PessoaId);
            return View("ReservaForm", reserva);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, "Erro ao atualizar reserva: " + ex.Message);
            CarregarViewBags(reserva.VeiculoId, reserva.PessoaId);
            return View("ReservaForm", reserva);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        try
        {
            _reservaRepository.Remover(id);
            TempData["Sucesso"] = "Reserva excluída com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Erro"] = "Erro ao remover reserva: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private void ValidarPessoaEVeiculo(Reserva reserva)
    {
        if (reserva.DataFim.Date < reserva.DataInicio.Date)
        {
            ModelState.AddModelError("DataFim", "A data de término não pode ser anterior à data de início.");
        }

        var pessoa = _pessoaRepository.ObterPorId(reserva.PessoaId);
        if (pessoa == null)
        {
            ModelState.AddModelError("PessoaId", "A pessoa selecionada não existe no cadastro.");
        }

        var veiculo = _veiculoRepository.ObterPorId(reserva.VeiculoId);
        if (veiculo == null)
        {
            ModelState.AddModelError("VeiculoId", "O veículo selecionado não existe no cadastro.");
        }
    }

    private void CarregarViewBags(int? veiculoSelecionado = null, int? pessoaSelecionada = null)
    {
        var veiculos = _veiculoRepository.ObterTodos()
            .Select(v => new SelectListItem
            {
                Value = v.Id.ToString(),
                Text = $"{v.Marca} {v.Modelo} ({v.Placa})",
                Selected = veiculoSelecionado.HasValue && veiculoSelecionado.Value == v.Id
            }).ToList();

        var pessoas = _pessoaRepository.ObterTodas()
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Nome} - CPF: {p.CPF}",
                Selected = pessoaSelecionada.HasValue && pessoaSelecionada.Value == p.Id
            }).ToList();

        ViewBag.Veiculos = veiculos;
        ViewBag.Pessoas = pessoas;
    }
}
