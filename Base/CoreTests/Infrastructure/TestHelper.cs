using CoreType.Types;

namespace CoreTests.Infrastructure
{
    public static class TestHelper
    {
        public static RequestWithPagination<T> CreateListRequest<T>(T entity) where T : new()
        {
            return new RequestWithPagination<T>
            {
                Criteria = entity,
                Pagination = { CurrentPage = 1 }
            };
        }

        // response.Data.List.Should().HaveCount(response.Data.Pagination.ResultRowCount);
        // response.Data.List.Should().HaveCountLessOrEqualTo(request.Pagination.MaxRowsPerPage);

        // namespace CoreTests
        // {
        //     public class CompanyControllerTest : ControllerTestBase<CoreCompanyController, CoreCompany>
        //     {
        //         // private readonly GenesisContextBase _context;
        //         // private readonly CompanyService _service;
        //
        //         private static readonly Faker<CoreCompany> EntityFaker = new Faker<CoreCompany>()
        //                 .RuleFor(e => e.CompanyName, f => f.Company.CompanyName())
        //                 .RuleFor(e => e.ContactPersonEmail, f => f.Internet.Email())
        //                 .RuleFor(e => e.Email, f => f.Internet.Email());
        //
        //         private static readonly List<CoreCompany> Entities = EntityFaker.Generate(100);
        //
        //         public CompanyControllerTest()
        //         {
        //             // var serviceScope = TestContext.WebApplicationFactory.Services.CreateScope();
        //             // _context = serviceScope.ServiceProvider.GetRequiredService<genesisContextEx_PostgreSQL>();
        //             // _context = TestContext.ServiceProvider.GetRequiredService<genesisContextEx_PostgreSQL>();
        //             // _service = new CompanyService(_context);
        //         }
        //
        //         [Test]
        //         public override async Task Insert_ValidRecord_ReturnsRecord(
        //             [Values(nameof(CoreCompanyController.Insert))]
        //             string actionUrl,
        //             [ValueSource(nameof(Entities))] CoreCompany entity
        //         )
        //         {
        //             await base.Insert_ValidRecord_ReturnsRecord(actionUrl, entity);
        //         }
        //
        //         //         [Test, Order(1)]
        // //         [TestCaseSource(nameof(Companies))]
        // //         public virtual async Task Insert_ValidRecord_ReturnsRecord(CoreCompany entity)
        // //         {
        // //             Console.WriteLine(JsonConvert.SerializeObject(entity));
        // //
        // //             var actionDescriptors = ServiceLocator.Current.GetService<IActionDescriptorCollectionProvider>()
        // //                 .ActionDescriptors
        // //                 .Items
        // //                 // .Select(descriptor => descriptor)
        // //                 .Where(descriptor => (descriptor as ControllerActionDescriptor)?.ControllerTypeInfo == typeof(CoreCompanyController))
        // //                 .ToList();
        // //
        // //             var requestMessage = new HttpRequestMessage(HttpMethod.Post, actionDescriptors[2].AttributeRouteInfo.Template)
        // //             {
        // //                 Content = JsonConvert.SerializeObject(entity).AsStringContent()
        // //             };
        // //
        // //             HttpResponseMessage result = await TestContext.Client.SendAsync(requestMessage);
        // //
        // //             result.EnsureSuccessStatusCode();
        // //
        // //             // var result = await TestContext.Client.PostAsync(actionDescriptors.First().AttributeRouteInfo.Template);
        // //
        // //             // TestContext.Client.GetAsync(/)
        // //             /*using (var serviceScope = TestContext.WebApplicationFactory.Services.CreateScope())
        // //             {
        // //                 var _context = serviceScope.ServiceProvider.GetRequiredService<genesisContextEx_PostgreSQL>();
        // //
        // //                 // Arrange
        // //                 var _service = new CompanyService(_context);
        // //
        // //                 // Act
        // //                 var result = await _service.SaveAsync(entity);
        // //
        // //                 // Assert
        // //                 result.Should().BeValidWithData();
        // //             }*/
        // //         }
        //
        //         [Test, Order(2)]
        //         [TestCaseSource(nameof(Entities))]
        //         public virtual async Task GetById_ReturnsRecord(CoreCompany entity)
        //         {
        //             // Arrange
        //             var _service = new CompanyService();
        //
        //             // Act
        //             var result = await _service.GetAsync(entity);
        //
        //             // Assert
        //             result.Should().BeValidWithData();
        //         }
        //
        //         [Test]
        //         public virtual async Task GetAll_ReturnsAllRecords()
        //         {
        //             // Arrange
        //             var _service = new CompanyService();
        //
        //             // Act
        //             var result = await _service.GetAllAsync();
        //
        //             // Assert
        //             result.Should().BeValidWithData();
        //         }
        //
        //         [Test]
        //         public virtual async Task List_GuidFilter_ReturnsEmpty()
        //         {
        //             // Arrange
        //             var _service = new CompanyService();
        //             var request = new RequestWithPagination<CoreCompany> { Pagination = { CurrentPage = 1 } };
        //             // Act
        //             var result = await _service.ListAsync(request);
        //
        //             // Assert
        //             result.Should().BeValidWithData();
        //             result.Data.List.Should().HaveCount(result.Data.Pagination.ResultRowCount);
        //             result.Data.List.Should().HaveCountLessOrEqualTo(request.Pagination.MaxRowsPerPage);
        //         }
        //     }
        // }
    }
}