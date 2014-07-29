using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Providers;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers
{
    /// <summary>
    /// Generates tokens or connection strings for a BLOB resource.
    /// </summary>
    public class AzureBlobBroker : AzureResourceBroker
    {
        private BlobParameters blobParameters;
        private IStorageProvider storage = new StorageProvider();

        /// <summary>
        /// Initializes a new instance of the AzureBlobBroker class.
        /// </summary>
        /// <param name="storageConnectionString">The Azure storage connection string.</param>
        /// <param name="parameters">The optional parameters.</param>
        public AzureBlobBroker(string storageConnectionString, ResourceParameters parameters)
            : base(parameters)
        {
            if (string.IsNullOrWhiteSpace(storageConnectionString))
            {
                throw new ArgumentException("storageConnectionString is invalid");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            this.blobParameters = parameters as BlobParameters;

            if (this.blobParameters == null)
            {
                throw new ArgumentException("Expected a BlobParameters collection", "parameters");
            }

            this.storage.Initialize(storageConnectionString);
        }

        /// <summary>
        /// Generates the resource.
        /// </summary>
        /// <returns>Returns the resource.</returns>
        public override async Task<ResourceResponseToken> CreateResourceAsync()
        {
            // Todo: is this necessary?
            if (!await this.storage.CheckBlobContainerExistsAsync(this.blobParameters.Container))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            return this.storage.CreateBlob(this.blobParameters.Container, this.blobParameters.Name, this.blobParameters.Permissions, this.blobParameters.Expiration);
        }
    }
}