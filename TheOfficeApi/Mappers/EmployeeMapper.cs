using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TheOfficeApi.DTOs;
using TheOfficeApi.Models;

namespace TheOfficeApi.Mappers
{
    public static class EmployeeMapper
    {
        public static EmployeeDto ToEmployeeDto(this Employee employeeModel)
        {
            return new EmployeeDto{
            Id = employeeModel.Id,
            FirstName = employeeModel.FirstName,
            LastName = employeeModel.LastName,
            Email = employeeModel.Email,
            Salary = employeeModel.Salary,
            // Department = employeeModel.Department.Select(dept => dept.ToEmployeeDepartmentDto()).ToList()
            Department = employeeModel.Department?.ToEmployeeDepartmentDto()

        };
    }

    public static DepartmentDto ToEmployeeDepartmentDto(this Department dept)
        {
            return new DepartmentDto
            {
                Id = dept.Id,
                Name = dept.Name,
                Code = dept.Code
            };
        }
    
    
    }
}