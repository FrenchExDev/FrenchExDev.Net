using FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Models;
using FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Controllers;

public class AgentsController : Controller
{
    private readonly IAgentRegistry _agentRegistry;
    private readonly IOrchestratorRegistry _orchestratorRegistry;
    private readonly ILogger<AgentsController> _logger;

    public AgentsController(
  IAgentRegistry agentRegistry,
   IOrchestratorRegistry orchestratorRegistry,
        ILogger<AgentsController> logger)
    {
    _agentRegistry = agentRegistry;
    _orchestratorRegistry = orchestratorRegistry;
      _logger = logger;
    }

    public IActionResult Index()
    {
    var agents = _agentRegistry.GetAll();
        return View(agents);
    }

    public IActionResult Details(string id)
    {
     var agents = _agentRegistry.GetAll();
    var agent = agents.FirstOrDefault(a => a.Id == id);
  
        if (agent == null)
        {
     return NotFound();
   }

      // Get orchestrator info
        var orchestrators = _orchestratorRegistry.GetAll();
   var orchestrator = orchestrators.FirstOrDefault(o => o.Id == agent.OrchestratorId);
        ViewBag.Orchestrator = orchestrator;
        
        return View(agent);
    }

    [HttpPost]
    public IActionResult Unregister(string id)
 {
        var success = _agentRegistry.Unregister(id);
        
        if (success)
      {
            TempData["SuccessMessage"] = $"Agent {id} unregistered successfully.";
        }
    else
        {
 TempData["ErrorMessage"] = $"Failed to unregister agent {id}.";
        }
        
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult UpdateStatus(string id, AgentStatus status)
    {
        _agentRegistry.UpdateStatus(id, status);
      TempData["SuccessMessage"] = $"Agent {id} status updated to {status}.";
        
      return RedirectToAction(nameof(Details), new { id });
    }
}
