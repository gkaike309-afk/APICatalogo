<<<<<<< HEAD
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("api/v{version:apiVersion}/teste")]
[ApiController]
[ApiVersion("1.0", Deprecated = true)]
[ApiExplorerSettings(IgnoreApi = true)]
public class TesteV1Controller : ControllerBase
{
    [HttpGet]
    public string GetVersion()
    {
        return "TsteV1 -- GET -- Api versão 1.0";
    }
=======
namespace APICatalogo.Controllers;

public class TesteV1Controller
{
    
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
}