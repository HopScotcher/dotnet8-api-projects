using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheOfficeApi.Models;

namespace TheOfficeApi.Interfaces
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllAsync();
        Task<Department> CreateAsync(Department dept);
        Task<Department?> DeleteAsync(int id);

        Task<Department?> GetByIdAsync(int id);
    }
}