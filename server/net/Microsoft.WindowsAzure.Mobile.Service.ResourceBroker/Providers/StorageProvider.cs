using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Providers
{
    /// <summary>
    /// Provides access to Azure Storage accounts.
    /// </summary>
    public class StorageProvider : IStorageProvider
    {
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;

        /// <summary>
        /// Initializes a new instance of the StorageProvider class.
        /// </summary>
        public StorageProvider()
        {
        }

        /// <summary>
        /// Initializes the StorageProvider class.
        /// </summary>
        /// <param name="connectionString">The storage connection string.</param>
        public void Initialize(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Cannot be null or empty", "connectionString");
            }

            this.storageAccount = CloudStorageAccount.Parse(connectionString);
        }

        /// <summary>
        /// Determines whether a container with the given name exists.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <returns>true if the container exists; false otherwise.</returns>
        public async Task<bool> CheckBlobContainerExistsAsync(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("must not be null or empty", "containerName");
            }

            CloudBlobContainer container = this.BlobClient.GetContainerReference(containerName);
            return await container.ExistsAsync();
        }

        /// <summary>
        /// Creates a blob with the given name, permissions, and expiration.
        /// </summary>
        /// <param name="containerName">The name of the container to create the blob within.</param>
        /// <param name="blobName">The name of the blob to create.</param>
        /// <param name="permissions">The permissions to apply to the blob.</param>
        /// <param name="expiration">The expiration time for the blob.</param>
        /// <returns>Returns the blob's URI access string.</returns>
        public ResourceResponseToken CreateBlob(string containerName, string blobName, ResourcePermissions permissions, DateTime? expiration = null)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("must not be null or empty", "containerName");
            }

            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException("must not be null or empty", "containerName");
            }

            // Set up the container connection.
            CloudBlobContainer container = this.BlobClient.GetContainerReference(containerName);

            // Set up the SAS constraints.
            SharedAccessBlobPolicy sasConstraints = GetBlobSasConstraints(permissions, expiration);

            // Get a reference to the blob.
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            // Get the blob SAS.
            string sasBlobToken = blob.GetSharedAccessSignature(sasConstraints);

            // Append this to the URI.
            return new ResourceResponseToken { Uri = blob.Uri + sasBlobToken };
        }

        /// <summary>
        /// Creates SAS constraints from the given parameters.
        /// </summary>
        /// <param name="permissions">The permission set.</param>
        /// <param name="expiration">The expiration time.</param>
        /// <returns>Returns the access policy.</returns>
        private static SharedAccessBlobPolicy GetBlobSasConstraints(ResourcePermissions permissions, DateTime? expiration)
        {
            SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy();

            // Set the start time to five minutes in the past.
            sasConstraints.SharedAccessStartTime = DateTimeOffset.Now - TimeSpan.FromMinutes(5);

            // Expiration.
            if (expiration != null)
            {
                sasConstraints.SharedAccessExpiryTime = expiration.Value;
            }

            // Permissions.
            sasConstraints.Permissions = SharedAccessBlobPermissions.None;
            if ((permissions & ResourcePermissions.Read) == ResourcePermissions.Read)
            {
                sasConstraints.Permissions |= SharedAccessBlobPermissions.Read;
            }

            if ((permissions & ResourcePermissions.Write) == ResourcePermissions.Write)
            {
                sasConstraints.Permissions |= SharedAccessBlobPermissions.Write;
            }

            return sasConstraints;
        }

        /// <summary>
        /// Gets the blob client.
        /// </summary>
        private CloudBlobClient BlobClient
        {
            get
            {
                if (this.blobClient == null)
                {
                    this.blobClient = this.storageAccount.CreateCloudBlobClient();
                }

                return this.blobClient;
            }
        }
    }
}
