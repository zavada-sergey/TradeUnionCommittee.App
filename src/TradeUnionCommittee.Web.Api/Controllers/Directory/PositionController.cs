﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using TradeUnionCommittee.BLL.DTO;
using TradeUnionCommittee.BLL.Enums;
using TradeUnionCommittee.BLL.Interfaces.Directory;
using TradeUnionCommittee.BLL.Interfaces.SystemAudit;
using TradeUnionCommittee.ViewModels.ViewModels;
using TradeUnionCommittee.Web.Api.Extensions;

namespace TradeUnionCommittee.Web.Api.Controllers.Directory
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class PositionController : ControllerBase
    {
        private readonly IPositionService _services;
        private readonly ISystemAuditService _systemAuditService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _accessor;
        private readonly ILogger<PositionController> _logger;

        public PositionController(IPositionService services, ISystemAuditService systemAuditService, IMapper mapper, IHttpContextAccessor accessor, ILogger<PositionController> logger)
        {
            _services = services;
            _systemAuditService = systemAuditService;
            _mapper = mapper;
            _accessor = accessor;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAll")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin,Accountant,Deputy", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetAll()
        {
            //_logger.LogInformation($"Test Position API Controller: {JsonConvert.SerializeObject(await _services.GetAllAsync())}");
            return Ok(await _services.GetAllAsync());
        }

        [HttpGet]
        [Route("Get/{id}")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin,Accountant,Deputy", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Get([Required] string id)
        {
            var result = await _services.GetAsync(id);
            if (result.IsValid)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        [Route("Create")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin,Accountant,Deputy", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Create([FromBody] CreatePositionViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _services.CreateAsync(_mapper.Map<DirectoryDTO>(vm));
                if (result.IsValid)
                {
                    await _systemAuditService.AuditAsync(User.GetEmail(), _accessor.GetIp(), Operations.Insert, Tables.Position);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest();
        }

        [HttpPut]
        [Route("Update")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin,Accountant,Deputy", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Update([FromBody] UpdatePositionViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _services.UpdateAsync(_mapper.Map<DirectoryDTO>(vm));
                if (result.IsValid)
                {
                    await _systemAuditService.AuditAsync(User.GetEmail(), _accessor.GetIp(), Operations.Update, Tables.Position);
                    return Ok(result);
                }
                return BadRequest(result);
            }
            return BadRequest();
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        [MapToApiVersion("1.0")]
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> Delete([Required] string id)
        {
            var result = await _services.DeleteAsync(id);
            if (result.IsValid)
            {
                await _systemAuditService.AuditAsync(User.GetEmail(), _accessor.GetIp(), Operations.Delete, Tables.Position);
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}