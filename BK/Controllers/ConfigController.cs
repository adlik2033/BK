using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ConfigController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public ActionResult Get()
        {
            var connstr = _config["ConnectionStrings:PostgreConnection"];
            var appName = _config["AppSettings:AppName"];
            var key = _config["AppSettings:APIKey"];
            return Ok(new { connstr, appName, key });
        }
    }
}
