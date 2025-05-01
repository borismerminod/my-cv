using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCV.Server.Controllers;

namespace MyCV.Tests
{
    class RegistrationControllerTest
    {
        private RegistrationController _controller;

        [SetUp]
        public void Setup()
        {
            _controller = new RegistrationController();
        }

        [Test]
        public void GetRegistrationView()
        {
            var result = _controller.Register();
            
            Assert.IsInstanceOf<ViewResult>(result);
            var viewResult = result as ViewResult;
            Assert.IsNotNull(viewResult);
        }

        [TearDown]
        public void TearDown()
        {
            _controller.Dispose();
        }
    }
}
