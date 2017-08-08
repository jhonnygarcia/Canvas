using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Models.Dto;
using WebApp.Parameters;

namespace WebApp.Models.Services
{
    public interface ICanvasExtendAppServices
    {
        ResultValue<AccountExtendDto> GetAccountExted(int id);
        ResultValue<CourseExtendDto> GetCourseExted(int id);
        ResultValue<bool> VerificarDatos(int accountId);
        ResultValue<AccountExtendDto> UpdateAccountExtend(int id);
        ResultList<PeriodoActivoDto> GetPeriodoActivos(string searchText, int pageIndex, int? pageCount, int accountId);
        ResultValue<AccountGenerateDto> GetAccountPeriodo(int id);
        ResultValue<bool> VerificarPeriodosNoLectivos(PeriodoNoLectivosSaveParameters model);
        ResultValue<AccountGenerateDto> GuardarPeriodosNoLectivos(PeriodoNoLectivosSaveParameters model);
        ResultList<AccountGenerateDto> GetAccountGenerates(string searchText, int pageIndex, int? pageCount,
            int accountId);
    }
}