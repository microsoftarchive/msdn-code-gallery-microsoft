using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSPostComplexJsonASPNETCore.Models
{
    public class ItemGroup
    {
        public string Name { get; set; }

        public IList<GroupItem> Items { get; set; }
    }
}
