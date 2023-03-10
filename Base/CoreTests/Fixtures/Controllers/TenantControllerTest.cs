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
    public class TenantControllerTest
    {
        public static readonly Faker<Tenant> EntityFaker = new Faker<Tenant>()
            .RuleFor(e => e.TenantName, f => f.Company.CompanyName())
            .RuleFor(e => e.TenantType, f => (int) f.PickRandom<TenantType>())
            .RuleFor(e => e.TaxNumber, f => f.Random.AlphaNumeric(f.Random.Number(1, 10)).ToUpperInvariant())
            .RuleFor(e => e.Status, f => (int) f.PickRandomWithout(Status.Deleted));

        public static readonly List<Tenant> Entities = EntityFaker.Generate(Constants.DEFAULT_ENTITY_GENERATION_COUNT);

        [Test, Order(1)]
        [TestCaseSource(nameof(Entities))]
        public async Task Insert_ValidRecord_ReturnsRecord(Tenant entity)
        {
            // Arrange
            var controller = new TenantController();

            // Act
            var response = await controller.Insert(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(2)]
        [TestCaseSource(nameof(Entities))]
        public async Task Update_ValidRecord_ReturnsRecord(Tenant entity)
        {
            // Arrange
            var controller = new TenantController();

            // Update fields

            // Act
            var response = await controller.Update(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(3)]
        [TestCaseSource(nameof(Entities))]
        public async Task Get_ValidRecord_ReturnsRecord(Tenant entity)
        {
            // Arrange
            var controller = new TenantController();

            // Act
            var response = await controller.Get(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(4)]
        [TestCaseSource(nameof(Entities))]
        public async Task List_ByEntity_ReturnsRecord(Tenant entity)
        {
            // Arrange
            var controller = new TenantController();
            var request = TestHelper.CreateListRequest(entity);

            // Act
            var response = await controller.List(request);

            // Assert
            response.Should().BeValidWithData();
            response.Data.List.Should().HaveCount(1);
        }

        // [Test, Order(5)]
        // [TestCaseSource(nameof(Entities))]
        // public async Task Delete_ValidRecord_ReturnsTrue(Tenant entity)
        // {
        //     // Arrange
        //     var controller = new TenantController();
        //
        //     // Act
        //     var response = await controller.Delete(entity);
        //
        //     // Assert
        //     response.Should().BeValidWithData();
        //     response.Data.Should().BeTrue();
        // }
    }
}