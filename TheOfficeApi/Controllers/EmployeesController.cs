using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TheOfficeApi.DTOs;
using TheOfficeApi.Interfaces;
using TheOfficeApi.Mappers;
using TheOfficeApi.Services;

namespace TheOfficeApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEmployees([FromQuery] EmployeeQueryDto query)
        {
            var employees =  await _employeeService.GetAllAsync(query);
 
            var employeeDto =  employees.Select(e => e.ToEmployeeDto()).ToList();

            return Ok(employeeDto);    
        
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEmployeeById([FromRoute] int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if(employee == null)
            {
                return NotFound();
            }

            return Ok(employee.ToEmployeeDto());
        }
        

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto createEmployeeDto)
        {
            try
            {
               var employee = await _employeeService.CreateAsync(createEmployeeDto);

            return CreatedAtAction(nameof(GetEmployeeById), new {id = employee.Id}, employee.ToEmployeeDto());  

            }catch(InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
           
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] int id, UpdateEmployeeDto updateEmployeeDto)
        {
            try
            {
                var updatedEmployee = _employeeService.UpdateByIdAsync(id, updateEmployeeDto);

                if(updatedEmployee == null)
                {
                    return NotFound();
                }

                return Ok(updatedEmployee);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);  
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] int id)
        {
            var employee = await _employeeService.DeleteAsync(id);

            if(employee == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}