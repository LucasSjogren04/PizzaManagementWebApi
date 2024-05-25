using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tomaso_Pizza.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    { 
        [HttpGet]
        public async Task<bool> Register()
        {

        }
        [HttpGet]
        public async Task Login()
        {

        }
    }
}
