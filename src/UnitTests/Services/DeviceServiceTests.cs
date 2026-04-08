using BackendMarshProject.Data;
using BackendMarshProject.Enums;
using BackendMarshProject.Exceptions;
using BackendMarshProject.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTests.Services
{
    [TestClass]
    public class DeviceServiceTests
    {
        private DeviceService _service;
        private Mock<LLMService> _mockLLMService;
        private AppDbContext _appDbContext;
        private IDbContextTransaction _transaction;

        private int _deviceId;

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

            _service = new DeviceService(_appDbContext, _mockLLMService.Object);
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
        public async Task GetAllDevices_ShouldReturnFirst5Devices()
        {
            var result = await _service.GetAllDevices(new()
            {
                PageNumber = 1,
                PageSize = 5,
            });

            Assert.IsNotNull(result.Data);
            Assert.IsNotNull(result.Metadata);
            Assert.AreEqual(5, result.Data.Count());
        }

        [TestMethod]
        public async Task GetAllDevices_ShouldReturn0Devices()
        {
            var result = await _service.GetAllDevices(new()
            {
                PageNumber = 100,
                PageSize = 5,
            });

            Assert.IsNotNull(result.Data);
            Assert.IsNotNull(result.Metadata);
            Assert.AreEqual(0, result.Data.Count());
        }

        [TestMethod]
        public async Task GetAllDevices_ShouldReturnSelectedWithAssignedUserDevice()
        {
            var result = await _service.GetDeviceById(5);

            Assert.IsNotNull(result);

            Assert.AreEqual(5, result.Id);
            Assert.AreEqual("Pixel 8 Pro", result.Name);
            Assert.AreEqual("Google", result.Manufacturer);
            Assert.IsNotNull(result.AssignedToUser);
            Assert.AreEqual(1, result.AssignedToUser.Id);
            Assert.AreEqual(DeviceType.Phone, result.Type);
            Assert.AreEqual("Android", result.OperatingSystem);
            Assert.AreEqual("14.0", result.OSVersion);
            Assert.AreEqual("Tensor G3", result.Processor);
            Assert.AreEqual(12, result.RAMAmount);
            Assert.AreEqual("Stock Android testing for frontend team", result.Description);
        }

        [TestMethod]
        public async Task GetAllDevices_ShouldReturnSelectedWithoutAssignedUserDevice()
        {
            var result = await _service.GetDeviceById(9);

            Assert.IsNotNull(result);

            Assert.AreEqual(9, result.Id);
            Assert.AreEqual("Surface Pro 9", result.Name);
            Assert.AreEqual("Microsoft", result.Manufacturer);
            Assert.IsNull(result.AssignedToUser);
            Assert.AreEqual(DeviceType.Tablet, result.Type);
            Assert.AreEqual("Windows", result.OperatingSystem);
            Assert.AreEqual("11 Pro", result.OSVersion);
            Assert.AreEqual("Intel i5-1235U", result.Processor);
            Assert.AreEqual(16, result.RAMAmount);
            Assert.AreEqual("Portable testing for Windows Touch apps", result.Description);
        }

        [TestMethod]
        public async Task GetAllDevices_ShouldThrowAnError()
        {

            var result = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
            {
                await _service.GetDeviceById(9999);
            });

            Assert.AreEqual("The device no longer exists in the system.", result.Message);
        }

        [TestMethod]
        public async Task GetMyAndUnassignedDevices_ShouldReturnFirst5Devices()
        {
            var result = await _service.GetMyAndUnassignedDevices(new()
            {
                PageNumber = 1,
                PageSize = 5,
            }, 1);

            Assert.IsNotNull(result.Data);
            Assert.IsNotNull(result.Metadata);
            Assert.IsNotNull(result.Data.First().AssignedToUser);
            Assert.AreEqual(5, result.Data.Count());
        }

        [TestMethod]
        public async Task GetMyAndUnassignedDevices_ShouldReturn0Devices()
        {
            var result = await _service.GetMyAndUnassignedDevices(new()
            {
                PageNumber = 101,
                PageSize = 5,
            }, 1);

            Assert.IsNotNull(result.Data);
            Assert.IsNotNull(result.Metadata);
            Assert.AreEqual(0, result.Data.Count());
        }

        [TestMethod]
        public async Task AddNewDevice_SuccessTest()
        {
            var result = await _service.AddNewDevice(new()
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
            Assert.IsNotNull(result.Id);

            result = await _service.GetDeviceById(result.Id);

            Assert.IsNotNull(result.Id);
            Assert.IsTrue(result.Id > 0);
            Assert.AreEqual("Device Test", result.Name);
            Assert.AreEqual("TestBrand", result.Manufacturer);
            Assert.IsNull(result.AssignedToUser);
            Assert.AreEqual(DeviceType.Laptop, result.Type);
            Assert.AreEqual("Windows", result.OperatingSystem);
            Assert.AreEqual("11", result.OSVersion);
            Assert.AreEqual("Intel i7", result.Processor);
            Assert.AreEqual(16, result.RAMAmount);
            Assert.AreEqual("Unit test device", result.Description);
        }

        [TestMethod]
        public async Task AddNewDevice_AddDuplicateDevice()
        {
            var result = await Assert.ThrowsExceptionAsync<BadRequestException>(async () =>
            {
                await _service.AddNewDevice(new()
                {
                    Name = "ThinkPad X1 Carbon",
                    Manufacturer = "Lenovo",
                    Type = DeviceType.Laptop,
                    OperatingSystem = "Windows",
                    OSVersion = "11 Pro",
                    Processor = "Intel i7-1370P",
                    RAMAmount = 32,
                    Description = "Standard issue for Backend devs"
                });
            });

            Assert.AreEqual("This device already registred in the system.", result.Message);
        }

        [TestMethod]
        public async Task AddNewDevice_AddAnotherTypeDevice()
        {
            var result = await Assert.ThrowsExceptionAsync<BadRequestException>(async () =>
            {
                await _service.AddNewDevice(new()
                {
                    Name = "Device Test",
                    Manufacturer = "TestBrand",
                    Type = (DeviceType)10,
                    OperatingSystem = "Windows",
                    OSVersion = "11",
                    Processor = "Intel i7",
                    RAMAmount = 16,
                    Description = "Unit test device"
                });
            });

            Assert.AreEqual("Invalid type. Must be Laptop, Phone, or Tablet.", result.Message);
        }

        [TestMethod]
        public async Task UpdateDevice_SuccessTest()
        {
            var result = await _service.UpdateDevice(9, new()
            {
                OperatingSystem = "Windows Test",
                OSVersion = "11 Test",
                Processor = "Intel i7 Test",
                RAMAmount = 17,
                Description = "Unit test device"
            });
            Assert.IsNotNull(result.Id);

            result = await _service.GetDeviceById(result.Id);

            Assert.IsNotNull(result.Id);
            Assert.AreEqual(9, result.Id);
            Assert.AreEqual("Surface Pro 9", result.Name);
            Assert.AreEqual("Microsoft", result.Manufacturer);
            Assert.IsNull(result.AssignedToUser);
            Assert.AreEqual(DeviceType.Tablet, result.Type);
            Assert.AreEqual("Windows Test", result.OperatingSystem);
            Assert.AreEqual("11 Test", result.OSVersion);
            Assert.AreEqual("Intel i7 Test", result.Processor);
            Assert.AreEqual(17, result.RAMAmount);
            Assert.AreEqual("Unit test device", result.Description);
        }

        [TestMethod]
        public async Task UpdateDevice_NoExistDevice()
        {
            var result = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
            {
                await _service.UpdateDevice(9999, new()
                {
                    OperatingSystem = "Windows Test",
                    OSVersion = "11 Test",
                    Processor = "Intel i7 Test",
                    RAMAmount = 17,
                    Description = "Unit test device"
                });
            });

            Assert.AreEqual("The device no longer exists in the system.", result.Message);
        }

        [TestMethod]
        public async Task DeleteDevice_NoExistDevice()
        {
            var result = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
            {
                await _service.DeleteDevice(9999);
            });

            Assert.AreEqual("The device no longer exists in the system.", result.Message);
        }

        [TestMethod]
        public async Task DeleteDevice_SuccessTest()
        {
            await _service.DeleteDevice(9);

            var result = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
            {
                await _service.GetDeviceById(9999);
            });

            Assert.AreEqual("The device no longer exists in the system.", result.Message);
        }
    }
}
