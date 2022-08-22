namespace Shared
{
    public class BusConstants
    {
        public const string RabbitMqUri = "rabbitmq://localhost/";
        public const string Username = "guest";
        public const string Password = "guest";
        public const string OrderQueue = "validate-order-queue";
        public const string SagaBusQueue = "saga-bus-queue";
        public const string StartOrderTranastionQueue = "start-order";
        public const string ValidateOrderTranastionQueue = "validate-order";

        public const string SagaStartOrderTranastionQueue = "saga-start-order";
        public const string SagaCusomerValidateQueue = "saga-customer-validate";
    }
}
