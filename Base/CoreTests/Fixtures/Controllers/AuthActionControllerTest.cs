using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using CoreSvc.Controllers;
using CoreTests.Infrastructure;
using CoreTests.Infrastructure.Extensions;
using CoreType.DBModels;
using CoreType.Types;
using FluentAssertions;
using NUnit.Framework;

namespace CoreTests.Fixtures.Controllers
{
    [Category("Genesis_API")]
    public class AuthActionControllerTest
    {
        public static readonly Faker<AuthActions> EntityFaker = new Faker<AuthActions>()
            .RuleFor(e => e.Resource, f => AuthResourceControllerTest.EntityFaker.Generate())
            .RuleFor(e => e.ActionType, f => (int) f.PickRandom<ActionType>())
            .RuleFor(e => e.OrderIndex, f => f.RandomByType<int>())
            .RuleFor(e => e.Status, f => (int) f.PickRandomWithout(Status.Deleted));

        public static readonly List<AuthActions> Entities = EntityFaker.Generate(Constants.DEFAULT_ENTITY_GENERATION_COUNT);

        [Test, Order(1)]
        [TestCaseSource(nameof(Entities))]
        public async Task Insert_ValidRecord_ReturnsRecord(AuthActions entity)
        {
            // Arrange
            var controller = new AuthActionsController();

            // Act
            var response = await controller.Insert(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(2)]
        [TestCaseSource(nameof(Entities))]
        public async Task Update_ValidRecord_ReturnsRecord(AuthActions entity)
        {
            // Arrange
            var controller = new AuthActionsController();

            // Update fields

            // Act
            var response = await controller.Update(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(3)]
        [TestCaseSource(nameof(Entities))]
        public async Task Get_ValidRecord_ReturnsRecord(AuthActions entity)
        {
            // Arrange
            var controller = new AuthActionsController();

            // Act
            var response = await controller.Get(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(4)]
        [TestCaseSource(nameof(Entities))]
        public async Task List_ByEntity_ReturnsRecord(AuthActions entity)
        {
            // Arrange
            var controller = new AuthActionsController();
            var request = TestHelper.CreateListRequest(entity);

            // Act
            var response = await controller.List(request);

            // Assert
            response.Should().BeValidWithData();
            response.Data.List.Should().HaveCount(1);
        }

        [Test, Order(5)]
        [TestCaseSource(nameof(Entities))]
        public async Task Delete_ValidRecord_ReturnsTrue(AuthActions entity)
        {
            // Arrange
            var controller = new AuthActionsController();

            // Act
            var response = await controller.Delete(entity);

            // Assert
            response.Should().BeValidWithData();
            response.Data.Should().BeTrue();
        }
    }
}