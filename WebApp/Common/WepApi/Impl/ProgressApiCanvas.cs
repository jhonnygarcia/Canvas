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
    public class ProgressApiCanvas : BaseApiCanvas, IProgressApiCanvas
    {
        private const string API_URL = "api/v1/progress/";
        private readonly ILogAppServices _logService;
        public ProgressApiCanvas(ILogAppServices logService)
        {
            _logService = logService;
        }

        public ResultValue<ProgressCanvasDto> Get(int id)
        {
            var result = new ResultValue<ProgressCanvasDto>();
            var url = API_URL + id;
            var returnData = base.Get(url);
            var logData = returnData as LogDto;
            if (logData != null)
            {
                var idLog = _logService.SaveLog(logData);
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                return result;
            }
            result.Value = new ProgressCanvasDto
            {
                Id = returnData["id"],
                Completion = returnData["completion"],
                WorkflowState = returnData["workflow_state"],
                Url = returnData["url"]
            };
            return result;
        }
    }
}