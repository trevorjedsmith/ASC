using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Configuration;
using ASC.Web.DataAccess.Interfaces;
using ASC.Web.DataModels;
using ASC.Web.Interfaces;
using ASC.Web.Models;
using ASC.Web.Models.BaseTypes;
using ASC.Web.Models.ViewModels;
using ASC.Web.ServiceHub;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Infrastructure;
using Microsoft.Extensions.Options;

namespace ASC.Web.Controllers
{
  public class ServiceRequestController : BaseController
  {
    private readonly IServiceRequestOperations _operations;
    private readonly IMapper _mapper;
    private readonly IRepository<MasterDataValue> _masterDataValuesRepo;
    private readonly IRepository<MasterDataKey> _masterDataKeysRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IServiceRequestMessageOperations _messageOptions;
    private readonly IConnectionManager _signalRConnectionManager;
    private readonly IOptions<ApplicationSettings> _options;

    public ServiceRequestController(IServiceRequestOperations operations, 
      IMapper mapper, IRepository<MasterDataValue> masterDataValuesRepo, 
      IRepository<MasterDataKey> masterDataKeysRepo, 
      UserManager<ApplicationUser> userManager, IServiceRequestMessageOperations messageOptions,
      IConnectionManager signalRConnectionManager, IOptions<ApplicationSettings> options)
    {
      _operations = operations;
      _mapper = mapper;
      _masterDataValuesRepo = masterDataValuesRepo;
      _masterDataKeysRepo = masterDataKeysRepo;
      _userManager = userManager;
      _messageOptions = messageOptions;
      _signalRConnectionManager = signalRConnectionManager;
      _options = options;
    }

    [HttpGet]
    public async Task<IActionResult> ServiceRequest()
    {

      var masterDataVehicleTypeKey = await _masterDataKeysRepo.FindSingleByQuery(x => x.Name == "VehicleType");

      if (masterDataVehicleTypeKey == null)
        throw new NullReferenceException("Could not find Vehicle Types Key");

      ViewBag.VehicleTypes =
         await _masterDataValuesRepo.FindAllByQuery(x => x.MasterDataKeyId == masterDataVehicleTypeKey.Id);

      var masterDataVehicleNameKey = await _masterDataKeysRepo.FindSingleByQuery(x => x.Name == "VehicleName");

      if (masterDataVehicleNameKey == null)
        throw new NullReferenceException("Could not find Vehicle Names Key");

      ViewBag.VehicleNames =
        await _masterDataValuesRepo.FindAllByQuery(x => x.MasterDataKeyId == masterDataVehicleNameKey.Id);

      return View(new NewServiceRequestViewModel());

    }

    [HttpPost]
    public async Task<IActionResult> ServiceRequest(NewServiceRequestViewModel request)
    {
      if (!ModelState.IsValid)
      {

        var masterDataVehicleTypeKey = await _masterDataKeysRepo.FindSingleByQuery(x => x.Name == "VehicleType");

        if (masterDataVehicleTypeKey == null)
          throw new NullReferenceException("Could not find Vehicle Types Key");

        ViewBag.VehicleTypes =
          await _masterDataValuesRepo.FindAllByQuery(x => x.MasterDataKeyId == masterDataVehicleTypeKey.Id);

        var masterDataVehicleNameKey = await _masterDataKeysRepo.FindSingleByQuery(x => x.Name == "VehicleName");

        if (masterDataVehicleNameKey == null)
          throw new NullReferenceException("Could not find Vehicle Names Key");

        ViewBag.VehicleNames =
          await _masterDataValuesRepo.FindAllByQuery(x => x.MasterDataKeyId == masterDataVehicleNameKey.Id);
        return View(request);
      }
      // Map the view model to Azure model
      var serviceRequest = _mapper.Map<NewServiceRequestViewModel,
        ServiceRequest>(request);
      // Set RowKey, PartitionKey, RequestedDate, Status properties
      serviceRequest.RequestedDate = request.RequestedDate;
      serviceRequest.Status = Status.New.ToString();
      //User has to be authenticated to get here
      serviceRequest.Customer = HttpContext.User.Identity.Name;
      await _operations.CreateServiceRequestAsync(serviceRequest);
      return RedirectToAction("ServiceRequest", "ServiceRequest");
    }

    [HttpGet]
    public async Task<IActionResult> ServiceRequestDetails(Guid id)
    {
      var serviceRequestDetails = await _operations.GetServiceRequestByRowKey(id);
      if (serviceRequestDetails == null)
        return NotFound();

      //Access check
      if (HttpContext.User.IsInRole(Roles.Engineer.ToString()) && 
          serviceRequestDetails.ServiceEngineer != HttpContext.User.Identity.Name)
      {
        throw new UnauthorizedAccessException();
      }

      if (HttpContext.User.IsInRole(Roles.User.ToString()) 
          && serviceRequestDetails.Customer != HttpContext.User.Identity.Name)
      {
        throw new UnauthorizedAccessException();
      }

      var masterDataVehicleTypeKey = await _masterDataKeysRepo.FindSingleByQuery(x => x.Name == "VehicleType");

      if (masterDataVehicleTypeKey == null)
        throw new NullReferenceException("Could not find Vehicle Types Key");

      var vt = await _masterDataValuesRepo.FindAllByQuery(x => x.MasterDataKeyId == masterDataVehicleTypeKey.Id);

      ViewBag.VehicleTypes = vt;
     

      var masterDataVehicleNameKey = await _masterDataKeysRepo.FindSingleByQuery(x => x.Name == "VehicleName");

      if (masterDataVehicleNameKey == null)
        throw new NullReferenceException("Could not find Vehicle Names Key");

      var vn = await _masterDataValuesRepo.FindAllByQuery(x => x.MasterDataKeyId == masterDataVehicleNameKey.Id);

      ViewBag.VehicleNames = vn;

      ViewBag.Status = Enum.GetValues(typeof(Status)).Cast<Status>().Select(v => v.ToString());

      ViewBag.ServiceEngineers = await _userManager.GetUsersInRoleAsync(Roles.Engineer.ToString());

      return View(new ServiceRequestDetailViewModel
      {
        ServiceRequest = _mapper.Map<ServiceRequest,UpdateServiceRequestViewModel>(serviceRequestDetails)
      });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateServiceRequestDetails(UpdateServiceRequestViewModel serviceRequest)
    {
      var originalServiceRequest = await _operations.GetServiceRequestByRowKey(serviceRequest.Id);

      originalServiceRequest.RequestedServices = serviceRequest.RequestedServices;

      //Update Status only if user is an engineer or admin
      //Or Customer only if the status is pending customer approval

      if (HttpContext.User.IsInRole(Roles.Admin.ToString()) || HttpContext.User.IsInRole(Roles.Engineer.ToString()) 
          || HttpContext.User.IsInRole(Roles.User.ToString()) && originalServiceRequest.Status == Status.PendingCustomerApproval.ToString())
      {
        originalServiceRequest.Status = serviceRequest.Status;
      }

      //Update service engineer if user is an admin
      if (HttpContext.User.IsInRole(Roles.Admin.ToString()))
      {
        originalServiceRequest.ServiceEngineer = serviceRequest.ServiceEngineer;
      }

      await _operations.UpdateServiceRequestAsync(originalServiceRequest);

      return RedirectToAction("ServiceRequestDetails", "ServiceRequest", new {Id = serviceRequest.Id});
    }

    #region Signal R

    [HttpGet]
    public async Task<IActionResult> ServiceRequestMessage(Guid serviceRequestId)
    {
      var messages = await _messageOptions.GetServiceRequestMessageAsync(serviceRequestId);
      return Json(
        messages.OrderByDescending(p => p.MessageDate));
    }

    [HttpPost]
    public async Task<IActionResult> CreateServiceRequestMessage(ServiceRequestMessage message)
    {

      if (string.IsNullOrWhiteSpace(message.Message) || message.ServiceRequestId == Guid.Empty)
        return Json(false);

      // Get Service Request details
      var serviceRequesrDetails = await _operations.GetServiceRequestByRowKey(message.ServiceRequestId.Value);

      // Populate message details
      message.FromEmail = HttpContext.User.Identity.Name;
      message.FromDisplayName = HttpContext.User.Identity.Name;
      message.MessageDate = DateTime.UtcNow;
      message.Id = Guid.NewGuid();

      // Get Customer and Service Engineer names
      var customer = await _userManager.FindByEmailAsync(serviceRequesrDetails.Customer);
      var customerName = customer.UserName;
      var serviceEngineerName = string.Empty;

      if (!string.IsNullOrWhiteSpace(serviceRequesrDetails.ServiceEngineer))
      {
        serviceEngineerName = (await _userManager.FindByEmailAsync(serviceRequesrDetails.
          ServiceEngineer)).UserName;
      }

      var adminName = (await _userManager.FindByEmailAsync(_options.Value.AdminEmail)).
        UserName;

      await _messageOptions.CreateServiceRequestMessageAsync(message);

      var users = new List<string> { customerName, adminName };
      if (!string.IsNullOrWhiteSpace(serviceEngineerName))
      {
        users.Add(serviceEngineerName);
      }

      // Broadcast the message to all clients asscoaited with Service Request
      _signalRConnectionManager.GetHubContext<ServiceMessagesHub>()
        .Clients
        .Users(users)
        .publishMessage(message);
      //return true

      return Ok(true);
    }

    #endregion
    }
}
