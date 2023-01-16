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
    public class CoreParameterControllerTest
    {
        public static readonly Faker<CoreParameters> EntityFaker = new Faker<CoreParameters>()
            .RuleFor(e => e.KeyCode, f => f.RandomByType<string>(maxLength: 100))
            .RuleFor(e => e.ParentValue, f => f.RandomByType<int>())
            .RuleFor(e => e.Value, f => f.RandomByType<string>(maxLength: 50));

        public static readonly List<CoreParameters> Entities = EntityFaker.Generate(Constants.DEFAULT_ENTITY_GENERATION_COUNT);

        [Test, Order(1)]
        [TestCaseSource(nameof(Entities))]
        public async Task Insert_ValidRecord_ReturnsRecord(CoreParameters entity)
        {
            // Arrange
            var controller = new ParameterController();

            // Act
            var response = await controller.Insert(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(2)]
        [TestCaseSource(nameof(Entities))]
        public async Task Update_ValidRecord_ReturnsRecord(CoreParameters entity)
        {
            // Arrange
            var controller = new ParameterController();

            // Update fields

            // Act
            var response = await controller.Update(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(3)]
        [TestCaseSource(nameof(Entities))]
        public async Task Get_ValidRecord_ReturnsRecord(CoreParameters entity)
        {
            // Arrange
            var controller = new ParameterController();

            // Act
            var response = await controller.Get(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(4)]
        [TestCaseSource(nameof(Entities))]
        public async Task List_ByEntity_ReturnsRecord(CoreParameters entity)
        {
            // Arrange
            var controller = new ParameterController();
            var request = TestHelper.CreateListRequest(entity);

            // Act
            var response = await controller.List(request);

            // Assert
            response.Should().BeValidWithData();
            response.Data.List.Should().HaveCount(1);
        }

        [Test, Order(5)]
        [TestCaseSource(nameof(Entities))]
        public async Task Delete_ValidRecord_ReturnsTrue(CoreParameters entity)
        {
            // Arrange
            var controller = new ParameterController();

            // Act
            var response = await controller.Delete(entity);

            // Assert
            response.Should().BeValidWithData();
            response.Data.Should().BeTrue();
        }
    }
}