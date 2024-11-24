using Microsoft.EntityFrameworkCore;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTBackend.Tests.Factories;

namespace TTTBackend.Tests.Data
{
    public class SceneTests
    {
        [Fact]
        public void Test_UserAndSceneRelationship()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryDbContext();

            var user = new User("JohnDoe", "john@example.com", "hashedpassword");
            var scene = new Scene { Name = "Battlefield", User = user };

            context.Users.Add(user);
            context.Scenes.Add(scene);
            context.SaveChanges();

            // Act
            var fetchedUser = context.Users.Include(u => u.Scenes).FirstOrDefault(u => u.Username == "JohnDoe");

            // Assert
            Assert.NotNull(fetchedUser);
            Assert.Single(fetchedUser.Scenes);
            Assert.Equal("Battlefield", fetchedUser.Scenes.First().Name);
        }
    }
}
