using System.Collections.Generic;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Parameters;

namespace WebApp.Common.WepApi
{
    public interface IProgressApiCanvas
    {
        ResultValue<ProgressCanvasDto> Get(int id);
    }
}