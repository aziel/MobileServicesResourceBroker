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
        private bool initialized;
        private string connectionString;
        private BlobParameters blobParameters;
        private IStorageProvider storageProvider;

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

            this.connectionString = storageConnectionString;

            if (parameters == null)
            {
                throw new ArgumentNullException("parameters");
            }

            this.blobParameters = parameters as BlobParameters;

            if (this.blobParameters == null)
            {
                throw new ArgumentException("Expected a BlobParameters collection", "parameters");
            }

            if (string.IsNullOrWhiteSpace(this.blobParameters.Container))
            {
                throw new ArgumentException("The container name must not be null or empty", "parameters.Container");
            }

            if (string.IsNullOrWhiteSpace(this.blobParameters.Name))
            {
                throw new ArgumentException("The blob name must not be null or empty", "parameters.Name");
            }
        }

        /// <summary>
        /// Gets or sets the storage provider.
        /// </summary>
        public IStorageProvider StorageProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Generates the resource.
        /// </summary>
        /// <returns>Returns the resource.</returns>
        public override async Task<ResourceResponseToken> CreateResourceAsync()
        {
            this.Initialize();

            // Todo: is this necessary?
            if (!await this.storageProvider.CheckBlobContainerExistsAsync(this.blobParameters.Container))
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            return this.storageProvider.CreateBlob(this.blobParameters.Container, this.blobParameters.Name, this.blobParameters.Permissions, this.blobParameters.Expiration);
        }

        /// <summary>
        /// Initializes the contents of the class if this has not been done already.
        /// </summary>
        private void Initialize()
        {
            if (!this.initialized)
            {
                if (this.storageProvider == null)
                {
                    this.storageProvider = new StorageProvider();
                }

                this.storageProvider.Initialize(this.connectionString);

                this.initialized = true;
            }
        }
    }
}