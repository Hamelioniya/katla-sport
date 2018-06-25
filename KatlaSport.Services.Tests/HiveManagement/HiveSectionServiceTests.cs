using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KatlaSport.DataAccess.ProductStoreHive;
using KatlaSport.Services.HiveManagement;
using Moq;
using Xunit;

namespace KatlaSport.Services.Tests.HiveManagement
{
    public class HiveSectionServiceTests
    {
        public HiveSectionServiceTests()
        {
            var x = MapperInitialize.Initialize();
        }

        [Fact]
        public void CreateHiveSectionServiceWithNullAsParametersTest()
        {
            Assert.Throws<ArgumentNullException>(() => new HiveSectionService(null, null));
        }

        [Fact]
        public async Task GetHiveSections_EmptyCollectionTest()
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveSectionService(context.Object, userContext.Object);

            var sections = await service.GetHiveSectionsAsync();

            Assert.Empty(sections);
        }

        [Fact]
        public async Task GetHiveSections_SetWithTwoElements_TwoReturnedTest()
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1 },
                new StoreHiveSection() { Id = 2 }
            });
            var service = new HiveSectionService(context.Object, userContext.Object);

            var sections = await service.GetHiveSectionsAsync();

            Assert.Equal(2, sections.Count);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetHiveSectionsWithHiveId_SetWithTwoElements_ZeroReturnedTest(int hiveId)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, StoreHiveId = 2 },
                new StoreHiveSection() { Id = 2, StoreHiveId = 2 }
            });
            var service = new HiveSectionService(context.Object, userContext.Object);

            var sections = await service.GetHiveSectionsAsync(hiveId);

            Assert.Empty(sections);
        }

        [Theory]
        [InlineData(1)]
        public async Task GetHiveSectionsWithHiveId_SetWithTwoElements_TwoReturnedTest(int hiveId)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, StoreHiveId = 1 },
                new StoreHiveSection() { Id = 2, StoreHiveId = 1 }
            });
            var service = new HiveSectionService(context.Object, userContext.Object);

            var sections = await service.GetHiveSectionsAsync(hiveId);

            Assert.Equal(2, sections.Count);
        }

        [Theory]
        [InlineData(2)]
        public async Task GetHiveSection_NoSuchSectionTest(int hiveSectionId)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1 }
            });
            var service = new HiveSectionService(context.Object, userContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () => await service.GetHiveSectionAsync(hiveSectionId));
        }

        [Theory]
        [InlineData(1)]
        public async Task GetHiveSection_SuccessfulTest(int hiveSectionId)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1 }
            });
            var service = new HiveSectionService(context.Object, userContext.Object);

            var section = await service.GetHiveSectionAsync(hiveSectionId);

            Assert.NotNull(section);
        }

        [Theory]
        [InlineData(1, true)]
        public void SetStatusAsync_NoSectionWithSuchIdTest(int id, bool status)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveSectionService(context.Object, userContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () => await service.SetStatusAsync(id, status));
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(1, false)]
        public async Task SetStatusAsync_SuccessfulTest(int id, bool status)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>() { new StoreHiveSection() { Id = 1, IsDeleted = false } });
            var service = new HiveSectionService(context.Object, userContext.Object);

            await service.SetStatusAsync(id, status);

            var hive = await service.GetHiveSectionAsync(id);
            Assert.Equal(status, hive.IsDeleted);
        }

        [Theory]
        [InlineData(1, "HiveSection", "11111")]
        public void CreateHiveSectionAsync_SuchHiveSectionAlreadyExistsTest(int hiveId, string name, string code)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection> { new StoreHiveSection() { Code = "11111" } });
            var service = new HiveSectionService(context.Object, userContext.Object);
            var request = new UpdateHiveSectionRequest
            {
                Name = name,
                Code = code,
            };

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () => await service.CreateHiveSectionAsync(hiveId, request));
        }

        [Theory]
        [InlineData(1, "HiveSection", "11111")]
        public async Task CreateHiveSectionAsync_SuccessfulTest(int hiveId, string name, string code)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Hives).ReturnsEntitySet(new List<StoreHive>() { new StoreHive() { Id = 1 } });
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveSectionService(context.Object, userContext.Object);
            var request = new UpdateHiveSectionRequest
            {
                Name = name,
                Code = code,
            };

            await service.CreateHiveSectionAsync(hiveId, request);

            var sections = await service.GetHiveSectionsAsync(hiveId);
            Assert.Single(sections);
        }

        [Theory]
        [InlineData(1, "HiveSection", "11112")]
        public void UpdateHiveSectionAsync_SuchSectionCodeAlreadyExistsTest(int id, string name, string code)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, Code = "11111" },
                new StoreHiveSection() { Id = 2, Code = "11112" }
            });
            var service = new HiveSectionService(context.Object, userContext.Object);
            var request = new UpdateHiveSectionRequest
            {
                Name = name,
                Code = code,
            };

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () => await service.UpdateHiveSectionAsync(id, request));
        }

        [Theory]
        [InlineData(3, "HiveSection", "11113")]
        public void UpdateHiveSectionAsync_NoHiveWithSuchIdTest(int id, string name, string code)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, Code = "11111" },
                new StoreHiveSection() { Id = 2, Code = "11112" }
            });
            var service = new HiveSectionService(context.Object, userContext.Object);
            var request = new UpdateHiveSectionRequest
            {
                Name = name,
                Code = code,
            };

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () => await service.UpdateHiveSectionAsync(id, request));
        }

        [Theory]
        [InlineData(1, "HiveSection", "11113")]
        public async Task UpdateHiveSectionAsync_SuccessfulTest(int id, string name, string code)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, Code = "11111" },
                new StoreHiveSection() { Id = 2, Code = "11112" }
            });
            var service = new HiveSectionService(context.Object, userContext.Object);
            var request = new UpdateHiveSectionRequest
            {
                Name = name,
                Code = code,
            };

            await service.UpdateHiveSectionAsync(id, request);

            var actualHive = await service.GetHiveSectionAsync(id);
            Assert.Equal(code, actualHive.Code);
        }

        [Theory]
        [InlineData(1)]
        public void DeleteHiveSectionAsync_NoSectionWithSuchIdTest(int id)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>());
            var service = new HiveSectionService(context.Object, userContext.Object);

            Assert.ThrowsAsync<RequestedResourceNotFoundException>(async () => await service.DeleteHiveSectionAsync(id));
        }

        [Theory]
        [InlineData(1)]
        public void DeleteHiveSectionAsync_IsDeletedFalseTest(int id)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, IsDeleted = false }
            });
            var service = new HiveSectionService(context.Object, userContext.Object);

            Assert.ThrowsAsync<RequestedResourceHasConflictException>(async () => await service.DeleteHiveSectionAsync(id));
        }

        [Theory]
        [InlineData(1)]
        public async Task DeleteHiveSectionAsync_SuccessfulTest(int id)
        {
            var context = new Mock<IProductStoreHiveContext>();
            var userContext = new Mock<IUserContext>();
            context.Setup(c => c.Sections).ReturnsEntitySet(new List<StoreHiveSection>()
            {
                new StoreHiveSection() { Id = 1, IsDeleted = true }
            });
            var service = new HiveSectionService(context.Object, userContext.Object);

            await service.DeleteHiveSectionAsync(id);

            var sections = await service.GetHiveSectionsAsync();
            Assert.Empty(sections);
        }
    }
}
