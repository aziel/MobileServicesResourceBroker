using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;

namespace Test.WindowsAzure.Mobile.Service.ResourceBroker
{
    [TestClass]
    public class AzureBlobBrokerTests : TestBase
    {
        [TestMethod]
        public void CreateWithNullConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(null, new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void CreateWithEmptyConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("", new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void CreateWithWhitespaceConnectionString_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker(" ", new BlobParameters { Name = "blob", Container = "container" }));
        }

        [TestMethod]
        public void CreateWithNullParameters_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", null));
        }

        [TestMethod]
        public void CreateWithResourceParameters_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new ResourceParameters { Name = "blob" }));
        }

        [TestMethod]
        public void CreateWithNullContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = "blob", Container = null }));
        }

        [TestMethod]
        public void CreateWithEmptyContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = "blob", Container = "" }));
        }

        [TestMethod]
        public void CreateWithWhitespaceContainerName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = "blob", Container = " " }));
        }

        [TestMethod]
        public void CreateWithNullBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = null, Container = "container" }));
        }

        [TestMethod]
        public void CreateWithEmptyBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = "", Container = "container" }));
        }

        [TestMethod]
        public void CreateWithWhitespaceBlobName_ThrowsArgumentException()
        {
            this.ExpectException<ArgumentException>(() => new AzureBlobBroker("123", new BlobParameters { Name = " ", Container = "container" }));
        }



    }
}
