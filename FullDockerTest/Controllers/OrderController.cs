using FullDockerTest.Models;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.Saga;
using Shared.Saga.Models;

namespace FullDockerTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint publishEndpoint;
        private readonly IRequestClient<IGetInfoRequest> _getInfoClient;
        private readonly IRequestClient<ICheckOrderStatus> _orderStatusClient;
        public OrderController(ISendEndpointProvider sendEndpointProvider,
            IPublishEndpoint publishEndpoint,
            IRequestClient<IGetInfoRequest> getInfoClient,
             IRequestClient<ICheckOrderStatus> orderStatusClient)
        {
            this.publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
            _getInfoClient = getInfoClient;
            _orderStatusClient = orderStatusClient;
        }


        [HttpPost]
        [Route("createorder")]
        public async Task<IActionResult> CreateOrderU([FromBody] OrderModel orderModel)
        {
            orderModel.OrderId = Guid.NewGuid();
            var endpoint = await _sendEndpointProvider
                .GetSendEndpoint(new Uri("queue:" + BusConstants.StartOrderTranastionQueue));


            await endpoint.Send<IStartOrder>(new
            {
                OrderId = orderModel.OrderId,
                PaymentCardNumber = orderModel.CardNumber,
                ProductName = orderModel.ProductName
            });

            return Ok("Success");
        }


        [HttpPost]
        [Route("createOrderSaga")]
        public async Task<IActionResult> CreateOrderU([FromBody] OrderSagaVm orderModel)
        {
            var endpoint = await _sendEndpointProvider
               .GetSendEndpoint(new Uri("queue:" + BusConstants.SagaStartOrderTranastionQueue));


            await endpoint.Send<ISagaStartOrder>(new
            {
                OrderId = Guid.NewGuid(),
                PaymentCardNumber = orderModel.CardNumber,
                ProductName = orderModel.ProductName
            });

            return Ok("Success");
        }

        [HttpPost]
        [Route("test")]
        public string Test()
        {
            return "testingo";
        }



        [HttpGet("orderStatus/{id}")]
        public async Task GetOrderStatus(Guid id)
        {
            var result = await _orderStatusClient.GetResponse<IOrderStatus, IOrderNotFound>(new
            {
                CorrelationId = id,
            });

            var t = "";
        }

        [HttpGet("requestResponse")]
        public async Task RequestResponse()
        {
            var t = await _getInfoClient
                .GetResponse<GetInfoAccepted, GetInfoReject>(new
                {
                    Name = "pejman"
                });
            var s = "";
            //if (accepted.IsCompletedSuccessfully)
            //{
            //    var response = await accepted;

            //}

            //if (accepted.IsCompleted)
            //{
            //    var response = await accepted;

            //}
            //else
            //{
            //    var response = await rejected;

            //}
        }
    }
}