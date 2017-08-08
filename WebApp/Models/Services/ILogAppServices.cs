using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Models.Dto;

namespace WebApp.Models.Services
{
    public interface ILogAppServices
    {
        int SaveLog(LogDto log);
    }
}