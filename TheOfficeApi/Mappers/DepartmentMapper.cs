using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheOfficeApi.DTOs;
using TheOfficeApi.Models;

namespace TheOfficeApi.Mappers
{
    public static class DepartmentMapper
    {
        public static Department ToDepartmentDto(this Department department)
        {
            return new Department
            {
                Id = department.Id,
                Name = department.Name,
                Code = department.Code,
                Employees = department.Employees.Select(e => e.ToDepartmentEmployeesDto()).ToList()
            };
        }

        public static Employee ToDepartmentEmployeesDto(this Employee employeeModel)
        {
            return new Employee
            {
            Id = employeeModel.Id,
            FirstName = employeeModel.FirstName,
            LastName = employeeModel.LastName,
            Email = employeeModel.Email,
            Salary = employeeModel.Salary
             };
        }

         
    }
}