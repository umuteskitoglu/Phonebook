using Application.Features.Contacts.Commands.AddContactInformation;
using Application.Features.Contacts.Commands.CreateContact;
using Application.Features.Contacts.Commands.DeleteContact;
using Application.Features.Contacts.Commands.DeleteContactInformation;
using Application.Features.Contacts.Queries.GetContactDetails;
using Application.Features.Contacts.Queries.GetContacts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContactServiceAPI.Controllers
{
    /// <summary>
    /// Contacts Controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ContactsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ContactsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get all contacts
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            var result = await _mediator.Send(new GetContactsQuery());
            return Ok(result);
        }
        /// <summary>
        /// Get a contact by id
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetContactById(GetContactDetailsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        /// <summary>
        /// Create a contact
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateContact(CreateContactCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        /// <summary>
        /// Delete a contact
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteContact(DeleteContactCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        /// <summary>
        /// Add a contact information
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddContactInformation(AddContactInformationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        /// <summary>
        /// Delete a contact information
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteContactInformation(DeleteContactInformationCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
} 