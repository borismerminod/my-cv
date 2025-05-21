using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using MyCV.Server.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyCV.Tests
{
    internal class LoginTest
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false // <--- C’est crucial pour que .Headers.Location fonctionne
            });
        }

        [Test]
        public async Task GetLoginPage()
        {
            // Effectuer une requête GET sur la page de login
            var response = await _client.GetAsync("/Identity/Account/Login");

            // Vérifier que le code de statut est un succès (200 OK)
            Assert.IsTrue(response.IsSuccessStatusCode, "Expected a success status code.");
        }

        [TestCase("test@test.com", "Password123!", "false", HttpStatusCode.Found, "/", false, true)]
        [TestCase("testuser", "Password123!", "false", HttpStatusCode.Found, "/", false, true)]
        [TestCase("test@test.com", "Password123!", "true", HttpStatusCode.Found, "/", false, true)]
        [TestCase("testuser", "Password123!", "true", HttpStatusCode.Found, "/", false, true)]
        [TestCase("testtest.com", "Password123!", "false", HttpStatusCode.OK, null, true, false)]
        [TestCase("nonExistantUser", "Password123!", "false", HttpStatusCode.OK, null, true, false)]
        [TestCase("", "Password123!", "false", HttpStatusCode.OK, null, true, false)]
        [TestCase("test@test.com", "UncorrectPass", "false", HttpStatusCode.OK, null, true, false)]
        [TestCase("testuser", "UncorrectPass", "false", HttpStatusCode.OK, null, true, false)]
        [TestCase("test@test.com", "", "false", HttpStatusCode.OK, null, true, false)]
        [TestCase("testuser", "", "false", HttpStatusCode.OK, null, true, false)]
        [TestCase("test@test.com", "Password123!", "IncorrectInput", HttpStatusCode.InternalServerError, null, true, false)]
        [TestCase("testuser", "Password123!", "", HttpStatusCode.OK, null, true, false)]
        public async Task LoginUser(
            string usernameOrEmail, 
            string password, 
            string rememberMe, 
            HttpStatusCode expectedStatusCode, 
            string? expectedRedirection,
            bool shouldBeNull,
            bool shouldLogout
        )
        {

            // 1. Obtenir la page de login pour récupérer le token anti-forgery
            var getResponse = await _client.GetAsync("/Identity/Account/Login");
            var html = await getResponse.Content.ReadAsStringAsync();

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            var token = doc.DocumentNode
                .SelectSingleNode("//input[@name='__RequestVerificationToken']")
                ?.Attributes["value"].Value;

            Assert.IsNotNull(token, "Anti-forgery token was not found");

            // 2. Préparer les données du formulaire
            var formData = new Dictionary<string, string>
            {
                {"__RequestVerificationToken", token!},
                {"Input.UsernameOrEmail", usernameOrEmail},
                {"Input.Password", password},
                {"Input.RememberMe", rememberMe}
            };

            var content = new FormUrlEncodedContent(formData);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

            // 3. Soumettre le formulaire de login
            var postResponse = await _client.PostAsync("/Identity/Account/Login", content);

            // 4. Vérifier la redirection après un login réussi
            Assert.AreEqual(expectedStatusCode, postResponse.StatusCode);
            Assert.That(postResponse.Headers.Location?.OriginalString, Is.EqualTo(expectedRedirection));

            // 5. Tester le logout
            //if (expectedStatusCode == HttpStatusCode.Found && expectedRedirection == "/")
           // {
                var logoutGetResponse = await _client.GetAsync("/Identity/Account/Logout");
                var logoutHtml = await logoutGetResponse.Content.ReadAsStringAsync();

                var logoutDoc = new HtmlAgilityPack.HtmlDocument();
                logoutDoc.LoadHtml(logoutHtml);

                var logoutToken = logoutDoc.DocumentNode
                .SelectSingleNode("//input[@name='__RequestVerificationToken']")
                ?.Attributes["value"].Value;

                // Vérifier que le token anti-forgery pour le logout a été trouvé   
                if(shouldBeNull == true)
                    Assert.IsNull(logoutToken, "Anti-forgery token for logout should be null"); 
                 else
                    Assert.IsNotNull(logoutToken, "Anti-forgery token for logout was not found");

                // Préparer les données pour le logout
                var logoutFormData = new Dictionary<string, string>
                {
                    {"__RequestVerificationToken", logoutToken!} // Réutiliser le token anti-forgery
                };

                var logoutContent = new FormUrlEncodedContent(logoutFormData);
                logoutContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");

                // Soumettre le formulaire de logout
                var logoutPostResponse = await _client.PostAsync("/Identity/Account/Logout?returnUrl=%2F%3Fpage%3D%252FIndex", logoutContent);

                // Vérifier que le logout a réussi (redirection ou autre comportement attendu)
                Assert.AreEqual(HttpStatusCode.Found, logoutPostResponse.StatusCode);

                if(shouldLogout == true)
                {
                    // Vérifier que le logout a été effectué
                    var logoutRedirectUrl = logoutPostResponse.Headers.Location?.OriginalString;
                    Assert.That(logoutRedirectUrl, Is.EqualTo("/?page=%2FIndex"));
                }
                else
                {
                    // Vérifier que le logout n'a pas été effectué
                    Assert.That(logoutPostResponse.Headers.Location?.OriginalString, Is.Not.EqualTo("/?page=%2FIndex"));
                }
            //}
        }

        [TearDown]
        public void TearDown()
        {
            _factory?.Dispose();
            _client?.Dispose();
        }
    }
}
