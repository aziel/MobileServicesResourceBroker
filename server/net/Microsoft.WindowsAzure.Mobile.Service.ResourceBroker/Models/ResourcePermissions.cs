using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.WindowsAzure.Mobile.Service.ResourceBroker.Models
{
    /// <summary>
    /// The possible resource access permissions.
    /// </summary>
    public enum ResourcePermissions
    {
        /// <summary>
        /// Read access only.
        /// </summary>
        None = 0,

        /// <summary>
        /// Read access only.
        /// </summary>
        Read = 1,

        /// <summary>
        /// Write access only.
        /// </summary>
        Write = 2,

        /// <summary>
        /// Read and write access only.
        /// </summary>
        ReadWrite = Read | Write
    }
}