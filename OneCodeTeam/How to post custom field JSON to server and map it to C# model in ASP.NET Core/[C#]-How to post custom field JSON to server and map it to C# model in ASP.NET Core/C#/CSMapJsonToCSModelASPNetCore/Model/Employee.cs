using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace CSMapJsonToCSModelASPNetCore.Model
{
    public class Employee
    {
        [JsonProperty(propertyName: "employee_employee_id")]
        public Guid EmployeeId { get; set; }

        [JsonProperty(propertyName: "employee_name")]
        public string Name { get; set; }

        [JsonProperty(propertyName: "employee_age")]
        public string Age { get; set; }

        [JsonProperty(propertyName: "employee_department_name")]
        public string DepartmentName { get; set; }
    }
}
