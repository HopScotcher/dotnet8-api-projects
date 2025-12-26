using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheOfficeApi.DTOs
{
    public class EmployeeQueryDto
    {
        public int? DepartmentId {get; set; } = null;
        public decimal? MinSalary {get; set;} = null;
        public decimal? MaxSalary {get; set;} = null;
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}