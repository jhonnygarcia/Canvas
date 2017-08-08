using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using DataCacheServices.AppDataCache;
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
    public class CoursesApiCanvas : BaseApiCanvas, ICoursesApiCanvas
    {
        private const string API_URL = "api/v1/courses/";
        private readonly ILogAppServices _logService;
        public CoursesApiCanvas(ILogAppServices logService)
        {
            _logService = logService;
        }

        public ResultValue<CourseCanvasDto> Update(CourseSaveParameters parameters)
        {
            var result = new ResultValue<CourseCanvasDto>();
            var url = API_URL + parameters.Id;
            dynamic param = new ExpandoObject();
            param.course = new ExpandoObject();
            param.course.sis_course_id = parameters.Asignatura?.Id.ToString() ?? "";
            var returnData = base.Put(url, (object) param);
            var logData = returnData as LogDto;
            if (logData != null)
            {
                var idLog = _logService.SaveLog(logData);
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                return result;
            }
            result.Value = new CourseCanvasDto
            {
                Id = returnData["id"],
                Name = returnData["name"],
                SisId = returnData["sis_course_id"],
                StartDate = returnData["start_at"],
                EndDate = returnData["end_at"],
                WorkflowState = returnData["workflow_state"]
            };
            return result;
        }
        public ResultValue<CourseCanvasDto> GetBySisId(string sisId)
        {
            var result = new ResultValue<CourseCanvasDto>();
            var url = API_URL + "sis_course_id:" + sisId;
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
                result.Value = new CourseCanvasDto
                {
                    Id = returnData["id"],
                    Name = returnData["name"],
                    SisId = returnData["sis_course_id"],
                    StartDate = returnData["start_at"],
                    EndDate = returnData["end_at"],
                    WorkflowState = returnData["workflow_state"]
                };
            }
            return result;
        }
        public ResultValue<CourseCanvasDto> Get(int id)
        {
            var result = new ResultValue<CourseCanvasDto>();
            var queryString = new List<string>()
            {
                "include[]=total_students",
            };
            var url = API_URL + id + "/?" + string.Join("&", queryString);
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
                result.Value = new CourseCanvasDto
                {
                    Id = returnData["id"],
                    Name = returnData["name"],
                    SisId = returnData["sis_course_id"],
                    TotalStudents = returnData["total_students"],
                    StartDate = returnData["start_at"],
                    EndDate = returnData["end_at"],
                    WorkflowState = returnData["workflow_state"]
                };
            }
            return result;
        }

        public ResultValue<MigrationCanvasDto> GetMigrationContent(int courseId,  int migrationId)
        {
            var result = new ResultValue<MigrationCanvasDto>();
            var url = API_URL + courseId + "/content_migrations/"+ migrationId;
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
                result.Value = new MigrationCanvasDto
                {
                    Id = returnData["id"],
                    MigrationType = returnData["migration_type"],
                    ProgressUrl = returnData["progress_url"],
                    WorkflowState = returnData["workflow_state"]
                };
            }
            return result;
        }
        public ResultValue<MigrationCanvasDto> CreateMigrationContent(int courseFromId, int courseToId)
        {
            var result = new ResultValue<MigrationCanvasDto>();
            var url = API_URL + courseToId + "/content_migrations";
            var param = new
            {
                migration_type = "course_copy_importer",
                settings = new
                {
                    source_course_id = courseFromId
                }
            };
            var returnData = base.Post(url, param);
            var logData = returnData as LogDto;
            if (logData != null)
            {
                var idLog = _logService.SaveLog(logData);
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                return result;
            }
            result.Value = new MigrationCanvasDto
            {
                Id = returnData["id"],
                MigrationType = returnData["migration_type"],
                ProgressUrl = returnData["progress_url"],
                WorkflowState = returnData["workflow_state"]
            };
            return result;
        }
        public ResultValue<CourseCanvasDto> Delete(int id)
        {
            var result = new ResultValue<CourseCanvasDto>();
            var url = API_URL + id + "/?event=delete";
            var returnData = base.Delete(url);
            var logData = returnData as LogDto;
            if (logData != null)
            {
                var idLog = _logService.SaveLog(logData);
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                return result;
            }
            return result;
        }
        public ResultList<AssignmentCanvasDto> GetAssignments(int courseId)
        {
            var result = new ResultList<AssignmentCanvasDto>();
            var url = API_URL + courseId + "/assignments?per_page=50&page=";
            var lista = new List<AssignmentCanvasDto>();
            var complete = false;
            var page = 1;
            while (!complete)
            {
                var returnData = base.GetAndHeaders(url + page);
                var logData = returnData as LogDto;
                if (logData != null)
                {
                    var idLog = _logService.SaveLog(logData);
                    result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                    return result;
                }
                var header = (HttpResponseHeaders)returnData.Headers;
                var links = header.GetValues("link").First();
                foreach (var data in returnData.Content)
                {
                    lista.Add(new AssignmentCanvasDto
                    {
                        Id = data["id"],
                        Name = data["name"],
                        StartDate = data["unlock_at"],
                        EndDate = data["lock_at"],
                        PresentationDate = data["due_at"],
                        Position = data["position"],
                        GroupId = data["assignment_group_id"],
                        HtmlUrl = data["html_url"]
                    });
                }
                var lnks = links.Split(',');
                complete = lnks.All(lk => !lk.Contains("next"));
                page++;
            }
            result.Elements = lista;
            return result;
        }
        public ResultValue<AssignmentCanvasDto> UpdateAssignment(int courseId, AssignmentCanvasDto parameters)
        {
            var result = new ResultValue<AssignmentCanvasDto>();
            var url = API_URL + courseId +"/assignments/" + parameters.Id;
            dynamic param = new ExpandoObject();
            param.assignment = new ExpandoObject();
            param.assignment.unlock_at = parameters.StartDate;
            param.assignment.due_at = parameters.PresentationDate;
            param.assignment.lock_at = parameters.EndDate;
            
            var returnData = base.Put(url, (object) param, HttpStatusCode.OK, HttpStatusCode.Created);
            var logData = returnData as LogDto;
            if (logData != null)
            {
                var idLog = _logService.SaveLog(logData);
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                return result;
            }
            result.Value = new AssignmentCanvasDto
            {
                Id = returnData["id"],
                Name = returnData["name"],
                StartDate = returnData["unlock_at"],
                EndDate = returnData["lock_at"],
                PresentationDate = returnData["due_at"],
                Position = returnData["position"],
                GroupId = returnData["assignment_group_id"],
                HtmlUrl = returnData["html_url"]
            };
            return result;
        }
        public ResultValue<SectionCanvasDto> CreateSection(int courseId, SectionCanvasDto section)
        {
            var result = new ResultValue<SectionCanvasDto>();
            var url = API_URL + courseId + "/sections";
            var param = new
            {
                course_section = new
                {
                    name = section.Name,
                    sis_section_id = section.SisId
                }
            };
            var returnData = base.Post(url, param);
            var logData = returnData as LogDto;
            if (logData != null)
            {
                var idLog = _logService.SaveLog(logData);
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                return result;
            }
            result.Value = new SectionCanvasDto
            {
                Id = returnData["id"],
                Name = returnData["name"],
                SisId = returnData["sis_section_id"]
            };
            return result;
        }
        public ResultList<SectionCanvasDto> GetSecctionsCourse(int courseId)
        {
            var result = new ResultList<SectionCanvasDto>();
            var url = API_URL + courseId + "/sections?per_page=50&page=";
            var lista = new List<SectionCanvasDto>();
            var complete = false;
            var page = 1;
            while (!complete)
            {
                var returnData = base.GetAndHeaders(url + page);
                var logData = returnData as LogDto;
                if (logData != null)
                {
                    var idLog = _logService.SaveLog(logData);
                    result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                    return result;
                }
                var header = (HttpResponseHeaders)returnData.Headers;
                var links = header.GetValues("link").First();
                foreach (var data in returnData.Content)
                {
                    lista.Add(new SectionCanvasDto
                    {
                        Id = data["id"],
                        Name = data["name"],
                        SisId = data["sis_section_id"]
                    });
                }
                var lnks = links.Split(',');
                complete = lnks.All(lk => !lk.Contains("next"));
                page++;
            }
            result.Elements = lista;
            return result;
        }
        public ResultValue<SectionCanvasDto> SetSisIdSecction(int sectionId, string sisId)
        {
            var result = new ResultValue<SectionCanvasDto>();
            var url = "api/v1/sections/" + sectionId;
            dynamic param = new ExpandoObject();
            param.course_section = new ExpandoObject();
            param.course_section.sis_section_id = string.IsNullOrEmpty(sisId) ? "" : sisId;

            var returnData = base.Put(url, (object)param);
            var logData = returnData as LogDto;
            if (logData != null)
            {
                var idLog = _logService.SaveLog(logData);
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                return result;
            }
            result.Value = new SectionCanvasDto
            {
                Id = returnData["id"],
                Name = returnData["name"],
                SisId = returnData["sis_section_id"]
            };
            return result;
        }
        public ResultValue<SectionCanvasDto> DeleteSecction(int sectionId)
        {
            var result = new ResultValue<SectionCanvasDto>();
            var url = "api/v1/sections/" + sectionId;
            var returnData = base.Delete(url);
            var logData = returnData as LogDto;
            if (logData != null)
            {
                var idLog = _logService.SaveLog(logData);
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                return result;
            }
            result.Value = new SectionCanvasDto
            {
                Id = returnData["id"],
                Name = returnData["name"],
                SisId = returnData["sis_section_id"]
            };
            return result;
        }
    }
}