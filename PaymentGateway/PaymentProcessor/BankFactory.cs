using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace PaymentGateway
{
    public interface IBankFactory
    {
        IBank? GetBankOrDefault(string bankname);
    }

    public class BankFactory : IBankFactory
    {
        private readonly IOptions<Dictionary<string, BankConfiguration>> config;

        public BankFactory(IOptions<Dictionary<string, BankConfiguration>> config)
        {
            this.config = config;
        }

        public IBank? GetBankOrDefault(string bankname)
        {
            if (config.Value.TryGetValue(bankname, out var bankConfig))
                return GetBankOrDefault(bankConfig);
            return null;
        }

        private IBank? GetBankOrDefault(BankConfiguration bank)
        {
            return bank.ConnectorType switch
            {
                BankConnectorType.Test => new TestBank(),
                // Will add more banks here based on how you are connecting to them
                _ => null
            }; 
        }
    }
}
