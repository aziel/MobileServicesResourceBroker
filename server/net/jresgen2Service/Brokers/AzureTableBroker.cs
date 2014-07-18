namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Brokers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models;

    /// <summary>
    /// Generates tokens or connection strings for a table resource.
    /// </summary>
    public class AzureTableBroker : AzureResourceBroker
    {
        private string storageConnectionString;

        /// <summary>
        /// Initializes a new instance of the AzureTableBroker class.
        /// </summary>
        /// <param name="storageConnectionString">The Azure storage connection string.</param>
        /// <param name="parameters">The optional parameters.</param>
        public AzureTableBroker(string storageConnectionString, ResourceParameters parameters)
            : base(parameters)
        {
            if (string.IsNullOrWhiteSpace(storageConnectionString))
            {
                throw new ArgumentException("storageConnectionString is invalid");
            }

            this.storageConnectionString = storageConnectionString;
        }

        /// <summary>
        /// Generates the resource.
        /// </summary>
        /// <returns>Returns the resource.</returns>
        public override Task<ResponseToken> CreateResourceAsync()
        {
            throw new NotImplementedException();
        }
    }
}
