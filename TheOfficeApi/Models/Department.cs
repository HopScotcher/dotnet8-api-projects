using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheOfficeApi.Models
{
    public class Department
    {
        [Key]
        public int Id {get; set;}
        [Required]
        public string Name {get; set;} = string.Empty;
        [Required]
        public string Code {get; set;} = string.Empty;

        public ICollection<Employee> Employees  {get; set;} = new List<Employee>();
        
    }
}