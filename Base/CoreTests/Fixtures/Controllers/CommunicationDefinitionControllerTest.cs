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
    public class CommunicationDefinitionControllerTest
    {
        public static readonly Faker<CommunicationDefinitions> EntityFaker = new Faker<CommunicationDefinitions>()
            .RuleFor(e => e.CommDefinitionName, f => f.RandomByType<string>(maxLength: 100))
            .RuleFor(e => e.CommDefinitionType, f => f.RandomByType<short>(minValue: 1, maxValue: 2))
            .RuleFor(e => e.Status, f => (short) f.PickRandomWithout(Status.Deleted));

        public static readonly List<CommunicationDefinitions> Entities = EntityFaker.Generate(Constants.DEFAULT_ENTITY_GENERATION_COUNT);

        [Test, Order(1)]
        [TestCaseSource(nameof(Entities))]
        public async Task Insert_ValidRecord_ReturnsRecord(CommunicationDefinitions entity)
        {
            // Arrange
            var controller = new CommunicationDefinitionsController();

            // Act
            var response = await controller.Insert(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(2)]
        [TestCaseSource(nameof(Entities))]
        public async Task Update_ValidRecord_ReturnsRecord(CommunicationDefinitions entity)
        {
            // Arrange
            var controller = new CommunicationDefinitionsController();

            // Update fields

            // Act
            var response = await controller.Update(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(3)]
        [TestCaseSource(nameof(Entities))]
        public async Task Get_ValidRecord_ReturnsRecord(CommunicationDefinitions entity)
        {
            // Arrange
            var controller = new CommunicationDefinitionsController();

            // Act
            var response = await controller.Get(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(4)]
        [TestCaseSource(nameof(Entities))]
        public async Task List_ByEntity_ReturnsRecord(CommunicationDefinitions entity)
        {
            // Arrange
            var controller = new CommunicationDefinitionsController();
            var request = TestHelper.CreateListRequest(entity);

            // Act
            var response = await controller.List(request);

            // Assert
            response.Should().BeValidWithData();
            response.Data.List.Should().HaveCount(1);
        }

        [Test, Order(5)]
        [TestCaseSource(nameof(Entities))]
        public async Task Delete_ValidRecord_ReturnsTrue(CommunicationDefinitions entity)
        {
            // Arrange
            var controller = new CommunicationDefinitionsController();

            // Act
            var response = await controller.Delete(entity);

            // Assert
            response.Should().BeValidWithData();
            response.Data.Should().BeTrue();
        }
    }
}