using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

//using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using URAuth.Data;
using URAuth.Model;

namespace URAuth.Controller
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EmployeController:ControllerBase
    {
        private readonly URDBContext _context;
        private readonly IMapper _mapper;
        public EmployeController(URDBContext context){
            _context = context;
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("GetEmploye")]
        public async Task<ActionResult<IEnumerable<EmployeeMaster>>>GetEmployeeall(){
            return await _context.employeeMasters.ToListAsync();
        }
    }
}