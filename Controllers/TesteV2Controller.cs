<<<<<<< HEAD
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("api/v{version:apiVersion}/teste")]
[ApiController]
[ApiVersion("2.0")]
[ApiExplorerSettings(IgnoreApi = true)]
public class TesteV2Controller : ControllerBase
{
    [HttpGet]
    public string GetVersion()
    {
        return "TesteV2 -- GET -- Api versão 2.0";
    }
=======
namespace APICatalogo.Controllers;

public class TesteV2Controller
{
    
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
}