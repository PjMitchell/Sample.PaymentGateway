using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace PaymentGateway.Test
{
    public class BankFactoryTests
    {
        private readonly Dictionary<string, BankConfiguration> config;
        private readonly IBankFactory target;

        public BankFactoryTests()
        {
            config = new Dictionary<string, BankConfiguration>();
            var options = new Mock<IOptions<Dictionary<string, BankConfiguration>>>();
            options.Setup(s => s.Value).Returns(() => config);
            target = new BankFactory(options.Object);
        }

        [Fact]
        public void GetBankOrDefault_IfConfigNotFound_ReturnNull()
        {
            config.Add("ABank", new BankConfiguration());
            var result = target.GetBankOrDefault("NotABank");
            Assert.Null(result);
        }

        [Fact]
        public void GetBankOrDefault_CanBuildTestBank()
        {
            config.Add("ABank", new BankConfiguration { ConnectorType = BankConnectorType.Test });
            var result = target.GetBankOrDefault("ABank");
            Assert.NotNull(result);
            Assert.IsType<TestBank>(result);
        }
    }
}
