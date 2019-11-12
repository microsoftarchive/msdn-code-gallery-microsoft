using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSOneDriveAccess
{
    public enum HTTPMethod
    {
        Get,
        Post,
        Put,
        Delete,
        Head,
        Options,
        Patch
    }

    /// <summary>
    /// The following values are allowed for the type parameter.
    /// </summary>
    public enum OneDriveShareLinkType
    {
        /// <summary>
        /// Creates a read-only link to the item.
        /// </summary>
        view,
        /// <summary>
        /// Creates a read-write link to the item.
        /// </summary>
        edit,
        /// <summary>
        /// Creates an embeddable link to the item. This option is only available for OneDrive Personal.
        /// </summary>
        embed
    }

    /// <summary>
    /// The following values are allowed for the scope parameter. This is an optional parameter. If the scope parameter is not specified, the most permissive link available will be created.
    /// </summary>
    public enum OneDrevShareScopeType
    {
        /// <summary>
        /// Creates a link to the item accessible to anyone. Anonymous links may be disabled by the tenant administrator.
        /// </summary>
        anonymous,
        /// <summary>
        /// Creates a link to the item accessible within an organization. Organization link scope is not available for OneDrive Personal.
        /// </summary>
        organization
    }
}
