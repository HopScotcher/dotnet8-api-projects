using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace TheOfficeApi.DTOs
{
    public class CreateEmployeeDto
    {
        [Required]
        public string  FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName {get; set;} = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Range(1000, 1000_000)]
        public decimal Salary { get; set; }

        // 1-to-many rshp 
        [Required]
        public int DepartmentId {get; set;}
    }
}