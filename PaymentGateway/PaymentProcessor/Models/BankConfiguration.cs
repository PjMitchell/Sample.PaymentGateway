namespace PaymentGateway
{
    public record BankConfiguration
    {
        /// <summary>
        /// This will define what IBank implementation we are going to use
        /// I am assuming that there will be a couple of protocols we will need to implement, each with their own set of configs
        /// </summary>
        public BankConnectorType ConnectorType { get; init; }
        
        // We will probably add some connection info for the various connection protocols
    }
}
