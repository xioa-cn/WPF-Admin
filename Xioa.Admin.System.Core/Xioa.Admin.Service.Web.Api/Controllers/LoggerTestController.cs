using Microsoft.AspNetCore.Mvc;

namespace Xioa.Admin.Service.Web.Api.Controllers;

[ApiController]
[Route("api/[controller]/[Action]")]
public class LoggerTestController {
    private readonly ILogger<LoggerTestController> _logger;

    public LoggerTestController(ILogger<LoggerTestController> logger) {
        _logger = logger;
    }

    [HttpGet]
    public string TestLogger() {
        _logger.LogInformation("TestLogger");
        int.Parse("-");
        return "TestLogger";
    }
}