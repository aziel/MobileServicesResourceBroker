using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Providers
{
    /// <summary>
    /// Abstracts the Azure storage provider.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Initializes the StorageProvider class.
        /// </summary>
        /// <param name="connectionString">The storage connection string.</param>
        void Initialize(string connectionString);

        /// <summary>
        /// Determines whether a container with the given name exists.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <returns>true if the container exists; false otherwise.</returns>
        Task<bool> CheckBlobContainerExistsAsync(string containerName);

        /// <summary>
        /// Creates a blob with the given name, permissions, and expiration.
        /// </summary>
        /// <param name="containerName">The name of the container to create the blob within.</param>
        /// <param name="blobName">The name of the blob to create.</param>
        /// <param name="permissions">The permissions to apply to the blob.</param>
        /// <param name="expiration">The expiration time for the blob.</param>
        /// <returns>Returns the blob's URI access string.</returns>
        ResourceResponseToken CreateBlob(string containerName, string blobName, ResourcePermissions permissions, DateTime? expiration = null);
    }
}
