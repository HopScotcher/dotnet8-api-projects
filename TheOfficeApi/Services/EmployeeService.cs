using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TheOfficeApi.Data;
using TheOfficeApi.DTOs;
using TheOfficeApi.Interfaces;
using TheOfficeApi.Models;

namespace TheOfficeApi.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _dbContext;
        public EmployeeService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Employee?> CreateAsync(CreateEmployeeDto createEmployeeDto)
        {

            var employeeExists = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Email.ToLower() == createEmployeeDto.Email.ToLower());

            if(employeeExists != null)
            {
                throw new InvalidOperationException("This email already exists");
            }

            var department = await _dbContext.Departments.FindAsync(createEmployeeDto.DepartmentId);

            if(department == null)
            {
             throw new ArgumentException("Department Id does not exist");
            }

            var newEmployee = new Employee
            {
                FirstName = createEmployeeDto.FirstName,
                LastName = createEmployeeDto.LastName,
                Salary = createEmployeeDto.Salary,
                DepartmentId = createEmployeeDto.DepartmentId,
                Email = createEmployeeDto.Email

            };

            await _dbContext.Employees.AddAsync(newEmployee);

            await _dbContext.SaveChangesAsync();

            return newEmployee;
        }

        public async Task<Employee?> DeleteAsync(int id)
        {
             var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id);

             if(employee == null)
            {
                return null;
            }

            _dbContext.Remove(employee);
            await _dbContext.SaveChangesAsync();

            return employee;
        }

        public async Task<List<Employee>> GetAllAsync()
        {
        return await _dbContext.Employees.Include(e => e.Department).ToListAsync();
        }
 
        public async Task<Employee?> GetByIdAsync(int id)
        {
             var employee = await  _dbContext.Employees.Include(e => e.Department).FirstOrDefaultAsync(e => e.Id == id);
             
             if(employee == null)
            {
                return null;
            }

            return employee;
             }
        }
    }
