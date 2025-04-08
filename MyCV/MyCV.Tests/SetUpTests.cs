using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using Moq;

namespace MyCV.Tests
{
    internal class SetUpTests
    {
        [Test]
        public void NUnitInstalled()
        {
            Assert.Pass();
        }

        [Test]
        public void MicrosoftExtensionsLoggingInstalled()
        {
            var mockLogger = new Mock<ILogger<MockedLoggingService>>();
            var service = new MockedLoggingService(mockLogger.Object);
            service.UseLog();
            Assert.Pass();
        }
    }

    public class MockedLoggingService
    {
        private readonly ILogger<MockedLoggingService> _logger;

        public MockedLoggingService(ILogger<MockedLoggingService> logger)
        {
            _logger = logger;
        }

        public void UseLog()
        {
            _logger.LogInformation("UseLog() executed");
        }

    }
}
