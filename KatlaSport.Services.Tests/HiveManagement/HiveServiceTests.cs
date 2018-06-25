using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    public class HiveServiceTests
    {
        public HiveServiceTests()
        {
            var x = MapperInitialize.Initialize();
        }

        [Fact]
        public void CreateHiveServiceWithNullAsParametersTest()
        {
            Assert.Throws<ArgumentNullException>(() => new HiveService(null, null));
        }

        [Fact]
        public async Task GetHivesAsync_EmptyCollectionTest()
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveService(context.Object, userContext.Object);

            var hives = await service.GetHivesAsync();

            Assert.Empty(hives);
        }

        [Fact]
        public async Task GetHivesAsync_SetWithTwoElements_TwoReturnedTest()
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>() { new StoreHive() { Id = 1, Sections = null }, new StoreHive() { Id = 2, Sections = null } });
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveService(context.Object, userContext.Object);

            var hives = await service.GetHivesAsync();

            Assert.Equal(2, hives.Count);
        }

        [Theory]
        [InlineData(1)]
        public void GetHiveAsync_NoSuchHiveIdTest(int id)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveService(context.Object, userContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () => await service.GetHiveAsync(id));
        }

        [Theory]
        [InlineData(1)]
        public async Task GetHiveAsync_SuccessfulTest(int id)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>() { new StoreHive() { Id = 1 } });
            var service = new HiveService(context.Object, userContext.Object);

            var hive = await service.GetHiveAsync(id);

            Assert.NotNull(hive);
        }

        [Theory]
        [InlineData("Hive", "11111", "Address")]
        public void CreateHiveAsync_SuchHiveAlreadyExistsTest(string name, string code, string address)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>() { new StoreHive() { Code = "11111" } });
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveService(context.Object, userContext.Object);
            var request = new UpdateHiveRequest
            {
                Name = name,
                Code = code,
                Address = address
            };

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () => await service.CreateHiveAsync(request));
        }

        [Theory]
        [InlineData("Hive", "11111", "Address")]
        public async Task CreateHiveAsync_SuccessfulTest(string name, string code, string address)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveService(context.Object, userContext.Object);
            var request = new UpdateHiveRequest
            {
                Name = name,
                Code = code,
                Address = address
            };

            await service.CreateHiveAsync(request);

            var hives = await service.GetHivesAsync();
            Assert.Single(hives);
        }

        [Theory]
        [InlineData(1, "Hive", "11111", "Address")]
        public void UpdateHiveAsync_SuchHiveCodeAlreadyExistsTest(int id, string name, string code, string address)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>() { new StoreHive() { Id = 1, Code = "11111" } });
            var service = new HiveService(context.Object, userContext.Object);
            var request = new UpdateHiveRequest
            {
                Name = name,
                Code = code,
                Address = address
            };

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () => await service.UpdateHiveAsync(id, request));
        }

        [Theory]
        [InlineData(1, "Hive", "11111", "Address")]
        public void UpdateHiveAsync_NoHiveWithSuchIdTest(int id, string name, string code, string address)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveService(context.Object, userContext.Object);
            var request = new UpdateHiveRequest
            {
                Name = name,
                Code = code,
                Address = address
            };

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () => await service.UpdateHiveAsync(id, request));
        }

        [Theory]
        [InlineData(1, "Hive", "11112", "Address")]
        public async Task UpdateHiveAsync_SuccessfulTest(int id, string name, string code, string address)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>() { new StoreHive() { Id = 1, Code = "11111" } });
            var service = new HiveService(context.Object, userContext.Object);
            var request = new UpdateHiveRequest
            {
                Name = name,
                Code = code,
                Address = address
            };

            await service.UpdateHiveAsync(id, request);

            var actualHive = await service.GetHiveAsync(id);
            Assert.Equal(code, actualHive.Code);
        }

        [Theory]
        [InlineData(1)]
        public void DeleteHiveAsync_NoHiveWithSuchIdTest(int id)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveService(context.Object, userContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () => await service.DeleteHiveAsync(id));
        }

        [Theory]
        [InlineData(1)]
        public void DeleteHiveAsync_IsDeletedFalseTest(int id)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>() { new StoreHive() { Id = 1, IsDeleted = false } });
            var service = new HiveService(context.Object, userContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () => await service.DeleteHiveAsync(id));
        }

        [Theory]
        [InlineData(1)]
        public async Task DeleteHiveAsync_SuccessfulTest(int id)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>() { new StoreHive() { Id = 1, IsDeleted = true } });
            var service = new HiveService(context.Object, userContext.Object);

            await service.DeleteHiveAsync(id);

            var hives = await service.GetHivesAsync();
            Assert.Empty(hives);
        }

        [Theory]
        [InlineData(1, true)]
        public void SetStatusAsync_NoHiveWithSuchIdTest(int id, bool status)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>());
            var service = new HiveService(context.Object, userContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () => await service.SetStatusAsync(id, status));
        }

        [Theory]
        [InlineData(1, true)]
        public async Task SetStatusAsync_SuccessfulTest(int id, bool status)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>() { new StoreHive() { Id = 1, IsDeleted = false } });
            var service = new HiveService(context.Object, userContext.Object);

            await service.SetStatusAsync(id, status);

            var hive = await service.GetHiveAsync(id);
            Assert.Equal(status, hive.IsDeleted);
        }
    }
}
