﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradeUnionCommittee.BLL.DTO.Children;
using TradeUnionCommittee.BLL.Enums;
using TradeUnionCommittee.BLL.Interfaces.Helpers;
using TradeUnionCommittee.BLL.Interfaces.Lists.Children;
using TradeUnionCommittee.BLL.Interfaces.SystemAudit;
using TradeUnionCommittee.Razor.Web.GUI.Controllers.Directory;
using TradeUnionCommittee.Razor.Web.GUI.Extensions;
using TradeUnionCommittee.ViewModels.ViewModels.Children;

namespace TradeUnionCommittee.Razor.Web.GUI.Controllers.Lists.Children
{
    public class HobbyChildrenController : Controller
    {
        private readonly IHobbyChildrenService _services;
        private readonly IDirectories _directories;
        private readonly IMapper _mapper;
        private readonly ISystemAuditService _systemAuditService;
        private readonly IHttpContextAccessor _accessor;
        private readonly IReferenceParentService _referenceParent;

        public HobbyChildrenController(IHobbyChildrenService services, IDirectories directories, IMapper mapper, ISystemAuditService systemAuditService, IHttpContextAccessor accessor, IReferenceParentService referenceParent)
        {
            _services = services;
            _mapper = mapper;
            _systemAuditService = systemAuditService;
            _accessor = accessor;
            _referenceParent = referenceParent;
            _directories = directories;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        [Authorize(Roles = "Admin,Accountant,Deputy")]
        public async Task<IActionResult> Index([Required] string id)
        {
            var result = await _services.GetAllAsync(id);
            var referenceParent = _referenceParent.GetHashIdEmployee(id, ReferenceParentType.Children);
            if (result.IsValid && referenceParent.IsValid)
            {
                ViewData["HashIdChildren"] = id;
                ViewData["HashIdEmployee"] = referenceParent.Result;
                return View(result.Result);
            }
            var errorsList = new List<string>();
            errorsList.AddRange(result.ErrorsList);
            errorsList.AddRange(referenceParent.ErrorsList);
            TempData["ErrorsList"] = errorsList;
            return View();
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        [Authorize(Roles = "Admin,Accountant,Deputy")]
        public async Task<IActionResult> Create([Required] string id)
        {
            ViewBag.Hobby = await _directories.GetHobby();
            return View(new CreateHobbyChildrenViewModel { HashIdChildren = id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Accountant,Deputy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateHobbyChildrenViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _services.CreateAsync(_mapper.Map<HobbyChildrenDTO>(vm));
                if (result.IsValid)
                {
                    await _systemAuditService.AuditAsync(User.GetEmail(), _accessor.GetIp(), Operations.Insert, Tables.HobbyChildrens);
                    return RedirectToAction("Index", new { id = vm.HashIdChildren });
                }
                TempData["ErrorsList"] = result.ErrorsList;
            }
            ViewBag.Hobby = await _directories.GetHobby();
            return View(vm);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        [HttpGet]
        [Authorize(Roles = "Admin,Accountant,Deputy")]
        public async Task<IActionResult> Update([Required] string id)
        {
            var result = await _services.GetAsync(id);
            if (result.IsValid)
            {
                ViewBag.Hobby = await _directories.GetHobby(result.Result.HashIdHobby);
                return View(_mapper.Map<UpdateHobbyChildrenViewModel>(result.Result));
            }
            TempData["ErrorsList"] = result.ErrorsList;
            return View();
        }

        [HttpPost, ActionName("Update")]
        [Authorize(Roles = "Admin,Accountant,Deputy")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateHobbyChildrenViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _services.UpdateAsync(_mapper.Map<HobbyChildrenDTO>(vm));
                if (result.IsValid)
                {
                    await _systemAuditService.AuditAsync(User.GetEmail(), _accessor.GetIp(), Operations.Update, Tables.HobbyChildrens);
                    return RedirectToAction("Index", new { id = vm.HashIdChildren });
                }
                TempData["ErrorsListConfirmed"] = result.ErrorsList;
            }
            ViewBag.Hobby = await _directories.GetHobby(vm.HashIdHobby);
            return View(vm);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete([Required] string id)
        {
            var result = await _services.DeleteAsync(id);
            if (result.IsValid)
            {
                await _systemAuditService.AuditAsync(User.GetEmail(), _accessor.GetIp(), Operations.Delete, Tables.HobbyChildrens);
            }
            return Ok(result);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------

        protected override void Dispose(bool disposing)
        {
            _services.Dispose();
            base.Dispose(disposing);
        }
    }
}