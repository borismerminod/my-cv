using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyCV.Server.Models;
using MyCV.Server.Data;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace MyCV.Tests
{
    internal class UserTest
    {
        private UserDbContext _context;

        [SetUp]
        public void SetUp()
        {
            //var options = new DbContextOptionsBuilder<UserDbContext>().UseInMemoryDatabase(databaseName: "MyCVTest").Options;

            var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseSqlServer("Server=DESKTOP-30BP46K\\DB_SERVER_WEB;Database=MYCV_DB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;")
            .Options;

            _context = new UserDbContext(options);
            /*_context.User.Add(new User {Username="test", Mail = "test@example.com", Password="testpass", ConfirmedPassword="testpass" });
            _context.SaveChanges();*/
        }

        [Test]
        public void GetUserById()
        {
            var user = _context.User.FirstOrDefault(u => u.Id == 1);
            Assert.IsNotNull(user);
            Assert.AreEqual(user.Username, "test");
            Assert.AreEqual(user.Mail, "test@example.com");
            Assert.AreEqual(user.Password, "testpass");

            user = _context.User.FirstOrDefault(u => u.Id == 2);
            Assert.IsNull(user);
        }

        [TearDown]
        public void TearDown()
        {
            /*var user = _context.User.FirstOrDefault(u => u.Id == 1);
            if (user != null)
            {
                _context.User.Remove(user);
                _context.SaveChanges();
            }*/
            _context.Dispose();
        }
        
    }
}
