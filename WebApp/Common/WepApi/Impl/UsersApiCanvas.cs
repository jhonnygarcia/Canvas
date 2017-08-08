using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebApp.App_Start;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Globalization.Services;
using WebApp.Models.Dto;
using WebApp.Models.Model.Entity;
using WebApp.Models.Services;
using WebApp.Parameters;

namespace WebApp.Common.WepApi.Impl
{
    public class UsersApiCanvas : BaseApiCanvas, IUsersApiCanvas
    {
        private const string API_URL = "api/v1/users/";
        private readonly ILogAppServices _logService;
        public UsersApiCanvas(ILogAppServices logService)
        {
            _logService = logService;
        }
        public ResultValue<UserCanvasDto> GetBySisId(string sisId)
        {
            var result = new ResultValue<UserCanvasDto>();
            var url = API_URL + "sis_user_id:" + sisId;
            var returnData = base.Get(url);
            var logData = returnData as LogDto;
            if (logData != null)
            {
                if (logData.StatusCode != Convert.ToInt32(HttpStatusCode.NotFound))
                {
                    var idLog = _logService.SaveLog(logData);
                    result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                    return result;
                }
            }
            else
            {
                result.Value = new UserCanvasDto
                {
                    Id = returnData["id"],
                    Name = returnData["name"],
                    SisId = returnData["sis_user_id"],
                    LoginId = returnData["login_id"]
                };
            }
            return result;
        }
    }
}