using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApp.Architecture.ApiControllers;
using WebApp.Common.WepApi;
using WebApp.Models.Services;

namespace WebApp.ApiControllers
{
    [Authorize]
    [RoutePrefix("api/v1/gestor")]
    public class GestorController : ApiControllerBase
    {
        private readonly IGestorAppServices _gestor;
        public GestorController(IGestorAppServices gestor)
        {
            _gestor = gestor;
        }
        [HttpGet]
        [Route("estudios")]
        public IHttpActionResult GetEstudios(string searchText, int pageIndex, int? pageCount)
        {
            var result = _gestor.GetEstudios(searchText, pageIndex, pageCount);
            if (!result.HasErrors)
            {
                var elements = result.Elements.Select(a => new
                {
                    Id = a.idEstudio,
                    Descripcion = a.sNombreEstudio
                }).ToList();
                return OkPagedList(elements, result.TotalElements, result.TotalPages);
            }
            return ResultWithMessages(result);
        }
        [HttpGet]
        [Route("asignaturas")]
        public IHttpActionResult GetAsignaturas(string searchText, int pageIndex, int? pageCount, int idEstudio)
        {
            var result = _gestor.GetAsignaturas(searchText, pageIndex, pageCount, idEstudio);
            if (!result.HasErrors)
            {
                var elements = result.Elements.Select(a => new
                {
                    Id = a.idAsignatura,
                    Descripcion = a.sNombreAsignatura
                }).ToList();
                return OkPagedList(elements, result.TotalElements, result.TotalPages);
            }
            return ResultWithMessages(result);
        }
    }
}
