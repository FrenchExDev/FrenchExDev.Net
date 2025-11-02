using FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrenchExDev.Net.CSharp.ProjectDependency3.Viz.Api.Controllers;

public class HomeController : Controller
{
    private readonly IOrchestratorRegistry _orchestratorRegistry;
    private readonly IAgentRegistry _agentRegistry;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IOrchestratorRegistry orchestratorRegistry,
   IAgentRegistry agentRegistry,
        ILogger<HomeController> logger)
    {
        _orchestratorRegistry = orchestratorRegistry;
        _agentRegistry = agentRegistry;
        _logger = logger;
    }

    public IActionResult Index()
    {
        var orchestrators = _orchestratorRegistry.GetAll();
        var agents = _agentRegistry.GetAll();

        ViewBag.OrchestratorCount = orchestrators.Count();
        ViewBag.AgentCount = agents.Count();

        return View();
    }
}
