using Microsoft.AspNetCore.Mvc;
using GaragemVeiculos.Models;
using GaragemVeiculos.Repositories;

namespace GaragemVeiculos.Controllers;

public class PessoaController : Controller
{
    private readonly IPessoaRepository _pessoaRepository;

    public PessoaController(IPessoaRepository pessoaRepository)
    {
        _pessoaRepository = pessoaRepository;
    }

    public IActionResult Index()
    {
        var pessoas = _pessoaRepository.ObterTodas();
        return View(pessoas);
    }

    [HttpGet]
    public IActionResult Create() => View("PessoaForm", new Pessoa());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Pessoa pessoa)
    {
        if (!ModelState.IsValid)
            return View("PessoaForm", pessoa);

        try
        {
            _pessoaRepository.Adicionar(pessoa);
            TempData["Sucesso"] = "Pessoa cadastrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("CPF", ex.Message);
            return View("PessoaForm", pessoa);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("PessoaForm", pessoa);
        }
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var pessoa = _pessoaRepository.ObterPorId(id);
        if (pessoa == null)
            return NotFound();

        return View("PessoaForm", pessoa);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Pessoa pessoa)
    {
        if (!ModelState.IsValid)
            return View("PessoaForm", pessoa);

        try
        {
            _pessoaRepository.Atualizar(pessoa);
            TempData["Sucesso"] = "Pessoa atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("CPF", ex.Message);
            return View("PessoaForm", pessoa);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View("PessoaForm", pessoa);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        try
        {
            _pessoaRepository.Remover(id);
            TempData["Sucesso"] = "Pessoa excluída com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Erro"] = "Erro ao remover pessoa: " + ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
