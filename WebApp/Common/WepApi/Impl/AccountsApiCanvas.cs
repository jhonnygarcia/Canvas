using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Mvc;
using DataCacheServices.AppDataCache;
using Newtonsoft.Json;
using UNIR.Servicios.Integracion.Negocio.ClasesDto;
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
    public class AccountsApiCanvas : BaseApiCanvas, IAccountsApiCanvas
    {
        private const string API_URL = "api/v1/accounts/";
        private readonly ILogAppServices _logService;
        public AccountsApiCanvas(ILogAppServices logService)
        {
            _logService = logService;
        }

        public ResultList<AccountCanvasDto> GetSubAccount(int accountId)
        {
            var result = new ResultList<AccountCanvasDto>();
            var url = API_URL + accountId + "/sub_accounts?per_page=50&page=";
            var lista = new List<AccountCanvasDto>();
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
                    lista.Add(new AccountCanvasDto
                    {
                        Id = data["id"],
                        Name = data["name"],
                        SisId = data["sis_account_id"],
                        ParentId = data["parent_account_id"]
                    });
                }
                var lnks = links.Split(',');
                complete = lnks.All(lk => !lk.Contains("next"));
                page++;
            }
            result.Elements = lista;
            return result;
        }
        public ResultList<CourseCanvasDto> GetCourses(int accountId)
        {
            var result = new ResultList<CourseCanvasDto>();
            var queryString = new List<string>()
            {
                "include[]=total_students",
                "per_page=50&page="
            };
            var url = API_URL + accountId + "/courses?" + string.Join("&", queryString);
            var lista = new List<CourseCanvasDto>();
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
                    lista.Add(new CourseCanvasDto
                    {
                        Id = data["id"],
                        Name = data["name"],
                        SisId = data["sis_course_id"],
                        TotalStudents = data["total_students"],
                        WorkflowState = data["workflow_state"],
                        StartDate = data["start_at"],
                        EndDate = data["end_at"]
                    });
                }
                var lnks = links.Split(',');
                complete = lnks.All(lk => !lk.Contains("next"));
                page++;
            }
            result.Elements = lista;
            return result;
        }
        public ResultValue<AccountCanvasDto> GetAccount(int accountId)
        {
            var result = new ResultValue<AccountCanvasDto>();
            var url = API_URL + accountId;
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
            result.Value = new AccountCanvasDto
            {
                Id = returnData["id"],
                Name = returnData["name"],
                SisId = returnData["sis_account_id"],
                ParentId = returnData["parent_account_id"]
            };
            return result;
        }
        public ResultValue<AccountCanvasDto> Update(AccountSaveParameters parameters)
        {
            var result = new ResultValue<AccountCanvasDto>();
            var url = API_URL + parameters.Id;
            var param = new
            {
                account = new
                {
                    sis_account_id = parameters.Estudio?.Id
                }
            };
            var returnData = base.Put(url, param);
            var logData = returnData as LogDto;
            if (logData != null)
            {
                var idLog = _logService.SaveLog(logData);
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                return result;
            }
            result.Value = new AccountCanvasDto
            {
                Id = returnData["id"],
                Name = returnData["name"],
                SisId = returnData["sis_account_id"],
                WorkflowState = returnData["workflow_state"],
                ParentId = returnData["parent_account_id"]
            };
            return result;
        }
        public ResultValue<AccountCanvasDto> GetBySisId(string sisId)
        {
            var result = new ResultValue<AccountCanvasDto>();
            var url = API_URL + "sis_account_id:" + sisId;
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
                result.Value = new AccountCanvasDto
                {
                    Id = returnData["id"],
                    Name = returnData["name"],
                    SisId = returnData["sis_account_id"],
                    WorkflowState = returnData["workflow_state"],
                    ParentId = returnData["parent_account_id"]
                };
            }
            return result;
        }
        public ResultValue<AccountCanvasDto> CreateAccount(AccountSaveParameters parameters)
        {
            var result = new ResultValue<AccountCanvasDto>();
            var url = API_URL + parameters.ParentAccountId + "/sub_accounts";
            var param = new
            {
                account = new
                {
                    sis_account_id = parameters.Estudio.Id + "-" + parameters.IdPeriodoMatriculacion,
                    name = parameters.Name
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
            result.Value = new AccountCanvasDto
            {
                Id = returnData["id"],
                Name = returnData["name"],
                SisId = returnData["sis_account_id"],
                WorkflowState = returnData["workflow_state"],
                ParentId = returnData["parent_account_id"]
            };
            return result;
        }
        public ResultValue<CourseCanvasDto> CreateCourse(CourseCanvasDto course, int idPeriodoMatriculacion, int idAsignatura)
        {
            var result = new ResultValue<CourseCanvasDto>();
            var url = API_URL + course.AccountId + "/courses";
            var param = new
            {
                course = new
                {
                    name = course.Name,
                    start_at = course.StartDate,
                    end_at = course.EndDate,
                    sis_course_id = idPeriodoMatriculacion + "-" + idAsignatura
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
            result.Value = new CourseCanvasDto
            {
                Id = returnData["id"],
                Name = returnData["name"],
                SisId = returnData["sis_course_id"],
                WorkflowState = returnData["workflow_state"],
                StartDate = returnData["start_at"],
                EndDate = returnData["end_at"]
            };
            return result;
        }
        public ResultList<CourseCanvasDto> GetCoursesAll(int accountId)
        {
            var result = new ResultList<CourseCanvasDto>();
            var queryString = new List<string>()
            {
                "include[]=total_students",
                 "state[]=all",
                "per_page=50&page="
            };
            var url = API_URL + accountId + "/courses?" + string.Join("&", queryString);
            var lista = new List<CourseCanvasDto>();
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
                    lista.Add(new CourseCanvasDto
                    {
                        Id = data["id"],
                        Name = data["name"],
                        SisId = data["sis_course_id"],
                        TotalStudents = data["total_students"],
                        WorkflowState = data["workflow_state"],
                        StartDate = data["start_at"],
                        EndDate = data["end_at"]
                    });
                }
                var lnks = links.Split(',');
                complete = lnks.All(lk => !lk.Contains("next"));
                page++;
            }
            result.Elements = lista;
            return result;
        }
        public ResultValue<ProgressCanvasDto> DeleteCoursesAccount(int accountId, int[] coursesIds)
        {
            var result = new ResultValue<ProgressCanvasDto>();
            var url = API_URL + accountId + "/courses";
            var param = new
            {
                @event = "delete",
                course_ids = coursesIds
            };
            var returnData = base.Put(url, param);
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
        public ResultValue<SisImportCanvasDto> ImportUsers(int accountId, List<PersonaDto> stundents)
        {
            var result = new ResultValue<SisImportCanvasDto>();
            var csv = new StringBuilder();
            csv.AppendLine("user_id,login_id,password,first_name,last_name,email,status");
            foreach (var personaDto in stundents)
            {
                var line = $"{personaDto.idPersona},{personaDto.login},{personaDto.password}," +
                           $"{personaDto.sNombrePersona}, {personaDto.sApellidosPersona},{personaDto.correoElectronico},active";
                csv.AppendLine(line);
            }

            var url = API_URL + accountId + "/sis_imports";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalValues.TokenCanvas);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                client.Timeout = new TimeSpan(0, 0, 40);
                var uri = new Uri(GlobalValues.UrlCanvas + url);
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                var content = new MultipartFormDataContent();
                request.Headers.ExpectContinue = false;
                request.Content = content;

                byte[] toBytes = Encoding.UTF8.GetBytes(csv.ToString());
                var fileContent = new ByteArrayContent(toBytes);
                content.Add(new StringContent("instructure_csv"), "import_type");
                content.Add(fileContent, "\"attachment\"", "\"" + "users.csv" + "\"");

                var response = client.SendAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var returnData = response.Content.ReadAsAsync<dynamic>().Result;
                    result.Value = new SisImportCanvasDto
                    {
                        Id = returnData["id"],
                        WorkflowState = returnData["workflow_state"],
                        Progress = returnData["progress"]
                    };
                }
                else
                {
                    var statusCode = Convert.ToInt32(response.StatusCode);
                    var status = response.StatusCode.ToString();
                    var exception = request.Content.ReadAsStringAsync().Result;
                    var logEntity = new LogDto
                    {
                        Date = DateTime.Now,
                        Loger = uri.ToString(),
                        Message = "POST " + status + ":" + statusCode,
                        Exception = exception,
                        Level = GlobalValues.WARNING,
                        StatusCode = statusCode
                    };
                    var idLog = _logService.SaveLog(logEntity);
                    result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                    return result;
                }
            }
            return result;
        }
        public ResultValue<SisImportCanvasDto> ImportEnrollments(int accountId, List<EnrollmentCanvasDto> enrollments)
        {
            var result = new ResultValue<SisImportCanvasDto>();
            var csv = new StringBuilder();
            csv.AppendLine("course_id,user_id,role,section_id,status");
            foreach (var enrollment in enrollments)
            {
                var line = $"{enrollment.CourseId},{enrollment.UserId},{enrollment.Role}," +
                           $"{enrollment.SectionId}, {enrollment.Status}";
                csv.AppendLine(line);
            }

            var url = API_URL + accountId + "/sis_imports";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalValues.TokenCanvas);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                client.Timeout = new TimeSpan(0, 0, 40);
                var uri = new Uri(GlobalValues.UrlCanvas + url);
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                var content = new MultipartFormDataContent();
                request.Headers.ExpectContinue = false;
                request.Content = content;

                byte[] toBytes = Encoding.UTF8.GetBytes(csv.ToString());
                var fileContent = new ByteArrayContent(toBytes);
                content.Add(new StringContent("instructure_csv"), "import_type");
                content.Add(fileContent, "\"attachment\"", "\"" + "enrollments.csv" + "\"");

                var response = client.SendAsync(request).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var returnData = response.Content.ReadAsAsync<dynamic>().Result;
                    result.Value = new SisImportCanvasDto
                    {
                        Id = returnData["id"],
                        WorkflowState = returnData["workflow_state"],
                        Progress = returnData["progress"]
                    };
                }
                else
                {
                    var statusCode = Convert.ToInt32(response.StatusCode);
                    var status = response.StatusCode.ToString();
                    var exception = request.Content.ReadAsStringAsync().Result;
                    var logEntity = new LogDto
                    {
                        Date = DateTime.Now,
                        Loger = uri.ToString(),
                        Message = "POST " + status + ":" + statusCode,
                        Exception = exception,
                        Level = GlobalValues.WARNING,
                        StatusCode = statusCode
                    };
                    var idLog = _logService.SaveLog(logEntity);
                    result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                    return result;
                }
            }
            return result;
        }
        public ResultValue<SisImportCanvasDto> GetImported(int accountId, int importedId)
        {
            var result = new ResultValue<SisImportCanvasDto>();
            var url = API_URL + accountId + "/sis_imports/" + importedId;
            var returnData = base.Get(url);
            var logData = returnData as LogDto;
            if (logData != null)
            {
                var idLog = _logService.SaveLog(logData);
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorApi, idLog));
                return result;
            }
            result.Value = new SisImportCanvasDto
            {
                Id = returnData["id"],
                WorkflowState = returnData["workflow_state"],
                Progress = returnData["progress"]
            };
            return result;
        }
    }
}