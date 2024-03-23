using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Villa_API.Models;

namespace Villa_API.Controllers.v2
{
    [Route("api/v{version:apiVersion}/VillaNumberAPI")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaNumberAPIController : ControllerBase
    {
        protected APIResponse _response;

        public VillaNumberAPIController()
        {
            this._response = new(); 
        }

        // You can add API v2 endpoints here ...

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
