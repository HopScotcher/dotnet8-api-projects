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
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _dbContext;
        public DepartmentService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Department> CreateAsync(Department dept)
        {
            var newDepartment = new Department
            {
                Name = dept.Name,
                Code = dept.Code
            };

            await _dbContext.Departments.AddAsync(newDepartment);
            await _dbContext.SaveChangesAsync();

            return newDepartment;
        }

        public async Task<Department?> DeleteAsync(int id)
        {
            var department = await _dbContext.Departments.Include(dept => dept.Employees).FirstOrDefaultAsync(dept => dept.Id == id);

            if(department == null)
            {
                return null;
            }

            if (department.Employees.Any())
            {
                throw new InvalidOperationException("Cannot delete department with employees");
            }

            _dbContext.Departments.Remove(department);
            await _dbContext.SaveChangesAsync();

            return department;

        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _dbContext.Departments.Include(dept => dept.Employees).ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
             var dept = await _dbContext.Departments.Include(dept => dept.Employees).FirstOrDefaultAsync(dept => dept.Id == id);

             if(dept == null)
            {
                return null;
            }

            return dept;
        }

        public async Task<Department?> UpdateByIdAsync(int id, UpdateDepartmentDto departmentDto)
        {
            var dept = await _dbContext.Departments.FirstOrDefaultAsync(dept => dept.Id == id);

            if(dept == null)
            {
                return null;
            }

            dept.Name = departmentDto.Name;
            dept.Code = departmentDto.Code;

            return dept;
        }
    }
}