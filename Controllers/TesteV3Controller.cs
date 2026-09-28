<<<<<<< HEAD
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace APICatalogo.Controllers;

[Route("api/teste")]
[ApiController]
[ApiVersion(3)]
[ApiVersion(4)]
[ApiExplorerSettings(IgnoreApi = true)]

public class TesteV3Controller : ControllerBase
{
    [MapToApiVersion(3)]
    [HttpGet]
    public string GetVersion3()
    {
        return "Version3 -- GET -- Api versão 3.0";
    }
    
    [MapToApiVersion(4)]
    [HttpGet]
    public string GetVersion4()
    {
        return "Version4 -- GET -- Api versão 4.0";
    }
=======
namespace APICatalogo.Controllers;

public class TesteV3Controller
{
    
>>>>>>> ebe7d541f79dfcdac52da27cc2b0c32df3d25af4
}