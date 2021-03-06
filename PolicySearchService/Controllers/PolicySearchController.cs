﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PolicySearchService.Api.Queries;

namespace PolicySearchService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PolicySearchController : ControllerBase
    {
        private readonly IMediator bus;
        private ILogger<PolicySearchController> _logger;

        public PolicySearchController(IMediator bus, ILogger<PolicySearchController> logger)
        {
            this.bus = bus;
            _logger = logger;
        }


        // GET api/values
        [HttpGet()]
        public async Task<ActionResult> SearchAsync([FromQuery] string q)
        {
            _logger.LogInformation($"@@@@@@@@@@@@@@@@@@@@@@@ Search Async {q}");
            
            var result = await bus.Send(new FindPolicyQuery { QueryText = q });
            return new JsonResult(result);
        }
    }
}
