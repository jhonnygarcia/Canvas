using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using UNIR.Servicios.Integracion.Negocio.ClasesDto;
using WebApp.App_Start;
using WebApp.Architecture;
using WebApp.Architecture.ApiControllers;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Common.WepApi;
using WebApp.Globalization.Services;
using WebApp.Models.Dto;
using WebApp.Models.Services;
using WebApp.Parameters;

namespace WebApp.ApiControllers
{
    [Authorize]
    [RoutePrefix("api/v1/canvas")]
    public class CanvasController : ApiControllerBase
    {
        private readonly ICanvasAppServices _canvas;
        private readonly IJobService _jobService;
        public CanvasController(ICanvasAppServices canvas, IJobService jobService)
        {
            _canvas = canvas;
            _jobService = jobService;
        }

        [HttpGet]
        [Route("accounts/{accountId}/sub-accounts-courses")]
        public IHttpActionResult GetSubAccountsCourses(int accountId)
        {
            var result = _canvas.GetSubAccountsCourses(accountId);
            if (!result.HasErrors)
            {
                return OkList(result.Elements);
            }
            return ResultWithMessages(result);
        }
        [HttpGet]
        [Route("accounts/{id}")]
        public IHttpActionResult GetAccount(int id)
        {
            var result = _canvas.GetAccount(id);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpGet]
        [Route("accounts/{accountId}/courses")]
        public IHttpActionResult GetCourses(int accountId)
        {
            var result = _canvas.GetCourses(accountId);
            if (!result.HasErrors)
            {
                var elements = result.Elements.Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.SisId,
                    c.StartDate,
                    c.EndDate,
                    c.TotalStudents,
                    Url = c.HtmlUrl
                }).ToList();
                return Ok(elements);
            }
            return ResultWithMessages(result);
        }
        [HttpPut]
        [Route("accounts/{id}")]
        public IHttpActionResult UpdateAccount([FromUri] int id, [FromBody] AccountSaveParameters param)
        {
            param.Id = id;
            var result = _canvas.UpdateAccount(param);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpPut]
        [Route("courses/{id}")]
        public IHttpActionResult UpdateCourse([FromUri] int id, [FromBody] CourseSaveParameters param)
        {
            param.Id = id;
            var result = _canvas.UpdateCourse(param);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpPost]
        [Route("accounts")]
        public IHttpActionResult GenerarPeriodo(AccountSaveParameters param)
        {
            var result = _canvas.GenerarPeriodo(param);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpPost]
        [Route("accounts/{accountId}/migrar-cursos")]
        public IHttpActionResult MigrarCursos(int accountId)
        {
            var progressId = _jobService.CreateProgress();
            _jobService.EnqueueProcess(() => _canvas.MigrarCursos(accountId, progressId), progressId);
            return Ok(progressId);
        }
        [HttpGet]
        [Route("progress/{id}")]
        public IHttpActionResult GetProgress(int id)
        {
            var result = _canvas.GetProgress(id);
            if (!result.HasErrors)
            {
                return Ok(new
                {
                    result.Value.Id,
                    result.Value.Completion,
                    result.Value.Finished
                });
            }
            return ResultWithMessages(result);
        }
        [HttpGet]
        [Route("progress/{progressId}/migration")]
        public IHttpActionResult GetResultMigration(int progressId)
        {
            var result = _canvas.GetResult(progressId);
            if (!result.HasErrors)
            {
                var returnData = new
                {
                    Courses = result.Value.Courses.Select(c => new
                    {
                        c.Value.Id,
                        c.Value.Name,
                        c.Value.SisId,
                        c.Value.HtmlUrl,
                        c.Code
                    }).ToList(),
                    Stundents = result.Value.Stundents.Select(s => new
                    {
                        Id = s.Value.idPersona,
                        Nombres = s.Value.sNombrePersona,
                        Apellidos = s.Value.sApellidosPersona,
                        Nif = s.Value.sNIF,
                        s.Code
                    }).ToList(),
                    Assignments = result.Value.Assignments.Select(a => new
                    {
                        a.Value.Id,
                        a.Value.Name,
                        a.Value.HtmlUrl,
                        a.Code
                    }).ToList()
                };
                return Ok(returnData);
            }
            return ResultWithMessages(result);
        }

        [HttpGet]
        [Route("accounts/{accountId}/migrations")]
        public IHttpActionResult GetMigrations(int accountId)
        {
            var result = _canvas.GetMigrations(accountId);
            if (!result.HasErrors)
            {
                var elements = result.Elements.OrderByDescending(a => a.Id)
                    .Select(m => new
                    {
                        m.Id,
                        m.Inicio,
                        m.Fin
                    }).ToList();
                return Ok(elements);
            }
            return ResultWithMessages(result);
        }
        [HttpGet]
        [Route("migrations/{id}")]
        public IHttpActionResult GetMigration(int id)
        {
            var result = _canvas.GetMigration(id);
            if (!result.HasErrors)
            {
                var returnData = new
                {
                    Courses = result.Value.Courses.Select(c => new
                    {
                        c.Value.Id,
                        c.Value.Name,
                        c.Value.SisId,
                        c.Value.HtmlUrl,
                        c.Code
                    }).ToList(),
                    Stundents = result.Value.Stundents.Select(s => new
                    {
                        Id = s.Value.idPersona,
                        Nombres = s.Value.sNombrePersona,
                        Apellidos = s.Value.sApellidosPersona,
                        Nif = s.Value.sNIF,
                        s.Code
                    }).ToList(),
                    Assignments = result.Value.Assignments.Select(a => new
                    {
                        a.Value.Id,
                        a.Value.Name,
                        a.Value.HtmlUrl,
                        a.Code
                    }).ToList()
                };
                return Ok(returnData);
            }
            return ResultWithMessages(result);
        }
        [HttpPost]
        [Route("accounts/{accountId}/migrar-cursos/{courseId}")]
        public IHttpActionResult MigrarCurso(int accountId, int courseId)
        {
            var progressId = _jobService.CreateProgress();
            _jobService.EnqueueProcess(() => _canvas.MigrarCurso(accountId, courseId, progressId), progressId);
            return Ok(progressId);
        }
        [HttpGet]
        [Route("accounts/{accountId}/has-course-generate/{courseId}")]
        public IHttpActionResult HasCourseGenerate(int accountId, int courseId)
        {
            var result = _canvas.HasCourseGenerate(accountId, courseId);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpDelete]
        [Route("courses/{id}")]
        public IHttpActionResult RemoveCourse(int id)
        {
            var result = _canvas.RemoveCourse(id);
            if (!result.HasErrors)
            {
                return Ok(result.Value);
            }
            return ResultWithMessages(result);
        }
        [HttpDelete]
        [Route("accounts/{id}")]
        public IHttpActionResult RemoveAccount(int id)
        {
            var result = _canvas.RemoveAccount(id);
            if (!result.HasErrors)
            {
                var url = GlobalValues.UrlCanvas + "accounts/{0}/sub_accounts#account_" + id;
                return Ok(url);
            }
            return ResultWithMessages(result);
        }
    }
}