using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UNIR.Servicios.Integracion.Negocio.ClasesDto;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;

namespace WebApp.Models.Services
{
    public interface IGestorAppServices
    {
        ResultList<EstudioDto> GetEstudios(string searchText, int pageIndex, int? pageCount);
        ResultList<AsignaturaDto> GetAsignaturas(string searchText, int pageIndex, int? pageCount, int idEstudio);
    }
}