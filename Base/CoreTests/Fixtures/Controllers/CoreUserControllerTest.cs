using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using CoreSvc.Controllers;
using CoreTests.Infrastructure;
using CoreTests.Infrastructure.Extensions;
using CoreType.DBModels;
using FluentAssertions;
using NUnit.Framework;

namespace CoreTests.Fixtures.Controllers
{
    [Category("Genesis_API")]
    public class CoreUserControllerTest
    {
        public static readonly Faker<CoreUsers> EntityFaker = new Faker<CoreUsers>()
            .RuleFor(e => e.Name, f => f.Name.FirstName())
            .RuleFor(e => e.Surname, f => f.Name.LastName())
            .RuleFor(e => e.Email, (f, u) => f.Internet.Email(u.Name, u.Surname))
            .RuleFor(e => e.Password, (f, u) => f.Internet.Password(8))
            .RuleFor(e => e.ShouldChangePassword, f => f.RandomByType<bool>());

        public static readonly List<CoreUsers> Entities = EntityFaker.Generate(Constants.DEFAULT_ENTITY_GENERATION_COUNT);

        [Test, Order(1)]
        [TestCaseSource(nameof(Entities))]
        public async Task Insert_ValidRecord_ReturnsRecord(CoreUsers entity)
        {
            // Arrange
            var controller = new UserController();

            // Act
            var response = await controller.Insert(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(2)]
        [TestCaseSource(nameof(Entities))]
        public async Task Update_ValidRecord_ReturnsRecord(CoreUsers entity)
        {
            // Arrange
            var controller = new UserController();

            // Update fields

            // Act
            var response = await controller.Update(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(3)]
        [TestCaseSource(nameof(Entities))]
        public async Task Get_ValidRecord_ReturnsRecord(CoreUsers entity)
        {
            // Arrange
            var controller = new UserController();

            // Act
            var response = await controller.Get(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(4)]
        [TestCaseSource(nameof(Entities))]
        public async Task List_ByEntity_ReturnsRecord(CoreUsers entity)
        {
            // Arrange
            var controller = new UserController();
            var request = TestHelper.CreateListRequest(entity);

            // Act
            var response = await controller.List(request);

            // Assert
            response.Should().BeValidWithData();
            response.Data.List.Should().HaveCount(1);
        }

        [Test, Order(5)]
        [TestCaseSource(nameof(Entities))]
        public async Task Delete_ValidRecord_ReturnsTrue(CoreUsers entity)
        {
            // Arrange
            var controller = new UserController();

            // Act
            var response = await controller.Delete(entity);

            // Assert
            response.Should().BeValidWithData();
            response.Data.Should().BeTrue();
        }
    }
}