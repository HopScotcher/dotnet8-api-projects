using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheOfficeApi.DTOs
{
    public class UpdateEmployeeDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary {get; set;}
        public int DepartmentId {get; set;}
    }
}