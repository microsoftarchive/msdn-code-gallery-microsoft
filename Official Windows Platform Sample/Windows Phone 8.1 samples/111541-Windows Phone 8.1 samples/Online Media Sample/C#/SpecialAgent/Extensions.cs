using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Phone.SocialInformation;

namespace SpecialAgent
{
    internal static class Extensions
    {
        /// <summary>
        /// Safely marks operations complete
        /// </summary>
        /// <param name="operation">The operation to complete.</param>
        /// <param name="success">Completion status.</param>
        internal static void SafeNotifyCompletion(this ISocialOperation operation, bool success = true)
        {
            try
            {
                operation.NotifyCompletion(success);
            }
            catch
            {
                // Failures here are not actionable and uninteresting to log 
                // since that info is already present in OS logs
            }
        }
    }

}
