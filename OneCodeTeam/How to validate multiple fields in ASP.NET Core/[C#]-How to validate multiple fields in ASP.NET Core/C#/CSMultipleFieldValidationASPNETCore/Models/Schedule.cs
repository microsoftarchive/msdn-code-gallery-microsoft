using CSMultipleFieldValidationASPNETCore.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CSMultipleFieldValidationASPNETCore.Models
{
    public class Schedule
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        [BeforeThan(nameof(EndTime),ErrorMessage = "StartTime should before than EndTime")]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }
    }
}
