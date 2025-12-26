using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheOfficeApi.DTOs;
using TheOfficeApi.Models;

namespace TheOfficeApi.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllAsync();
        Task<Employee?> CreateAsync(CreateEmployeeDto createEmployeeDto);
        Task<Employee?> UpdateByIdAsync(int id, UpdateEmployeeDto updateEmployeeDto);
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee?> DeleteAsync(int id);

    }
}