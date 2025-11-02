using FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Models;
using FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Controllers;

public class OrchestratorsController : Controller
{
    private readonly IOrchestratorRegistry _orchestratorRegistry;
    private readonly IAgentRegistry _agentRegistry;
    private readonly ILogger<OrchestratorsController> _logger;

  public OrchestratorsController(
   IOrchestratorRegistry orchestratorRegistry,
     IAgentRegistry agentRegistry,
        ILogger<OrchestratorsController> logger)
    {
        _orchestratorRegistry = orchestratorRegistry;
        _agentRegistry = agentRegistry;
        _logger = logger;
    }

  public IActionResult Index()
    {
    var orchestrators = _orchestratorRegistry.GetAll();
        return View(orchestrators);
    }

    public IActionResult Details(string id)
 {
        var orchestrators = _orchestratorRegistry.GetAll();
        var orchestrator = orchestrators.FirstOrDefault(o => o.Id == id);
        
    if (orchestrator == null)
      {
    return NotFound();
        }

        var agents = _agentRegistry.GetByOrchestrator(id);
        ViewBag.Agents = agents;
        
        return View(orchestrator);
    }

  [HttpPost]
    public IActionResult Unregister(string id)
    {
        var success = _orchestratorRegistry.Unregister(id);
        
     if (success)
        {
            TempData["SuccessMessage"] = $"Orchestrator {id} unregistered successfully.";
        }
        else
        {
     TempData["ErrorMessage"] = $"Failed to unregister orchestrator {id}.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult UpdateStatus(string id, OrchestratorStatus status)
    {
        _orchestratorRegistry.UpdateStatus(id, status);
        TempData["SuccessMessage"] = $"Orchestrator {id} status updated to {status}.";
        
        return RedirectToAction(nameof(Details), new { id });
    }
}
