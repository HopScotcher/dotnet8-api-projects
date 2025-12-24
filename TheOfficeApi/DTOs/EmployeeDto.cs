using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheOfficeApi.DTOs
{
    
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DepartmentDto? Department {get; set;}
    }

    public class ToDepartmentEmployeesDto
    {
        public int Id {get ; set;}
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
    }
}