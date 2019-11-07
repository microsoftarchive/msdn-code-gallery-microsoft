using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRoleAPI.Models
{
    public sealed class InitializedModel
    {
        private static InitializedModel instance = null;

        public static InitializedModel Instance {
            get
            {
                return instance;
            }
        }

        private InitializedModel()
        {
        }

        private static readonly object sinlock = new object();

        public static void Initialization()
        {
            if (instance == null)
            {
                lock (sinlock)
                {
                    if (instance == null)
                    {
                        instance = new InitializedModel();
                    }
                }
            }
        }
    }
}