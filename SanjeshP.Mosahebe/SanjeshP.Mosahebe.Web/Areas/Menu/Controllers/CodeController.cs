using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SanjeshP.Mosahebe.Web.Areas.Menu.ViewModels.Code;
using SanjeshP.Mosahebe.Common;
using SanjeshP.Mosahebe.Data.Contracts.Menus;
using SanjeshP.Mosahebe.Data.Contracts.Users;
using SanjeshP.Mosahebe.Entities.Menu;
using SanjeshP.Mosahebe.WebFramework.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SanjeshP.Mosahebe.Web.Areas.Menu.Controllers
{
    [Area("Menu")]
    public class CodeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICompositeViewEngine _viewEngine;//
        private readonly ILogger<CodeController> _logger;//
        private readonly IUserRepository _userRepository;
        private readonly ICodeRepository _codeRepository;//
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IMemoryCache _memoryCache;

        public CodeController(IMapper mapper
                                , ICompositeViewEngine viewEngine
                                , ILogger<CodeController> logger
                                , IUserRepository userRepository
                                , ICodeRepository codeRepository
                                , IUserTokenRepository userTokenRepository
                                , IMemoryCache memoryCache)
        {
            _mapper = mapper;
            _viewEngine = viewEngine;
            _logger = logger;
            _userRepository = userRepository;
            _codeRepository = codeRepository;
            _userTokenRepository = userTokenRepository;
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CodeTree()
        {
            return PartialView("CodeTree");
        }

        #region دریافت فهرست کد به صورت درختی
        public async Task<ActionResult<string>> GetCodeTree(CancellationToken cancellationToken)
        {
            var curentUserToken = User.Identity.FindFirstValue("Token");
            var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(curentUserToken), cancellationToken);
            var user = await _userRepository.GetUserByIdAsync(token.UserId, cancellationToken);
            if (user == null)
            {
                return NotFound();
            }
            var userid = token.UserId;

            #region Convert ListCode to CodeViewModel for use jstree
            List<Code> listCodes = await _codeRepository.GetAllCodesAsync(cancellationToken);
            List<CodeTreeViewModel> codeTreeViewModel = new List<CodeTreeViewModel>();
            foreach (var item in listCodes)
            {
                if (item.ParentId == null)
                {
                    codeTreeViewModel.Add(new CodeTreeViewModel
                    {
                        id = item.Id,
                        parent = "#",
                        ShowCode = item.IsActive,
                        text = "کد اصلی : " + item.PrimaryCode + " _ " + "کد فرعی : " + item.SubCode + " _ " + item.Title,
                    });
                }
                else
                {
                    codeTreeViewModel.Add(new CodeTreeViewModel
                    {
                        id = item.Id,
                        parent = item.ParentId.ToString(),
                        ShowCode = item.IsActive,
                        text = "کد اصلی : " + item.PrimaryCode + " _ " + "کد فرعی : " + item.SubCode + " _ " + item.Title,
                    });
                }
            }
            #endregion

            #region Add options jstree
            foreach (var item in codeTreeViewModel)
            {

                var result = true;
                foreach (var item2 in codeTreeViewModel)
                {
                    if (item2.parent == item.id.ToString())
                    {
                        result = false;
                        break;
                    }
                }

                if (result)
                {
                    CodeTreeStateViewModel codeTreeStateViewModel = new CodeTreeStateViewModel();
                    codeTreeStateViewModel.selected = true;
                    codeTreeStateViewModel.opened = true;
                    codeTreeStateViewModel.disabled = !item.ShowCode;
                    item.state = codeTreeStateViewModel;
                    if (item.ShowCode == false)
                    {
                        ViewModels.Code.JStreeAttr jStreeAttr = new ViewModels.Code.JStreeAttr()
                        {
                            href = "",
                            title = "غیر فعال / عدم نمایش"
                        };
                        item.a_attr = jStreeAttr;
                    }
                }
                else
                {
                    CodeTreeStateViewModel codeTreeStateViewModel = new CodeTreeStateViewModel();
                    codeTreeStateViewModel.selected = false;
                    codeTreeStateViewModel.opened = false;
                    codeTreeStateViewModel.disabled = !item.ShowCode;
                    item.state = codeTreeStateViewModel;
                    if (item.ShowCode == false)
                    {
                        ViewModels.Code.JStreeAttr jStreeAttr = new ViewModels.Code.JStreeAttr()
                        {
                            href = "",
                            title = "غیر فعال / عدم نمایش"
                        };
                        item.a_attr = jStreeAttr;
                    }
                }
            }

            #endregion

            var json = JsonConvert.SerializeObject(codeTreeViewModel);
            return (json);
        }
        #endregion پایان

        #region Create Code
        public async Task<IActionResult> CreateCode(int parentId, CancellationToken cancellationToken)
        {
            var Code = await _codeRepository.GetByIdAsync(cancellationToken, parentId);
            if (Code == null)
            {
                return NotFound();
            }
            // ارسال اطلاعات به view
            ViewBag.ParentId = parentId;
            ViewBag.ParentTitle = Code.Title;


            var model = new CodeCreateViewModel
            {
                ParentId = Code.ParentId,
                PrimaryCode = Code.PrimaryCode,
            };
            return PartialView("CreateCode", model);
        }

        [HttpPost]
        public async Task<ApiResult> CreateCode(CodeCreateViewModel CodeCreateViewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

                var code = new Code
                {
                    Title = CodeCreateViewModel.Title,
                    ParentId = CodeCreateViewModel.ParentId,
                    StandardCode = CodeCreateViewModel.StandardCode,
                    PrimaryCode = CodeCreateViewModel.PrimaryCode,
                    SubCode = CodeCreateViewModel.SubCode,
                    AdditionalInformation = CodeCreateViewModel.AdditionalInformation,

                    CreatedBy = token.UserId,
                    HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString(),
                    IsActive = CodeCreateViewModel.IsActive,
                    CreatedDate = DateTime.Now
                };

                await _codeRepository.AddAsync(code, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ایجاد آزمون");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion End Create Code

        #region Edit Code
        [HttpGet]
        public async Task<IActionResult> EditCode(int id, CancellationToken cancellationToken)
        {
            // دریافت اطلاعات منو برای ویرایش
            var code = await _codeRepository.GetByIdAsync(cancellationToken, id);
            if (code == null)
            {
                return NotFound();
            }


            var model = new CodeEditViewModel
            {
                Id = code.Id,
                Title = code.Title,
                StandardCode = code.StandardCode,
                PrimaryCode = code.PrimaryCode,
                SubCode = code.SubCode,
                AdditionalInformation = code.AdditionalInformation,
                IsActive = code.IsActive,
            };
            return PartialView("EditCode", model);
        }

        [HttpPost]
        public async Task<ApiResult> EditCode(CodeEditViewModel CodeEditViewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

                // بروزرسانی اطلاعات منو
                var code = await _codeRepository.GetByIdAsync(cancellationToken, CodeEditViewModel.Id);
                if (code == null)
                {
                    return (ApiResult)NotFound();
                }

                // به روز رسانی مقادیر
                code.Title = CodeEditViewModel.Title;
                code.StandardCode = CodeEditViewModel.StandardCode;
                code.PrimaryCode = CodeEditViewModel.PrimaryCode;
                code.SubCode = CodeEditViewModel.SubCode;
                code.AdditionalInformation = CodeEditViewModel.AdditionalInformation;
                code.IsActive = CodeEditViewModel.IsActive;
                //Code.ModifiedBy = token.UserId;
                code.UpdatedDate = DateTime.Now;
                code.HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();

                await _codeRepository.UpdateAsync(code, cancellationToken);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در ویرایش کد");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion End Edit Code

        #region Delete Code
        [HttpGet]
        public async Task<IActionResult> DeleteCode(int id, CancellationToken cancellationToken)
        {
            // دریافت اطلاعات منو برای ویرایش
            var Code = await _codeRepository.GetByIdAsync(cancellationToken, id);
            if (Code == null)
            {
                return NotFound();
            }
            var model = new CodeDeleteViewModel
            {
                Id = Code.Id,
                Title = "کد اصلی : " + Code.PrimaryCode + " _ " + "کد فرعی : " + Code.SubCode + " _ " + Code.Title,
                ConfirmDelete = false,
            };
            return PartialView("DeleteCode", model);
        }

        [HttpPost]
        public async Task<ApiResult> DeleteCode(CodeDeleteViewModel deleteCodeViewModel, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            try
            {
                var currentUserToken = User.FindFirstValue("Token");
                var token = await _userTokenRepository.GetUserTokenByIdAsync(new Guid(currentUserToken), cancellationToken);

                // بروزرسانی اطلاعات منو
                var Code = await _codeRepository.GetByIdAsync(cancellationToken, deleteCodeViewModel.Id);
                if (Code == null)
                {
                    return (ApiResult)NotFound();
                }

                if (deleteCodeViewModel.ConfirmDelete == false)
                {
                    return new BadRequestObjectResult("جهت حذف تایید نمایید..");

                }

                var allCodes = await _codeRepository.GetAllCodesAsync(cancellationToken);
                var hasChildren = allCodes.Any(Code => Code.ParentId == deleteCodeViewModel.Id);

                if (hasChildren)
                {
                    return new BadRequestObjectResult("این کد دارای زیرمجموعه است و نمی‌توان آن را حذف کرد.");
                }


                Code.IsActive = false;
                Code.IsDeleted = true;
                Code.UpdatedDate = DateTime.Now;
                Code.HostIp = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
                await _codeRepository.UpdateAsync(Code, cancellationToken);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطا در حذف کد");
                return (ApiResult)StatusCode(500, "خطا در سرور");
            }
        }
        #endregion End Delete Code
    }
}
