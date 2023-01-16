using System;
using System.Collections.Generic;
using Bogus;
using CoreTests.Infrastructure.Extensions;
using CoreType.DBModels;
using CoreType.Types;
using NUnit.Framework;

namespace CoreTests.Fixtures.Controllers
{
    [Category("Genesis_API")]
    public class TransactionLogControllerTest
    {
        public static readonly Faker<TransactionLogs> EntityFaker = new Faker<TransactionLogs>()
            .RuleFor(e => e.UserId, f => f.RandomByType<int>())
            .RuleFor(e => e.ServiceUrl, f => f.Internet.UrlRootedPath())
            .RuleFor(e => e.LogType, f => f.RandomByType<int>())
            .RuleFor(e => e.LogDateBegin, f => f.RandomByType<DateTime>())
            .RuleFor(e => e.Status, f => (int) f.PickRandomWithout(Status.Deleted));

        public static readonly List<TransactionLogs> Entities = EntityFaker.Generate(Constants.DEFAULT_ENTITY_GENERATION_COUNT);

        // [Test, Order(1)]
        // [TestCaseSource(nameof(Entities))]
        // public async Task Insert_ValidRecord_ReturnsRecord(TransactionLogs entity)
        // {
        //     // Arrange
        //     var controller = new TransactionLogsController();
        //
        //     // Act
        //     var response = await controller.Insert(entity);
        //
        //     // Assert
        //     // TODO Check for created date/user fields
        //     response.Should().BeValidWithData();
        // }

        // [Test, Order(2)]
        // [TestCaseSource(nameof(Entities))]
        // public async Task Update_ValidRecord_ReturnsRecord(TransactionLogs entity)
        // {
        //     // Arrange
        //     var controller = new TransactionLogsController();
        //
        //     // Update fields
        //
        //     // Act
        //     var response = await controller.Update(entity);
        //
        //     // Assert
        //     // TODO Check for updated date/user fields
        //     response.Should().BeValidWithData();
        // }

        // [Test, Order(3)]
        // [TestCaseSource(nameof(Entities))]
        // public async Task Get_ValidRecord_ReturnsRecord(TransactionLogs entity)
        // {
        //     // Arrange
        //     var controller = new TransactionLogsController();
        //
        //     // Act
        //     var response = await controller.Get(entity);
        //
        //     // Assert
        //     response.Should().BeValidWithData();
        // }

        // [Test, Order(4)]
        // [TestCaseSource(nameof(Entities))]
        // public async Task List_ByEntity_ReturnsRecord(TransactionLogs entity)
        // {
        //     // Arrange
        //     var controller = new TransactionLogsController();
        //     var request = TestHelper.CreateListRequest(entity);
        //
        //     // Act
        //     var response = await controller.List(request);
        //
        //     // Assert
        //     response.Should().BeValidWithData();
        //     response.Data.List.Should().HaveCount(1);
        // }

        // [Test, Order(5)]
        // [TestCaseSource(nameof(Entities))]
        // public async Task Delete_ValidRecord_ReturnsTrue(TransactionLogs entity)
        // {
        //     // Arrange
        //     var controller = new TransactionLogsController();
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