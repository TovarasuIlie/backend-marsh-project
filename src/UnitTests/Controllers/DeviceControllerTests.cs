using BackendMarshProject.Controllers;
using BackendMarshProject.Data;
using BackendMarshProject.DTOs.Device;
using BackendMarshProject.Entities;
using BackendMarshProject.Entities.Paging;
using BackendMarshProject.Enums;
using BackendMarshProject.Repository;
using BackendMarshProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests.Controller
{
    [TestClass]
    public class DeviceControllerTests
    {
        private DeviceController _controller;
        private DeviceService _service;
        private Mock<LLMService> _mockLLMService;
        private AppDbContext _appDbContext;
        private IDbContextTransaction _transaction;

        [TestInitialize]
        public void Setup()
        {
            var mockFactory = new Mock<IHttpClientFactory>();
            var mockConfig = new Mock<IConfiguration>();

            _mockLLMService = new Mock<LLMService>(mockFactory.Object, mockConfig.Object);

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("server=localhost;Database=MarshProjectDatabase;Trusted_Connection=True;TrustServerCertificate=True")
                .Options;

            _appDbContext = new AppDbContext(options);

            _appDbContext.Database.OpenConnection();

            _transaction = _appDbContext.Database.BeginTransaction();

            var deviceRepository = new DeviceRepository(_appDbContext);

            _service = new DeviceService(deviceRepository, _mockLLMService.Object);

            _controller = new DeviceController(_service);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            if (_appDbContext != null)
            {
                _appDbContext.Database.CloseConnection();
                _appDbContext.Dispose();
            }
        }

        [TestMethod]
        public async Task GetDevices_SuccessTest()
        {
            var response = await _controller.GetDevices(new()
            {
                PageNumber = 1,
                PageSize = 5
            });

            var okResult = response.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);

            var pagedResult = okResult.Value as PagedResult<DeviceDTO>;
            Assert.IsNotNull(pagedResult);
            Assert.AreEqual(5, pagedResult.Data.Count());
        }

        [TestMethod]
        public async Task GetDevices_InvalidPageNumber()
        {

            _controller.ModelState.AddModelError("PageNumber", "Page number must be greater than 0.");

            var response = await _controller.GetDevices(new()
            {
                PageNumber = -1,
                PageSize = 5
            });

            var badRequestResult = response.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(badRequestResult.Value);

            var property = badRequestResult.Value.GetType().GetProperty("message");
            Assert.IsNotNull(property);

            var errors = property.GetValue(badRequestResult.Value) as IEnumerable<string>;

            Assert.IsNotNull(errors);
            Assert.AreEqual("Page number must be greater than 0.", errors.First());
        }

        [TestMethod]
        public async Task GetDevices_InvalidPageSize()
        {

            _controller.ModelState.AddModelError("PageNumber", "Page number must be between 1 and 50.");

            var response = await _controller.GetDevices(new()
            {
                PageNumber = 1,
                PageSize = -5
            });

            var badRequestResult = response.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(badRequestResult.Value);

            var property = badRequestResult.Value.GetType().GetProperty("message");
            Assert.IsNotNull(property);

            var errors = property.GetValue(badRequestResult.Value) as IEnumerable<string>;

            Assert.IsNotNull(errors);
            Assert.AreEqual("Page number must be between 1 and 50.", errors.First());
        }

        [TestMethod]
        public async Task EditDevice_SuccessTest()
        {
            var response = await _controller.EditDevice(9, new()
            {
                OperatingSystem = "Windows Test",
                OSVersion = "11 Test",
                Processor = "Intel i7 Test",
                RAMAmount = 17,
                Description = "Unit test device"
            });
            var okResult = response.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);

            var pagedResult = okResult.Value as Device;

            Assert.IsNotNull(pagedResult);
            Assert.IsNotNull(pagedResult.Id);
            Assert.AreEqual(9, pagedResult.Id);
            Assert.AreEqual("Surface Pro 9", pagedResult.Name);
            Assert.AreEqual("Microsoft", pagedResult.Manufacturer);
            Assert.IsNull(pagedResult.AssignedToUser);
            Assert.AreEqual(DeviceType.Tablet, pagedResult.Type);
            Assert.AreEqual("Windows Test", pagedResult.OperatingSystem);
            Assert.AreEqual("11 Test", pagedResult.OSVersion);
            Assert.AreEqual("Intel i7 Test", pagedResult.Processor);
            Assert.AreEqual(17, pagedResult.RAMAmount);
            Assert.AreEqual("Unit test device", pagedResult.Description);
        }

        [TestMethod]
        public async Task EditDevice_InvalidOSAndRAMAmount()
        {
            _controller.ModelState.AddModelError("RAMAmount", "The RAM amount must be between 1 and 1024!");
            _controller.ModelState.AddModelError("OperatingSystem", "The Operating System is required!");

            var response = await _controller.EditDevice(9, new()
            {
                OperatingSystem = "",
                OSVersion = "11 Test",
                Processor = "Intel i7 Test",
                RAMAmount = 0,
                Description = "Unit test device"
            });

            var badRequestResult = response.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(badRequestResult.Value);

            var property = badRequestResult.Value.GetType().GetProperty("message");
            Assert.IsNotNull(property);

            var errors = property.GetValue(badRequestResult.Value) as IEnumerable<string>;

            Assert.IsNotNull(errors);
            Assert.AreEqual(2, errors.Count());
        }

        [TestMethod]
        public async Task AddDevice_SuccessTest()
        {
            var response = await _controller.AddDevice(new()
            {
                Name = "Device Test",
                Manufacturer = "TestBrand",
                Type = DeviceType.Laptop,
                OperatingSystem = "Windows",
                OSVersion = "11",
                Processor = "Intel i7",
                RAMAmount = 16,
                Description = "Unit test device"
            });
            var okResult = response.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.IsNotNull(okResult.Value);

            var pagedResult = okResult.Value as Device;

            Assert.IsNotNull(pagedResult);
            Assert.IsNotNull(pagedResult.Id);
            Assert.IsTrue(pagedResult.Id > 0);
            Assert.AreEqual("Device Test", pagedResult.Name);
            Assert.AreEqual("TestBrand", pagedResult.Manufacturer);
            Assert.IsNull(pagedResult.AssignedToUser);
            Assert.AreEqual(DeviceType.Laptop, pagedResult.Type);
            Assert.AreEqual("Windows", pagedResult.OperatingSystem);
            Assert.AreEqual("11", pagedResult.OSVersion);
            Assert.AreEqual("Intel i7", pagedResult.Processor);
            Assert.AreEqual(16, pagedResult.RAMAmount);
            Assert.AreEqual("Unit test device", pagedResult.Description);
        }

        [TestMethod]
        public async Task AddDevice_InvalidNameAndOSAndRAMAmount()
        {
            _controller.ModelState.AddModelError("RAMAmount", "The RAM amount must be between 1 and 1024!");
            _controller.ModelState.AddModelError("Name", "The name is required!");
            _controller.ModelState.AddModelError("OperatingSystem", "The Operating System is required!");

            var response = await _controller.AddDevice(new()
            {
                Manufacturer = "TestBrand",
                Type = DeviceType.Laptop,
                OperatingSystem = "",
                OSVersion = "11",
                Processor = "Intel i7",
                RAMAmount = 0,
                Description = "Unit test device"
            });

            var badRequestResult = response.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.IsNotNull(badRequestResult.Value);

            var property = badRequestResult.Value.GetType().GetProperty("message");
            Assert.IsNotNull(property);

            var errors = property.GetValue(badRequestResult.Value) as IEnumerable<string>;

            Assert.IsNotNull(errors);
            Assert.AreEqual(3, errors.Count());
        }
    }
}