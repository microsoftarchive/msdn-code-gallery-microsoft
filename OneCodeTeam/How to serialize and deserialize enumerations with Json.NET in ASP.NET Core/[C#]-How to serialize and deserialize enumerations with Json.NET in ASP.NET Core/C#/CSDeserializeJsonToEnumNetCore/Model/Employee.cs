using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSDeserializeJsonToEnumNetCore.Model
{
    public class Employee
    {
        public string Name { get; set; }

        public Gender Gender { get; set; }
    }
}
