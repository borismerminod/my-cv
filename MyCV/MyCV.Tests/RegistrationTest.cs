using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using MyCV.Server;
using System.Net;
using HtmlAgilityPack;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyCV.Server.Areas.Identity.Data;

namespace MyCV.Tests
{
    internal class RegistrationTest
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private IServiceScope _scope;
        private UserManager<User> _userManager;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false // <--- C’est crucial pour que .Headers.Location fonctionne
            });

            _scope = _factory.Services.CreateScope();
            _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        }

        [Test]
        public async Task GetRegistrationPage()
        {
            var response = await _client.GetAsync("/Identity/Account/Register");

            Assert.IsTrue(response.IsSuccessStatusCode, "Expected a success status code.");
        }

        [TestCase("newUser", "newuser@example.com", "Password123!", "Password123!", HttpStatusCode.Found, "/Identity/Account/RegisterConfirmation?email=newuser@example.com&returnUrl=%2F")] 
        [TestCase("", "newuser@example.com", "Password123!", "Password123!", HttpStatusCode.OK, null)] 
        [TestCase("newUser", "", "Password123!", "Password123!", HttpStatusCode.OK, null)] 
        [TestCase("newUser", "newuserexamplecom", "Password123!", "Password123!", HttpStatusCode.OK, null)] 
        [TestCase("newUser", "newuser@example.com", "", "Password123!", HttpStatusCode.OK, null)] 
        [TestCase("newUser", "newuser@example.com", "Password123!!", "Password123!", HttpStatusCode.OK, null)] 
        [TestCase("newUser", "newuser@example.com", "Password", "Password123!", HttpStatusCode.OK, null)] 
        [TestCase("newUser", "newuser@example.com", "Password123!", "", HttpStatusCode.OK, null)] 
        [TestCase("newUser", "newuser@example.com", "Password123!", "Password123", HttpStatusCode.OK, null)]  
        public async Task SubmitRegistration(
            string username,
            string email,
            string password,
            string confirmPassword,
            HttpStatusCode expectedStatusCode,
            string? expectedRedirectURL
        )
        {
            // 1. Get the registration page (to get the anti-forgery token)
            var getResponse = await _client.GetAsync("/Identity/Account/Register");
            var html = await getResponse.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var token = doc.DocumentNode
                .SelectSingleNode("//input[@name='__RequestVerificationToken']")
                ?.Attributes["value"].Value;

            Assert.IsNotNull(token, "Anti-forgery token was not found");

            // 2. Prepare form data
            var formData = new Dictionary<string, string>
            {
                {"__RequestVerificationToken", token!},
                {"Input.Username", username},
                {"Input.Email", email},
                {"Input.Password", password},
                {"Input.ConfirmPassword", confirmPassword}
            };

            var content = new FormUrlEncodedContent(formData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            // 3. Submit the form
            var postResponse = await _client.PostAsync("/Identity/Account/Register", content);

            var responseHtml = await postResponse.Content.ReadAsStringAsync();


            // 4. Check redirect (default returnUrl is /)
            Assert.AreEqual(expectedStatusCode, postResponse.StatusCode);
            Assert.That(postResponse.Headers.Location?.OriginalString, Is.EqualTo(expectedRedirectURL));

        }

        [TestCase("newUser", "newuser@example.com", "newUser", "newuser2@example.com")]
        [TestCase("newUser", "newuser@example.com", "newUser2", "newuser@example.com")]
        public async Task SubmitSameUserForRegistration(string firstUsername, string firstEmail, string secondUsername, string secondEmail)
        {
            const string password = "Password123!";

            // 1. Get the registration page (to get the anti-forgery token)
            var getResponse = await _client.GetAsync("/Identity/Account/Register");
            var html = await getResponse.Content.ReadAsStringAsync();

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var token = doc.DocumentNode
                .SelectSingleNode("//input[@name='__RequestVerificationToken']")
                ?.Attributes["value"].Value;

            Assert.IsNotNull(token, "Anti-forgery token was not found");

            // 2. Prepare form data
            var formData = new Dictionary<string, string>
            {
                {"__RequestVerificationToken", token!},
                {"Input.Username", firstUsername},
                {"Input.Email", firstEmail},
                {"Input.Password", password},
                {"Input.ConfirmPassword", password}
            };

            var content = new FormUrlEncodedContent(formData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            // 3. Submit the form
            var postResponse = await _client.PostAsync("/Identity/Account/Register", content);

            // 4. Check redirect (default returnUrl is /)
            Assert.AreEqual(HttpStatusCode.Found, postResponse.StatusCode);
            Assert.That(postResponse.Headers.Location?.OriginalString, Is.EqualTo("/Identity/Account/RegisterConfirmation?email=newuser@example.com&returnUrl=%2F"));


            // 2. Prepare form data
            formData = new Dictionary<string, string>
            {
                {"__RequestVerificationToken", token!},
                {"Input.Username", secondUsername},
                {"Input.Email", secondEmail},
                {"Input.Password", password},
                {"Input.ConfirmPassword", password}
            };

            content = new FormUrlEncodedContent(formData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            // 3. Submit the form
            postResponse = await _client.PostAsync("/Identity/Account/Register", content);

            Assert.AreEqual(HttpStatusCode.OK, postResponse.StatusCode);
            Assert.That(postResponse.Headers.Location?.OriginalString, Is.EqualTo(null));

        }

        [TearDown]
        public void TearDown()
        {
            var user = _userManager.FindByEmailAsync("newuser@example.com").GetAwaiter().GetResult();
            if (user != null)
            {
                var result = _userManager.DeleteAsync(user).GetAwaiter().GetResult();
            }

            _client?.Dispose();
            _factory?.Dispose();
            _scope?.Dispose();
            _userManager?.Dispose();
        }
    }
}
