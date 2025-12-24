using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TheOfficeApi.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string  FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName {get; set;} = string.Empty;

        [Required]
        [EmailAddress]
        public string Email {get; set;} =  string.Empty;

        [Column(TypeName ="decimal(18,2)")]
        public decimal Salary {get; set;}

        // foreign key rship; 1-to-many
        public int DepartmentId {get; set;}
        
        // navigational prop
        public Department? Department {get; set;} 

    }
}