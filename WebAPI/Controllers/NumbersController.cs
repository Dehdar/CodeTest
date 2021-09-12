using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using QueryService.Queries;
using Serilog;
using WebAPI.Models.Requests;

namespace WebAPI.Controllers
{
    [ApiController, Route("numbers")]
    public class NumbersController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILogger _logger;
        
        public NumbersController(IMediator mediator, ILogger logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _mediator.Send(new AllNumbersQuery()));
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var response = await _mediator.Send(new SingleNumbersQuery { Id = id });

            if (response == default)
                return NoContent();
            
            return Ok(response);
        }

        [HttpPost]
        public IActionResult Create([FromBody] SortNumbersRequest request)
        {
            var command = request.ToCommand();
            _logger.Information("Command {@command} is being published", command);
            _mediator.Publish(command);
            
            _logger.Information("Command {@command} published", command);
            return Accepted(command.Id);
        }
    }
}