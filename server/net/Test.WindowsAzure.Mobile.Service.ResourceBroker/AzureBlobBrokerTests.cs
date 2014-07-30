using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Providers;
using Moq;

namespace Test.WindowsAzure.Mobile.Service.ResourceBroker
{
    [TestClass]
    public class AzureBlobBrokerTests : TestBase
    {
        [TestMethod]
        public void Create_WithNullConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(null, new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void Create_WithEmptyConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("", new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void Create_WithWhitespaceConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(" ", new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void Create_WithNullParameters_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", null));
        }

        [TestMethod]
        public void Create_WithResourceParameters_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new ResourceParameters { Name = "blob" }));
        }

        [TestMethod]
        public void Create_WithNullContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = "blob", Container = null }));
        }

        [TestMethod]
        public void Create_WithEmptyContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = "blob", Container = "" }));
        }

        [TestMethod]
        public void Create_WithWhitespaceContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = "blob", Container = " " }));
        }

        [TestMethod]
        public void Create_WithNullBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = null, Container = "container" }));
        }

        [TestMethod]
        public void Create_WithEmptyBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = "", Container = "container" }));
        }

        [TestMethod]
        public void Create_WithWhitespaceBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = " ", Container = "container" }));
        }

        [TestMethod]
        public void Create_WithValidParameters_Succeeds()
        {
            AzureBlobBroker broker = new AzureBlobBroker("123", new BlobParameters { Name = " ", Container = "container", Permissions = ResourcePermissions.ReadWrite, Expiration = DateTime.Now + TimeSpan.FromDays(1) });
            Assert.IsNotNull(broker);
        }

        [TestMethod]
        public void CreateResourceAsync_WithNonExistantBlob_Throws()
        {
            // Setup
            var mockStorageProvider = new Mock<IStorageProvider>();
            mockStorageProvider.Setup(foo => foo.CheckBlobContainerExistsAsync("container")).Returns(Task.FromResult<bool>(false));

            AzureBlobBroker broker = new AzureBlobBroker("123", new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.ReadWrite, Expiration = DateTime.Now + TimeSpan.FromDays(1) });
            broker.StorageProvider = mockStorageProvider.Object;

            // Act.
            this.ExpectHttpResponseException(HttpStatusCode.Forbidden, () => broker.CreateResourceAsync().Wait());
        }


        [TestMethod]
        public void CreateResourceAsync_WithValidParameters_Succeeds()
        {
            // Setup
            ResourcePermissions permissions = ResourcePermissions.ReadWrite;
            DateTime expiration = DateTime.Now + TimeSpan.FromDays(1);
            var mockStorageProvider = new Mock<IStorageProvider>();
            mockStorageProvider.Setup(foo => foo.CheckBlobContainerExistsAsync("container")).Returns(Task.FromResult<bool>(true));
            mockStorageProvider.Setup(foo => foo.CreateBlob("container", "blob", permissions, expiration)).Returns(new ResourceResponseToken { Uri = "http://123" });

            AzureBlobBroker broker = new AzureBlobBroker("123", new BlobParameters { Name = "blob", Container = "container", Permissions = permissions, Expiration = expiration });
            broker.StorageProvider = mockStorageProvider.Object;

            // Act.
            ResourceResponseToken response = broker.CreateResourceAsync().Result;

            // Assert.
            Assert.IsNotNull(response);
            Assert.AreEqual("http://123", response.Uri);
        }
    }
}