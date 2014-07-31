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
        private const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=test;AccountKey=3w1OwI/N6dqvmN0Iaa0/y6zlqL81H42K/mfIbIIKeFQkNpHSNvOcnWpucvrX5rbKGm+WKEUxaOZikeTMWpXfxA==";

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
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, null));
        }

        [TestMethod]
        public void Create_WithResourceParameters_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new ResourceParameters { Name = "blob" }));
        }

        [TestMethod]
        public void Create_WithNullContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = null }));
        }

        [TestMethod]
        public void Create_WithEmptyContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "" }));
        }

        [TestMethod]
        public void Create_WithWhitespaceContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = " " }));
        }

        [TestMethod]
        public void Create_WithNullBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = null, Container = "container" }));
        }

        [TestMethod]
        public void Create_WithEmptyBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "", Container = "container" }));
        }

        [TestMethod]
        public void Create_WithWhitespaceBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(ConnectionString, new BlobParameters { Name = " ", Container = "container" }));
        }

        [TestMethod]
        public void CreateResourceAsync_ReturnsCorrectHostPath()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act.
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);

            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual(parts.HostName, "https://test.blob.core.windows.net/container/blob");
        }

        [TestMethod]
        public void CreateResourceAsync_WithReadPermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("r", parts.Value("sp"));
        }

        [TestMethod]
        public void CreateResourceAsync_WithWritePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Write, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("w", parts.Value("sp"));
        }

        [TestMethod]
        public void CreateResourceAsync_WithReadWritePermissions_ReturnsCorrectAccess()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.ReadWrite, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rw", parts.Value("sp"));
        }

        [TestMethod]
        public void CreateResourceAsync_WithReadAndWritePermissions_ReturnsCorrectAccess()
        {
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Write, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rw", parts.Value("sp"));
        }

        [TestMethod]
        public void CreateResourceAsync_WithReadAddUpdateDeletePermissions_ReturnsCorrectAccess()
        {
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("rw", parts.Value("sp"));
        }

        [TestMethod]
        public void CreateResourceAsync_WithAddUpdateDeletePermissions_ReturnsCorrectAccess()
        {
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            ResourceResponseToken token = broker.CreateResourceToken();

            // Assert.
            Assert.IsNotNull(token);
            SASParts parts = new SASParts(token.Uri);
            Assert.AreEqual("w", parts.Value("sp"));
        }

        [TestMethod]
        public void CreateResourceAsync_WithAddPermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void CreateResourceAsync_WithReadAddPermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void CreateResourceAsync_WithUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void CreateResourceAsync_WithReadUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void CreateResourceAsync_WithDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void CreateResourceAsync_WithReadDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void CreateResourceAsync_WithAddUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void CreateResourceAsync_WithReadAddUpdatePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Update, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void CreateResourceAsync_WithAddDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Add | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void CreateResourceAsync_WithReadAddDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Add | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void CreateResourceAsync_WithUpdateDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

        [TestMethod]
        public void CreateResourceAsync_WithReadUpdateDeletePermissions_ThrowsBadRequest()
        {
            // Setup
            AzureBlobBroker broker = new AzureBlobBroker(ConnectionString, new BlobParameters { Name = "blob", Container = "container", Permissions = ResourcePermissions.Read | ResourcePermissions.Update | ResourcePermissions.Delete, Expiration = DateTime.Now + TimeSpan.FromDays(1) });

            // Act
            this.ExpectHttpResponseException(HttpStatusCode.BadRequest, () => broker.CreateResourceToken());
        }

    }
}