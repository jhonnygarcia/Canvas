
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApp.Architecture.ApiControllers;
using WebApp.Common.WepApi;
using WebApp.Models.Services;
using WebApp.Parameters;

namespace WebApp.ApiControllers
{
    [Authorize]
    [RoutePrefix("api/v1/canvas-extend")]
    public class CanvasExtendController : ApiControllerBase
    {
        private readonly ICanvasExtendAppServices _canvasExtend;
        public CanvasExtendController(ICanvasExtendAppServices canvasExtend)
        {
            _canvasExtend = canvasExtend;
        }

        [HttpGet]
        [Route("accounts/{id}")]
        public IHttpActionResult GetAccountExtend(int id)
        {
            var result = _canvasExtend.GetAccountExted(id);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpGet]
        [Route("courses/{id}")]
        public IHttpActionResult GetCourseExtend(int id)
        {
            var result = _canvasExtend.GetCourseExted(id);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpGet]
        [Route("accounts/{id}/verificar-datos")]
        public IHttpActionResult VerificarDatos(int id)
        {
            var result = _canvasExtend.VerificarDatos(id);
            return Ok(result);
        }
        [HttpPut]
        [Route("accounts/{id}")]
        public IHttpActionResult UpdateAccountExtend(int id)
        {
            var result = _canvasExtend.UpdateAccountExtend(id);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpGet]
        [Route("accounts/{accountId}/periodo-activos")]
        public IHttpActionResult GetPeriodosLectivos(string searchText, int pageIndex, int? pageCount, int accountId)
        {
            var result = _canvasExtend.GetPeriodoActivos(searchText, pageIndex, pageCount, accountId);
            if (!result.HasErrors)
            {
                var elements = result.Elements.Select(a => new
                {
                    a.Id,
                    Descripcion = a.Nombre
                }).ToList();
                return OkPagedList(elements, result.TotalElements, result.TotalPages);
            }
            return ResultWithMessages(result);
        }
        [HttpGet]
        [Route("accounts/{id}/periodo")]
        public IHttpActionResult GetAccountPeriodo(int id)
        {
            var result = _canvasExtend.GetAccountPeriodo(id);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpPost]
        [Route("accounts/{id}/verificar-periodos-no-lectivos")]
        public IHttpActionResult VerificarPeriodosNoLectivos([FromUri] int id, [FromBody] PeriodoNoLectivosSaveParameters model)
        {
            model.AccountId = id;
            var result = _canvasExtend.VerificarPeriodosNoLectivos(model);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpPost]
        [Route("accounts/{id}/periodos-no-lectivos")]
        public IHttpActionResult GuardarPeriodosNoLectivos([FromUri] int id, [FromBody] PeriodoNoLectivosSaveParameters model)
        {
            model.AccountId = id;
            var result = _canvasExtend.GuardarPeriodosNoLectivos(model);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpGet]
        [Route("accounts-generates")]
        public IHttpActionResult GetAccountGenerates(string searchText, int pageIndex, int? pageCount, int accountId)
        {
            var result = _canvasExtend.GetAccountGenerates(searchText, pageIndex, pageCount, accountId);
            if (!result.HasErrors)
            {
                var elements = result.Elements.Select(a => new
                {
                    a.Id,
                    Descripcion = a.Name
                }).ToList();
                return OkPagedList(elements, result.TotalElements, result.TotalPages);
            }
            return ResultWithMessages(result);
        }
    }
}