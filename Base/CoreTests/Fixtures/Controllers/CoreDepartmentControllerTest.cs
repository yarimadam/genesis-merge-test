using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using CoreTests;
using CoreTests.Infrastructure;
using CoreTests.Infrastructure.Extensions;
using FluentAssertions;
using NUnit.Framework;
using CoreSvc.Controllers;
using CoreType.DBModels;

namespace CoreTests.Fixtures.Controllers
{
    [Category("API")]
    public class CoreDepartmentControllerTest
    {
        public static readonly Faker<CoreDepartment> EntityFaker = new Faker<CoreDepartment>()
            .RuleFor(e => e.DepartmentName, f => f.RandomByType<string>(maxLength: 100))
            .RuleFor(e => e.Status, f => f.RandomByType<short>())
            .RuleFor(e => e.TenantId, f => f.RandomByType<int>());

        public static readonly List<CoreDepartment> Entities = EntityFaker.Generate(Constants.DEFAULT_ENTITY_GENERATION_COUNT);

        [Test, Order(1)]
        [TestCaseSource(nameof(Entities))]
        public async Task Insert_ValidRecord_ReturnsRecord(CoreDepartment entity)
        {
            // Arrange
            var controller = new CoreDepartmentController();

            // Act
            var response = await controller.Insert(entity);

            // Assert
            // TODO Check for created date/user fields
            response.Should().BeValidWithData();
        }

        [Test, Order(2)]
        [TestCaseSource(nameof(Entities))]
        public async Task Update_ValidRecord_ReturnsRecord(CoreDepartment entity)
        {
            // Arrange
            var controller = new CoreDepartmentController();

            // Update fields

            // Act
            var response = await controller.Update(entity);

            // Assert
            // TODO Check for updated date/user fields
            response.Should().BeValidWithData();
        }

        [Test, Order(3)]
        [TestCaseSource(nameof(Entities))]
        public async Task Get_ValidRecord_ReturnsRecord(CoreDepartment entity)
        {
            // Arrange
            var controller = new CoreDepartmentController();

            // Act
            var response = await controller.Get(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(4)]
        [TestCaseSource(nameof(Entities))]
        public async Task List_ByEntity_ReturnsRecord(CoreDepartment entity)
        {
            // Arrange
            var controller = new CoreDepartmentController();
            var request = TestHelper.CreateListRequest(entity);

            // Act
            var response = await controller.List(request);

            // Assert
            response.Should().BeValidWithData();
            response.Data.List.Should().HaveCount(1);
        }

        [Test, Order(5)]
        [TestCaseSource(nameof(Entities))]
        public async Task Delete_ValidRecord_ReturnsTrue(CoreDepartment entity)
        {
            // Arrange
            var controller = new CoreDepartmentController();

            // Act
            var response = await controller.Delete(entity);

            // Assert
            response.Should().BeValidWithData();
            response.Data.Should().BeTrue();
        }
    }
}