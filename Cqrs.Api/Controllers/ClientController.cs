using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Cqrs.Common.Api;
using Cqrs.Common.Exceptions;
using Cqrs.Common.Models;
using Cqrs.Api.Models;
using Cqrs.Application.Clients;
using Cqrs.Domain.Models;

namespace Cqrs.Api.Controllers
{
    [Route("api/clients")]
    public class ClientController : ApiControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ClientController> _logger;

        public ClientController(IMapper mapper,
            IConfiguration configuration,
            ILogger<ClientController> logger)
            : base()
        {
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetClients()
        {
            var clientList = await Mediator.Send(new GetClientListQuery());
            var clientDtoList = _mapper.Map<IEnumerable<ClientListItemDto>>(clientList);

            return Ok(clientDtoList);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient(Guid id)
        {
            var client = await Mediator.Send(new GetClientQuery(id));
            var clientDto = _mapper.Map<ClientDto>(client);

            return Ok(clientDto);
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateClient([FromBody] ClientFormDto clientDto)
        {
            var client = _mapper.Map<Client>(clientDto);

            var command = new CreateClientCommand(client);

            var validationResult = command.Validate();
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var id = await Mediator.Send(command);

            return Ok(new CreateActionResponse { Id = id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(Guid id, [FromBody] ClientFormDto clientDto)
        {
            var client = _mapper.Map<Client>(clientDto);
            var command = new UpdateClientCommand(id, client);

            var validationResult = command.Validate();
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await Mediator.Send(command);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var command = new DeleteClientCommand(id);

            var validationResult = command.Validate();
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await Mediator.Send(command);

            return Ok();
        }

    }
}
