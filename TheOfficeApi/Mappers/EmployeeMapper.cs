using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheOfficeApi.DTOs;
using TheOfficeApi.Models;

namespace TheOfficeApi.Mappers
{
    public static class EmployeeMapper
    {
        public static Employee ToEmployeeDto(this Employee employeeModel)
        {
            return new Employee{
            FirstName = employeeModel.FirstName,
            LastName = employeeModel.LastName,
            Email = employeeModel.Email,
            Salary = employeeModel.Salary,
            Department = employeeModel.Department.ToEmployeeDepartmentsDto()
        };
    }

    public static Department ToEmployeeDepartmentsDto(this Department dept)
        {
            return new Department
            {
                Id = dept.Id,
                Name = dept.Name,
                Code = dept.Code,
            };
        }
    
    
    }
}