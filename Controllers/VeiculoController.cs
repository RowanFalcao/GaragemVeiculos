using Microsoft.AspNetCore.Mvc;
using GaragemVeiculos.Models;
using GaragemVeiculos.Repositories;

namespace GaragemVeiculos.Controllers;

public class VeiculoController : Controller
{
    private readonly IVeiculoRepository _veiculoRepository;

    public VeiculoController(IVeiculoRepository veiculoRepository)
    {
        _veiculoRepository = veiculoRepository;
    }

    public IActionResult Index()
    {
        var veiculos = _veiculoRepository.ObterTodos();
        return View(veiculos);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("VeiculoForm", new Veiculo { Ano = DateTime.Now.Year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Veiculo veiculo)
    {
        if (!ModelState.IsValid)
            return View("VeiculoForm", veiculo);

        try
        {
            _veiculoRepository.Adicionar(veiculo);
            TempData["Sucesso"] = "Veículo cadastrado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("Placa", ex.Message);
            return View("VeiculoForm", veiculo);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("VeiculoForm", veiculo);
        }
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var veiculo = _veiculoRepository.ObterPorId(id);
        if (veiculo == null)
            return NotFound();

        return View("VeiculoForm", veiculo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Veiculo veiculo)
    {
        if (!ModelState.IsValid)
            return View("VeiculoForm", veiculo);

        try
        {
            _veiculoRepository.Atualizar(veiculo);
            TempData["Sucesso"] = "Veículo atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("Placa", ex.Message);
            return View("VeiculoForm", veiculo);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("VeiculoForm", veiculo);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        try
        {
            _veiculoRepository.Remover(id);
            TempData["Sucesso"] = "Veículo excluído com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Erro"] = "Erro ao remover veículo: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
