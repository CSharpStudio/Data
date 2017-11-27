using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Css.DataPortal;
using Microsoft.AspNetCore.Mvc;

namespace Css.ServiceHost.Controllers
{
    [Route("api/[controller]")]
    public class DataController : Controller
    {
        // POST api/data
        [HttpPost]
        public ApiResponse Invoke([FromBody]ApiRequest value)
        {
            return new ApiResponse();
        }
    }
}
