using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TheOfficeApi.DTOs;
using TheOfficeApi.Interfaces;
using TheOfficeApi.Mappers;
using TheOfficeApi.Models;

namespace TheOfficeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            if (!ModelState.IsValid)
            {
                BadRequest("An error occurred");
            }

            var departments = await _departmentService.GetAllAsync();

            return Ok(departments.Select(dept => dept.ToDepartmentDto()));
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDepartment([FromRoute] int id)
    {

            try
            {
            var dept = await _departmentService.DeleteAsync(id);

            if(dept == null)
            {
                return NotFound();
            }
            
            return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
       
    }

    [HttpPost]
    public async Task<IActionResult> CreateDepartment([FromBody] Department dept)
        {
            var createdDept = await _departmentService.CreateAsync(dept);

            var createdDeptDto = createdDept.ToDepartmentDto();
        
            return CreatedAtAction(nameof(GetDepartmentById), new {id = createdDeptDto.Id}, createdDeptDto);
        }
    

    [HttpGet("{id:int}")]

    public async Task<IActionResult> GetDepartmentById([FromRoute] int id)
        {
            var dept = await _departmentService.GetByIdAsync(id);

            if(dept == null)
            {
                return NotFound();
            }

            return Ok(dept.ToDepartmentDto());
        }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateDepartmentById([FromRoute] int id, UpdateDepartmentDto departmentDto)
        {
            var dept = await _departmentService.GetByIdAsync(id);

            if(dept == null)
            {
                return NotFound();
            }

            var updatedDept = await _departmentService.UpdateByIdAsync(id, departmentDto);

            if (updatedDept == null)
            {
                return BadRequest("Could not update department, try again later");
            }

            return Ok(updatedDept.ToDepartmentDto());
        }}
}
