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
    public class AuthTemplateDetailControllerTest
    {
        public static readonly Faker<AuthTemplateDetail> EntityFaker = new Faker<AuthTemplateDetail>()
            .RuleFor(e => e.AuthTemplate, f => AuthTemplateControllerTest.EntityFaker.Generate())
            .RuleFor(e => e.ResourceId, f => f.RandomByType<int>(minValue: 1))
            .RuleFor(e => e.ActionId, f => f.RandomByType<int>(minValue: 1))
            .RuleFor(e => e.Status, f => (int) f.PickRandomWithout(Status.Deleted));

        public static readonly List<AuthTemplateDetail> Entities = EntityFaker.Generate(Constants.DEFAULT_ENTITY_GENERATION_COUNT);

        [Test, Order(1)]
        [TestCaseSource(nameof(Entities))]
        public async Task Insert_ValidRecord_ReturnsRecord(AuthTemplateDetail entity)
        {
            // Arrange
            var controller = new AuthTemplateDetailsController();

            // Act
            var response = await controller.Insert(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(2)]
        [TestCaseSource(nameof(Entities))]
        public async Task Update_ValidRecord_ReturnsRecord(AuthTemplateDetail entity)
        {
            // Arrange
            var controller = new AuthTemplateDetailsController();

            // Update fields

            // Act
            var response = await controller.Update(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(3)]
        [TestCaseSource(nameof(Entities))]
        public async Task Get_ValidRecord_ReturnsRecord(AuthTemplateDetail entity)
        {
            // Arrange
            var controller = new AuthTemplateDetailsController();

            // Act
            var response = await controller.Get(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(4)]
        [TestCaseSource(nameof(Entities))]
        public async Task List_ByEntity_ReturnsRecord(AuthTemplateDetail entity)
        {
            // Arrange
            var controller = new AuthTemplateDetailsController();
            var request = TestHelper.CreateListRequest(entity);

            // Act
            var response = await controller.List(request);

            // Assert
            response.Should().BeValidWithData();
            response.Data.Should().HaveCount(1);
        }

        [Test, Order(5)]
        [TestCaseSource(nameof(Entities))]
        public async Task Delete_ValidRecord_ReturnsTrue(AuthTemplateDetail entity)
        {
            // Arrange
            var controller = new AuthTemplateDetailsController();

            // Act
            var response = await controller.Delete(entity);

            // Assert
            response.Should().BeValidWithData();
            response.Data.Should().BeTrue();
        }
    }
}