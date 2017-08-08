using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Common.WepApi.Impl;
using WebApp.Globalization.Services;
using WebApp.Models.Dto;
using WebApp.Models.Services;

namespace WebApp.Common.WepApi
{
    public interface IUsersApiCanvas
    {

        ResultValue<UserCanvasDto> GetBySisId(string sisId);
    }
}