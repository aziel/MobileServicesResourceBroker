namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    /// <summary>
    /// Generates tokens or connection strings for a BLOB resource.
    /// </summary>
    public class AzureBlobBroker : AzureResourceBroker
    {
        private BlobParameters blobParameters;
        private string storageConnectionString;

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

            this.storageConnectionString = storageConnectionString;

            this.blobParameters = parameters as BlobParameters;

            if (this.blobParameters == null)
            {
                throw new ArgumentException("Expected a BlobParameters collection", "parameters");
            }
        }

        /// <summary>
        /// Generates the resource.
        /// </summary>
        /// <returns>Returns the resource.</returns>
        public override async Task<ResourceResponseToken> CreateResourceAsync()
        {
            // Set up the storage account connection.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(this.storageConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Set up the container connection and make sure the container exists. (todo: is this necessary?)
            CloudBlobContainer container = blobClient.GetContainerReference(this.blobParameters.Container);
            if (!await container.ExistsAsync())
            {
                throw new HttpResponseException(HttpStatusCode.Forbidden);
            }

            // Set up the SAS constraints.
            SharedAccessBlobPolicy sasConstraints = GetSasConstraints();

            // Get a reference to the blob.
            CloudBlockBlob blob = container.GetBlockBlobReference(this.blobParameters.Name);

            // Get the blob SAS.
            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            // Append this to the URI.
            return new ResourceResponseToken { Uri = blob.Uri + sasBlobToken };
        }

        private SharedAccessBlobPolicy GetSasConstraints()
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();

            // Start time.
            sasConstraints.SharedAccessStartTime = DateTimeOffset.Now - TimeSpan.FromMinutes(5);

            // Expiration.
            if (this.blobParameters.Expiration != DateTime.MaxValue)
            {
                sasConstraints.SharedAccessExpiryTime = this.blobParameters.Expiration;
            }

            // Permissions.
            sasConstraints.Permissions = SharedAccessBlobPermissions.None;
            if ((this.blobParameters.Permissions & ResourcePermissions.Read) == ResourcePermissions.Read)
            {
                sasConstraints.Permissions |= SharedAccessBlobPermissions.Read;
            }

            if ((this.blobParameters.Permissions & ResourcePermissions.Write) == ResourcePermissions.Write)
            {
                sasConstraints.Permissions |= SharedAccessBlobPermissions.Write;
            }

            return sasConstraints;
        }
    }
}
