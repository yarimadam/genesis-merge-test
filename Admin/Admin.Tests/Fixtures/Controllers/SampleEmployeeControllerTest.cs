using System.Collections.Generic;
using System.Threading.Tasks;
using Admin.Svc.Controllers;
using Bogus;
using CoreTests;
using CoreTests.Infrastructure;
using CoreTests.Infrastructure.Extensions;
using CoreType.DBModels;
using FluentAssertions;
using NUnit.Framework;

namespace Admin.Tests.Fixtures.Controllers
{
    [Category("Genesis_API")]
    public class SampleEmployeeControllerTest
    {
        public static readonly Faker<SampleEmployee> EntityFaker = new Faker<SampleEmployee>()
            .RuleFor(e => e.EmployeeName, (f, u) => f.Name.FirstName())
            .RuleFor(e => e.EmployeeSurname, (f, u) => f.Name.LastName())
            .RuleFor(e => e.Email, f => f.Internet.Email())
            .RuleFor(e => e.TaxNumber, f => f.Random.AlphaNumeric(f.Random.Number(1, 10)));

        public static readonly List<SampleEmployee> Entities = EntityFaker.Generate(Constants.DEFAULT_ENTITY_GENERATION_COUNT);

        [Test, Order(1)]
        [TestCaseSource(nameof(Entities))]
        public async Task Insert_ValidRecord_ReturnsRecord(SampleEmployee entity)
        {
            // Arrange
            var controller = new SampleEmployeeController();

            // Act
            var response = await controller.Insert(entity);

            // Assert
            // TODO Check for created date/user fields
            response.Should().BeValidWithData();
        }

        [Test, Order(2)]
        [TestCaseSource(nameof(Entities))]
        public async Task Update_ValidRecord_ReturnsRecord(SampleEmployee entity)
        {
            // Arrange
            var controller = new SampleEmployeeController();

            // Update fields

            // Act
            var response = await controller.Update(entity);

            // Assert
            // TODO Check for updated date/user fields
            response.Should().BeValidWithData();
        }

        [Test, Order(3)]
        [TestCaseSource(nameof(Entities))]
        public async Task Get_ValidRecord_ReturnsRecord(SampleEmployee entity)
        {
            // Arrange
            var controller = new SampleEmployeeController();

            // Act
            var response = await controller.Get(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(4)]
        [TestCaseSource(nameof(Entities))]
        public async Task List_ByEntity_ReturnsRecord(SampleEmployee entity)
        {
            // Arrange
            var controller = new SampleEmployeeController();
            var request = TestHelper.CreateListRequest(entity);

            // Act
            var response = await controller.List(request);

            // Assert
            response.Should().BeValidWithData();
            response.Data.List.Should().HaveCount(1);
        }

        [Test, Order(5)]
        [TestCaseSource(nameof(Entities))]
        public async Task Delete_ValidRecord_ReturnsTrue(SampleEmployee entity)
        {
            // Arrange
            var controller = new SampleEmployeeController();

            // Act
            var response = await controller.Delete(entity);

            // Assert
            response.Should().BeValidWithData();
            response.Data.Should().BeTrue();
        }
    }
}