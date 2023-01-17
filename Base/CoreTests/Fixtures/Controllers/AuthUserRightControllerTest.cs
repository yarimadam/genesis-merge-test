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
    public class AuthUserRightControllerTest
    {
        public static readonly Faker<AuthUserRight> EntityFaker = new Faker<AuthUserRight>()
            .RuleFor(e => e.ActionId, f => f.RandomByType<int>())
            .RuleFor(e => e.UserId, f => f.RandomByType<int>())
            .RuleFor(e => e.RecordType, f => f.RandomByType<int>())
            .RuleFor(e => e.Status, f => f.RandomByType<int>())
            .RuleFor(e => e.TenantId, f => f.RandomByType<int>());

        public static readonly List<AuthUserRight> Entities = EntityFaker.Generate(Constants.DEFAULT_ENTITY_GENERATION_COUNT);

        [Test, Order(1)]
        [TestCaseSource(nameof(Entities))]
        public async Task Insert_ValidRecord_ReturnsRecord(AuthUserRight entity)
        {
            // Arrange
            var controller = new AuthUserRightController();

            // Act
            var response = await controller.Insert(entity);

            // Assert
            // TODO Check for created date/user fields
            response.Should().BeValidWithData();
        }

        [Test, Order(2)]
        [TestCaseSource(nameof(Entities))]
        public async Task Update_ValidRecord_ReturnsRecord(AuthUserRight entity)
        {
            // Arrange
            var controller = new AuthUserRightController();

            // Update fields

            // Act
            var response = await controller.Update(entity);

            // Assert
            // TODO Check for updated date/user fields
            response.Should().BeValidWithData();
        }

        [Test, Order(3)]
        [TestCaseSource(nameof(Entities))]
        public async Task Get_ValidRecord_ReturnsRecord(AuthUserRight entity)
        {
            // Arrange
            var controller = new AuthUserRightController();

            // Act
            var response = await controller.Get(entity);

            // Assert
            response.Should().BeValidWithData();
        }

        [Test, Order(4)]
        [TestCaseSource(nameof(Entities))]
        public async Task List_ByEntity_ReturnsRecord(AuthUserRight entity)
        {
            // Arrange
            var controller = new AuthUserRightController();
            var request = TestHelper.CreateListRequest(entity);

            // Act
            var response = await controller.List(request);

            // Assert
            response.Should().BeValidWithData();
            response.Data.List.Should().HaveCount(1);
        }

        [Test, Order(5)]
        [TestCaseSource(nameof(Entities))]
        public async Task Delete_ValidRecord_ReturnsTrue(AuthUserRight entity)
        {
            // Arrange
            var controller = new AuthUserRightController();

            // Act
            var response = await controller.Delete(entity);

            // Assert
            response.Should().BeValidWithData();
            response.Data.Should().BeTrue();
        }
    }
}