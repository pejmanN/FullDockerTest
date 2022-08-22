using CusomerManagement.Persistance;
using FullDockerTest.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace CusomerManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {


        private readonly ILogger<WeatherForecastController> _logger;
        private readonly AppDbContext _db;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, AppDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public string Get()
        {
            var t = new Uri($"queue:{KebabCaseEndpointNameFormatter.Instance.Consumer<SagaCustomerValidateConsumer>()}");

            try
            {
                var customer = _db.Customers.FirstOrDefault();
                _logger.LogInformation("customer with  name {Name}", customer.Name);

                _logger.LogInformation("request for customers list is recived");
                var customers = _db.Customers.ToList();
                _logger.LogInformation("list of all cusomer is sent");

                return customer.Family;
                //return customers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error occurd");
                return ex.StackTrace.FirstOrDefault().ToString() + ",,,,," + ex.Message;
            }
        }

      
    }
}