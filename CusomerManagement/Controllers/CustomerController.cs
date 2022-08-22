using CusomerManagement.Models;
using CusomerManagement.Persistance;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared.Saga.Models;

namespace CusomerManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _publishEndpoint;
        public CustomerController(AppDbContext db, IPublishEndpoint publishEndpoint)
        {
            _db = db;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost("disableCustomer")]
        public ActionResult DisableCustomer([FromBody] DisableCustomerVm disableCustomerVm)
        {
            var customer = _db.Customers.FirstOrDefault(x => x.Id == disableCustomerVm.CusomerId);
            if (customer == null)
                throw new InvalidDataException();
            customer.IsActive = false;
            _db.SaveChanges();

            _publishEndpoint.Publish<ICustomerDeactivated>(new
            {
                CustomerId = disableCustomerVm.CusomerId,
            });

            return Ok();
        }
    }
}
