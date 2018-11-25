using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.DataModels;
using ASC.Web.Interfaces;
using ASC.Web.Models.ViewModels;
using ASC.Web.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ASC.Web.Controllers
{
  [Authorize(Roles = "Admin")]
  public class MasterDataController : BaseController
  {
    private readonly IMasterDataOperations _masterData;
    private readonly IMapper _mapper;

    public MasterDataController(IMasterDataOperations masterData, IMapper mapper)
    {
      _masterData = masterData;
      _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> MasterKeys()
    {
      var masterKeys = await _masterData.GetAllMasterKeysAsync();
      var masterKeysViewModel = _mapper.Map<List<MasterDataKey>, List<MasterDataKeyViewModel>>(masterKeys);

      // Hold all Master Keys in session
      HttpContext.Session.SetSession("MasterKeys", masterKeysViewModel);

      return View(new MasterKeysViewModel
      {
        MasterKeys = masterKeysViewModel == null ? null : masterKeysViewModel.ToList(),
        IsEdit = false
      });

    }

    [HttpGet]
    public async Task<IActionResult> MasterValues()
    {
      // Get All Master Keys and hold them in ViewBag for Select tag
      var masterKeys = await _masterData.GetAllMasterKeysAsync();
      ViewBag.MasterKeys = masterKeys;
      return View(new MasterValuesViewModel
      {
        MasterValues = new List<MasterDataValueViewModel>(),
        IsEdit = false
      });
    }

    [HttpGet]
    public async Task<IActionResult> MasterValuesByKey(Guid key)
    {
      // Get Master values based on master key.
      var data = await _masterData.GetAllMasterValuesByKeyAsync(key);
      return Json(new { data = data });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MasterValues(bool isEdit, MasterDataValueViewModel
      masterValue)
    {
      if (!ModelState.IsValid)
      {
        return Json("Error");
      }
      var masterDataValue = _mapper.Map<MasterDataValueViewModel,
        MasterDataValue>(masterValue);
      if (isEdit)
      {
        // Update Master Value
        await _masterData.UpdateMasterValueAsync(masterDataValue);
      }
      else
      {
        // Insert Master Value
        await _masterData.InsertMasterValueAsync(masterDataValue);
      }
      return Json(true);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MasterKeys(MasterKeysViewModel masterKeys)
    {
      masterKeys.MasterKeys = HttpContext.Session.GetSession<List<MasterDataKeyViewModel>>("MasterKeys");
      if (!ModelState.IsValid)
      {
        return View(masterKeys);
      }
      var masterKey = _mapper.Map<MasterDataKeyViewModel, MasterDataKey>(masterKeys.
        MasterKeyInContext);
      if (masterKeys.IsEdit)
      {
        // Update Master Key
        await _masterData.UpdateMasterKeyAsync(masterKey);
      }
      else
      {
        // Insert Master Key
        await _masterData.InsertMasterKeyAsync(masterKey);
      }
      return RedirectToAction("MasterKeys");
    }
  }
}
