using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using DataCacheServices.AppDataCache;
using Newtonsoft.Json;
using UNIR.Comun.Servicios;
using UNIR.Servicios.Integracion.Negocio.ClasesDto;
using WebApp.App_Start;
using WebApp.Architecture.Parameters;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Common.WepApi;
using WebApp.Globalization.Services;
using WebApp.Models.Dto;
using WebApp.Models.Model;
using WebApp.Models.Model.Entity;
using WebApp.Parameters;

namespace WebApp.Models.Services.Impl
{
    public class CanvasAppServices : ICanvasAppServices
    {
        private readonly IAccountsApiCanvas _accountsApi;
        private readonly ICoursesApiCanvas _coursesApi;
        private readonly IEstudios _wEstudios;
        private readonly CanvasExtendContenxt _context;
        private readonly IDataCacheService _dataCache;
        private readonly IProgressApiCanvas _progressApi;
        private readonly IUsersApiCanvas _userApi;
        public CanvasAppServices(IAccountsApiCanvas accountsApi, ICoursesApiCanvas coursesApi, CanvasExtendContenxt context,
            IEstudios wEstudios, IDataCacheService dataCache, IProgressApiCanvas progressApi, IUsersApiCanvas userApi)
        {
            _accountsApi = accountsApi;
            _coursesApi = coursesApi;
            _context = context;
            _wEstudios = wEstudios;
            _dataCache = dataCache;
            _progressApi = progressApi;
            _userApi = userApi;
        }
        private void SaveAccountExtend(AccountSaveParameters parameter)
        {
            var accountExtend = _context.AccountExtends.FirstOrDefault(a => a.AccountId == parameter.Id);
            var estudio = _context.Estudios.FirstOrDefault(e => e.Id == parameter.Estudio.Id);
            if (estudio == null)
            {
                estudio = new Estudio
                {
                    Id = parameter.Estudio.Id,
                    Nombre = parameter.Estudio.Value
                };
            }
            else
            {
                estudio.Nombre = parameter.Estudio.Value;
            }

            if (accountExtend == null)
            {
                accountExtend = new AccountExtend
                {
                    AccountId = parameter.Id,
                    Name = parameter.Name,
                    Estudio = estudio,
                    Asignaturas = new List<Asignatura>(),
                    PeriodoActivos = new List<PeriodoActivo>()
                };
                _context.AccountExtends.Add(accountExtend);
            }
            else
            {
                accountExtend.Estudio = estudio;
                accountExtend.Name = parameter.Name;
                accountExtend.Asignaturas.Clear();
                accountExtend.PeriodoActivos.Clear();
            }
            /*****Guardar Asignaturas Asociadas al estudio*****/
            var resAsignaturas = _wEstudios.ObtenerAsignaturasDeEstudio(parameter.Estudio.Id);
            var ids = resAsignaturas.Respuesta.Select(a => a.idAsignatura).ToArray();
            var asignaturas = _context.Asignaturas.Where(a => ids.Contains(a.Id)).ToList();
            foreach (var item in resAsignaturas.Respuesta)
            {
                var asignatura = asignaturas.FirstOrDefault(a => a.Id == item.idAsignatura);
                if (asignatura == null)
                {
                    asignatura = new Asignatura
                    {
                        Id = item.idAsignatura,
                        Nombre = item.sNombreAsignatura
                    };
                    _context.Asignaturas.Add(asignatura);
                }
                else
                {
                    asignatura.Nombre = item.sNombreAsignatura;
                }
                accountExtend.Asignaturas.Add(asignatura);
            }
            /*****Guardar PeriodoActivos asociados al estudio*****/
            var resPeriodosActivos = _wEstudios.ObtenerCursosDeAsignaturasDeEstudio(parameter.Estudio.Id);
            ids = resPeriodosActivos.Respuesta.listaPeriodosActivos.Select(a => a.idPeriodoMatriculacion).ToArray();
            var periodoActivos = _context.PeriodosActivos.Where(p => ids.Contains(p.Id)).ToList();
            foreach (var item in resPeriodosActivos.Respuesta.listaPeriodosActivos)
            {
                var periodoActivo = periodoActivos.FirstOrDefault(a => a.Id == item.idPeriodoMatriculacion);
                if (periodoActivo == null)
                {
                    periodoActivo = new PeriodoActivo
                    {
                        Id = item.idPeriodoMatriculacion,
                        Nombre = item.sNombrePeriodoMatriculacion,
                        FechaInicio = item.fechaInicioPeriodo,
                        FechaFin = item.fechaFinPeriodo,
                        NroPeriodo = item.iNumPeriodo,
                        AnioAcademico = item.sAyoAcademinco
                    };
                    _context.PeriodosActivos.Add(periodoActivo);
                }
                else
                {
                    periodoActivo.Nombre = item.sNombrePeriodoMatriculacion;
                    periodoActivo.FechaInicio = item.fechaInicioPeriodo;
                    periodoActivo.FechaFin = item.fechaFinPeriodo;
                    periodoActivo.NroPeriodo = item.iNumPeriodo;
                    periodoActivo.AnioAcademico = item.sAyoAcademinco;
                }
                accountExtend.PeriodoActivos.Add(periodoActivo);
            }
            _context.SaveChanges();
        }
        private void SaveCourseExtend(CourseSaveParameters parameter)
        {
            var accountExted = _context.AccountExtends.FirstOrDefault(a => a.AccountId == parameter.AccountId);
            var courseExtend = _context.CourseExtends.FirstOrDefault(a => a.CourseId == parameter.Id);
            var asignatura = _context.Asignaturas.FirstOrDefault(e => e.Id == parameter.Asignatura.Id);
            if (asignatura == null)
            {
                asignatura = new Asignatura
                {
                    Id = parameter.Asignatura.Id,
                    Nombre = parameter.Asignatura.Value,
                };
            }
            else
            {
                asignatura.Nombre = parameter.Asignatura.Value;
            }
            if (courseExtend == null)
            {
                courseExtend = new CourseExtend
                {
                    CourseId = parameter.Id,
                    Name = parameter.Name,
                    Asignatura = asignatura,
                    Account = accountExted
                };
                _context.CourseExtends.Add(courseExtend);
            }
            else
            {
                courseExtend.Asignatura = asignatura;
                courseExtend.Name = parameter.Name;
                courseExtend.Account = accountExted;
            }
            _context.SaveChanges();
        }
        private void SaveAccountGenerate(int newId, int accountId, int idEstudio, int idPeriodoMatriculacion, string nombre)
        {
            if (!_context.AccountGenerates.Any(a => a.IdEstudio == idEstudio
                    && a.IdPeriodoActivo == idPeriodoMatriculacion &&
                        a.AccountId == accountId))
            {
                var periodoActivo = _context.PeriodosActivos.Find(idPeriodoMatriculacion);
                var accountGenerate = new AccountGenerate
                {
                    Id = newId,
                    AccountId = accountId,
                    IdEstudio = idEstudio,
                    IdPeriodoActivo = idPeriodoMatriculacion,
                    Name = nombre,
                    NombrePeriodoMatriculacion = periodoActivo.Nombre,
                    AnioAcademico = periodoActivo.AnioAcademico,
                    FechaInicio = periodoActivo.FechaInicio,
                    FechaFin = periodoActivo.FechaFin
                };
                _context.AccountGenerates.Add(accountGenerate);
                _context.SaveChanges();
            }
        }
        public ResultList<AccountCanvasDto> GetSubAccountsCourses(int accountId)
        {
            var result = new ResultList<AccountCanvasDto>();
            var resAccounts = _accountsApi.GetSubAccount(accountId);
            if (resAccounts.HasErrors)
            {
                result.Errors.AddRange(resAccounts.Errors);
                return result;
            }
            var ids = resAccounts.Elements
                .Select(a => a.Id)
                .ToArray();

            var accountsGenerates = _context.AccountGenerates.Where(a => ids.Contains(a.AccountId)).ToList();
            var accountsExtends = _context.AccountExtends.Where(a => ids.Contains(a.AccountId)).ToList();
            foreach (var account in resAccounts.Elements)
            {
                var accountExtend = accountsExtends.FirstOrDefault(a => a.AccountId == account.Id);
                if (accountExtend != null)
                {
                    account.Estudio = new SimpleItem<int>
                    {
                        Id = accountExtend.IdEstudio,
                        Value = accountExtend.Estudio.Nombre
                    };
                }
                account.Generated = accountsGenerates.Any(a => a.AccountId == account.Id);
                account.IsAccountPeriodo = _context.AccountGenerates.Any(a => a.Id == account.Id);
                var resCourse = _accountsApi.GetCourses(account.Id);
                if (resCourse.HasErrors)
                {
                    result.Type = ResultType.InternalError;
                    result.Errors.AddRange(resCourse.Errors);
                    continue;
                }
                int test;
                ids = resCourse.Elements.Where(e => !string.IsNullOrEmpty(e.SisId) && int.TryParse(e.SisId, out test))
                        .Select(a => int.Parse(a.SisId))
                        .ToArray();
                var asignaturas = _context.Asignaturas.Where(a => ids.Contains(a.Id)).ToList();
                foreach (var course in resCourse.Elements)
                {
                    if (!string.IsNullOrEmpty(course.SisId))
                    {
                        var asigantura = asignaturas.FirstOrDefault(a => a.Id == int.Parse(course.SisId));
                        if (asigantura != null)
                        {
                            course.Asignatura = new SimpleItem<int>
                            {
                                Id = asigantura.Id,
                                Value = asigantura.Nombre
                            };
                        }
                    }
                }
                account.Courses = resCourse.Elements;
            }
            result.Elements = resAccounts.Elements;
            return result;
        }
        public ResultValue<AccountCanvasDto> GetAccount(int accountId)
        {
            var result = new ResultValue<AccountCanvasDto>();
            var resAccount = _accountsApi.GetAccount(accountId);
            if (resAccount.HasErrors)
            {
                result.Errors.AddRange(resAccount.Errors);
                return result;
            }
            result.Value = resAccount.Value;
            return result;
        }

        public ResultList<CourseCanvasDto> GetCourses(int accountId)
        {
            var result = new ResultList<CourseCanvasDto>();
            var resCourses = _accountsApi.GetCourses(accountId);
            if (resCourses.HasErrors)
            {
                result.Errors.AddRange(resCourses.Errors);
                return result;
            }
            result.Elements = resCourses.Elements;
            return result;
        }
        public ResultValue<AccountCanvasDto> UpdateAccount(AccountSaveParameters parameter)
        {
            var result = new ResultValue<AccountCanvasDto>();
            if (parameter.Estudio == null)
            {
                result.Errors.Add(CanvasApiStrings.ErrorParametroEstudioNull);
                return result;
            }
            var resAccountA = _accountsApi.GetAccount(parameter.Id);
            if (resAccountA.Value == null)
            {
                //Validacion de existencialismo
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorAccountNotExist, parameter.Id));
                return result;
            }
            var resAccountB = _accountsApi.GetBySisId(parameter.Estudio.Id.ToString());
            if (resAccountB.Value != null)
            {
                if (resAccountB.Value.GetState() == AccountState.Deleted)
                {
                    var resUpdateDelete = _accountsApi.Update(new AccountSaveParameters
                    {
                        Id = resAccountB.Value.Id,
                        Estudio = null
                    });
                    if (resUpdateDelete.HasErrors)
                    {
                        result.Errors.AddRange(resUpdateDelete.Errors);
                        return result;
                    }
                }
                else if (resAccountB.Value.Id != parameter.Id)
                {
                    //Validacion que no exista otra cuenta con el mismo SIS ID
                    result.Errors.Add(string.Format(CanvasApiStrings.ErrorAccountSisIDExistente, resAccountB.Value.Name, parameter.Estudio.Id));
                    return result;
                }
            }
            //Validacion pendiente el caso que la cuenta ya este clonada se borraran toda la cuenta demo
            var resUpdate = _accountsApi.Update(parameter);
            if (resUpdate.HasErrors)
            {
                result.Errors.AddRange(resUpdate.Errors);
                return result;
            }
            SaveAccountExtend(parameter);
            result.Value = resUpdate.Value;
            return result;
        }
        public ResultValue<CourseCanvasDto> UpdateCourse(CourseSaveParameters parameter)
        {
            var result = new ResultValue<CourseCanvasDto>();
            var resCourseA = _coursesApi.Get(parameter.Id);
            if (resCourseA.Value == null)
            {
                //Validacion de existencialismo
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorCourseNotExist, parameter.Id));
                return result;
            }
            if (resCourseA.Value.GetState() == CourseState.Available)
            {
                //Validacion que no este en estado publicado
                result.Errors.Add(CanvasApiStrings.ErrorCursoPublicadoNoModificar);
                return result;
            }
            var resCourseB = _coursesApi.GetBySisId(parameter.Asignatura.Id.ToString());
            if (resCourseB.Value != null)
            {
                if (resCourseB.Value.GetState() == CourseState.Deleted)
                {
                    var resUpdateDelete = _coursesApi.Update(new CourseSaveParameters
                    {
                        Id = resCourseB.Value.Id,
                        Asignatura = null
                    });
                    if (resUpdateDelete.HasErrors)
                    {
                        result.Errors.AddRange(resUpdateDelete.Errors);
                        return result;
                    }
                }
                else if (resCourseB.Value.Id != parameter.Id)
                {
                    //Validacion que no exista otro curso con el mismo SIS ID
                    result.Errors.Add(string.Format(CanvasApiStrings.ErrorCourseSisIDExistente, resCourseB.Value.Name,
                        parameter.Asignatura.Id));
                    return result;
                }
            }
            var accountExtend = _context.AccountExtends.FirstOrDefault(a => a.AccountId == parameter.AccountId);
            if (accountExtend == null)
            {
                result.Errors.Add(CanvasApiStrings.ErrorAccountError);
                return result;
            }
            else
            {
                var existsAccount = accountExtend.Asignaturas.Any(a => a.Id == parameter.Asignatura.Id);
                if (!existsAccount)
                {
                    result.Errors.Add(string.Format(CanvasApiStrings.ErrorAsignaturaNoAccount,
                        parameter.Asignatura.Value, parameter.AccountId));
                    return result;
                }
            }

            var resUpdate = _coursesApi.Update(parameter);
            if (resUpdate.HasErrors)
            {
                result.Errors.AddRange(resUpdate.Errors);
                return result;
            }
            SaveCourseExtend(parameter);
            result.Value = resUpdate.Value;
            return result;
        }
        public ResultValue<AccountCanvasDto> GenerarPeriodo(AccountSaveParameters parameter)
        {
            var result = new ResultValue<AccountCanvasDto>();
            if (string.IsNullOrEmpty(parameter.Name) || string.IsNullOrWhiteSpace(parameter.Name))
            {
                result.Errors.Add(CanvasApiStrings.ErrorNombrePeriodoVacio);
                return result;
            }
            if (!parameter.IdPeriodoMatriculacion.HasValue)
            {
                result.Errors.Add(CanvasApiStrings.ErrorPeriodoActvioNoNull);
                return result;
            }
            var resAccount = GetAccount(parameter.Id);
            if (resAccount.HasErrors)
            {
                result.Errors.AddRange(resAccount.Errors);
                return result;
            }
            if (string.IsNullOrEmpty(resAccount.Value.SisId))
            {
                result.Errors.Add(CanvasApiStrings.ErrorNullSisIdAccount);
                return result;
            }
            var resAccountBySisId = _accountsApi.GetBySisId(resAccount.Value.SisId + "-" + parameter.IdPeriodoMatriculacion);
            if (resAccountBySisId.Value != null)
            {
                if (resAccountBySisId.Value.GetState() == AccountState.Deleted)
                {
                    var resUpdateDelete = _accountsApi.Update(new AccountSaveParameters
                    {
                        Id = resAccountBySisId.Value.Id,
                        Estudio = null
                    });
                    if (resUpdateDelete.HasErrors)
                    {
                        result.Errors.AddRange(resUpdateDelete.Errors);
                        return result;
                    }
                }
                else
                {
                    result.Errors.Add(string.Format(CanvasApiStrings.ErrorAccountSisIDExistente,
                           resAccountBySisId.Value.Name,
                           resAccount.Value.SisId + "-" + parameter.IdPeriodoMatriculacion));
                    return result;
                }
            }

            parameter.ParentAccountId = resAccount.Value.ParentId;
            parameter.Estudio = new SimpleItem<int>
            {
                Id = int.Parse(resAccount.Value.SisId)
            };

            var resCreate = _accountsApi.CreateAccount(parameter);
            if (result.HasErrors)
            {
                result.Errors.AddRange(resCreate.Errors);
                return result;
            }
            SaveAccountGenerate(resCreate.Value.Id, parameter.Id, int.Parse(resAccount.Value.SisId),
                parameter.IdPeriodoMatriculacion.Value, parameter.Name);
            return result;
        }
        public ResultValue<ResultMigrationDto> MigrarCursos(int accountId, int progressId)
        {
            var result = new ResultValue<ResultMigrationDto> { Value = new ResultMigrationDto() };
            var rootId = int.Parse(ConfigurationManager.AppSettings["AccountRoot"]);
            var initialize = DateTime.Now;
            var resValidate = ValidateMigrarCursos(accountId, result);
            if (result.HasErrors) return result;

            #region Validaciones
            var resAccountPeriodo = resValidate.Item1;
            var acccountGenerate = resValidate.Item2;
            var resCourses = resValidate.Item3;
            var currentCourses = resValidate.Item4;
            var resAsignaturas = resValidate.Item5;
            #endregion

            var idsAsignaturas = resAsignaturas.Respuesta.Select(a => a.idAsignatura).Distinct().ToList();
            var test = -1;
            var courses = resCourses.Elements.Where(a => int.TryParse(a.SisId, out test) && idsAsignaturas.Contains(test)).ToList();

            var coursesNoDelete = currentCourses.Elements.Where(c => c.GetState() != CourseState.Deleted).ToList();
            foreach (var course in coursesNoDelete)
            {
                var resSection = _coursesApi.GetSecctionsCourse(course.Id);
                if (resSection.HasErrors)
                {
                    result.Errors.AddRange(resSection.Errors);
                    return result;
                }
                foreach (var section in resSection.Elements)
                {
                    var resUpdateSection = _coursesApi.SetSisIdSecction(section.Id, string.Empty);
                    if (resUpdateSection.HasErrors)
                    {
                        result.Errors.AddRange(resUpdateSection.Errors);
                        return result;
                    }
                }
            }
            var deleteCourses = coursesNoDelete.Select(a => a.Id).ToArray();
            if (deleteCourses.Length > 0)
            {
                var resProgress = _accountsApi.DeleteCoursesAccount(accountId, deleteCourses);
                var pending = true;
                while (pending)
                {
                    var resProg = _progressApi.Get(resProgress.Value.Id);
                    if (resProg.Value.GetState() == ProgressState.Failed)
                    {
                        result.Errors.Add(string.Format(CanvasExtedStrings.ErrorTareaDeleteCoursesAccount,
                            resAccountPeriodo.Value.Name));
                        return result;
                    }
                    if (resProg.Value.GetState() == ProgressState.Completed)
                    {
                        pending = false;
                    }
                }
            }

            var coursesGenerates = acccountGenerate.CoursesGenerates.ToList();
            foreach (var coursesGenerate in coursesGenerates)
            {
                _context.CourseGenerates.Remove(coursesGenerate);
            }
            if (coursesGenerates.Any())
            {
                _context.SaveChanges();
                acccountGenerate.CoursesGenerates.Clear();
            }

            var total = courses.Count;
            var avance = 0;
            var progressInfo = _context.ProgressInfo.Find(progressId);
            foreach (var course in courses)
            {
                avance++;
                var asignatura = resAsignaturas.Respuesta.FirstOrDefault(r => r.idAsignatura == int.Parse(course.SisId));
                if (asignatura == null)
                {
                    result.Value.Courses.Add(new MotivoDto<CourseCanvasDto>
                    {
                        Value = course,
                        Code = (int)CodeMigrationIrregularidad.CodeMessage1
                    });
                    continue;
                }
                if (!asignatura.listaCursos.Any() || asignatura.listaCursos.All(c => !c.fechaInicioCurso.HasValue)
                    || asignatura.listaCursos.All(c => !c.fechaFinCurso.HasValue))
                {
                    result.Value.Courses.Add(new MotivoDto<CourseCanvasDto>
                    {
                        Value = course,
                        Code = (int)CodeMigrationIrregularidad.CodeMessage2
                    });
                    continue;
                }

                var fechaIncioCurso = asignatura.listaCursos.Min(c => c.fechaInicioCurso);
                var fechaFinCurso = asignatura.listaCursos.Max(c => c.fechaFinCurso);
                if (fechaIncioCurso.HasValue && fechaFinCurso.HasValue && fechaIncioCurso.Value > fechaFinCurso.Value)
                {
                    result.Value.Courses.Add(new MotivoDto<CourseCanvasDto>
                    {
                        Value = new CourseCanvasDto
                        {
                            Id = -1,
                            Name = course.Name + " (Inicio: " + fechaIncioCurso.Value.ToString("dd/MM/yyyy") + " Fin:" + fechaFinCurso.Value.ToString("dd/MM/yyyy") + ")",
                            SisId = course.SisId
                        },
                        Code = (int)CodeMigrationIrregularidad.CodeMessage3
                    });

                    continue;
                }
                var resCreateCourse = _accountsApi.CreateCourse(new CourseCanvasDto
                {
                    AccountId = accountId,
                    Name = course.Name,
                    StartDate = fechaIncioCurso,
                    EndDate = fechaFinCurso,
                }, acccountGenerate.IdPeriodoActivo, int.Parse(course.SisId));
                if (resCreateCourse.HasErrors)
                {
                    result.Errors.AddRange(resCreateCourse.Errors);
                    return result;
                }
                var resMigrate = _coursesApi.CreateMigrationContent(course.Id, resCreateCourse.Value.Id);
                var pending = true;
                while (pending)
                {
                    var resProg = _coursesApi.GetMigrationContent(resCreateCourse.Value.Id, resMigrate.Value.Id);
                    if (resProg.Value.GetState() == MigrationState.Failed)
                    {
                        result.Errors.Add(string.Format(CanvasApiStrings.ErrorMigrationContent,
                            resAccountPeriodo.Value.Name));
                        return result;
                    }
                    if (resProg.Value.GetState() == MigrationState.Completed)
                    {
                        pending = false;
                    }
                }

                if (resMigrate.HasErrors)
                {
                    result.Errors.AddRange(resMigrate.Errors);
                    return result;
                }
                //Crear cursos Generados
                CreateCourseGenerate(resCreateCourse.Value, acccountGenerate.Id);
                foreach (var cursoCompletoDto in asignatura.listaCursos)
                {
                    var resCreateSection = _coursesApi.CreateSection(resCreateCourse.Value.Id,
                        new SectionCanvasDto
                        {
                            Name = "Grupo " + cursoCompletoDto.iNumeroGrupoReal ?? "",
                            SisId = cursoCompletoDto.idCurso.ToString()
                        });
                    if (resCreateSection.HasErrors)
                    {
                        result.Errors.AddRange(resCreateSection.Errors);
                        return result;
                    }
                }

                var alumnos = asignatura.listaCursos.SelectMany(c => c.ListaPersonas)
                    .Where(a => a.idTipoPersona == 1)
                    .ToList();
                if (alumnos.Any())
                {
                    var alumnosNuevos = new List<PersonaDto>();
                    foreach (var personaDto in alumnos)
                    {
                        var resUser = _userApi.GetBySisId(personaDto.idPersona.ToString());
                        if (resUser.Value == null)
                        {
                            if (string.IsNullOrEmpty(personaDto.login) || string.IsNullOrEmpty(personaDto.sNombrePersona)
                                || string.IsNullOrEmpty(personaDto.sApellidosPersona)
                                || string.IsNullOrEmpty(personaDto.correoElectronico))
                            {
                                //Guardar tabla de resultados
                                result.Value.Stundents.Add(new MotivoDto<PersonaDto>
                                {
                                    Value = personaDto,
                                    Code = (int)CodeMigrationIrregularidad.CodeMessage4
                                });
                                continue;
                            }
                            if (string.IsNullOrEmpty(personaDto.password) || personaDto.password.Length < 8)
                            {
                                //Guardar tabla de resultados
                                result.Value.Stundents.Add(new MotivoDto<PersonaDto>
                                {
                                    Value = personaDto,
                                    Code = (int)CodeMigrationIrregularidad.CodeMessage5
                                });
                                continue;
                            }
                            alumnosNuevos.Add(personaDto);
                        }
                    }
                    //Alumnos no existentes registrados como usuarios en canvas
                    if (alumnosNuevos.Count > 0)
                    {
                        var resImportedUsers = _accountsApi.ImportUsers(rootId, alumnosNuevos);
                        while (resImportedUsers.Value.Progress < 100)
                        {
                            resImportedUsers = _accountsApi.GetImported(rootId, resImportedUsers.Value.Id);
                            if (resImportedUsers.Value.GetState() == SisImportState.Failed)
                            {
                                result.Errors.Add(CanvasApiStrings.ErrrorMigrationAlumnos);
                                return result;
                            }
                        }
                    }

                    var enrollments = alumnos.Select(a =>
                    {
                        var sectionId = asignatura.listaCursos.FirstOrDefault(
                                c => c.ListaPersonas.Any(p => p.idPersona == a.idPersona));
                        if (sectionId == null) return null;
                        return new EnrollmentCanvasDto
                        {
                            CourseId = resCreateCourse.Value.SisId,
                            UserId = a.idPersona.ToString(),
                            Role = "student",
                            SectionId = sectionId.idCurso.ToString(),
                            Status = "active"
                        };
                    }).Where(e => e != null).ToList();
                    //Asociar alumnos o matricularlos
                    if (enrollments.Any())
                    {
                        var resImportedEnrollments = _accountsApi.ImportEnrollments(rootId, enrollments);
                        while (resImportedEnrollments.Value.Progress < 100)
                        {
                            resImportedEnrollments = _accountsApi.GetImported(rootId, resImportedEnrollments.Value.Id);
                            if (resImportedEnrollments.Value.GetState() == SisImportState.Failed)
                            {
                                result.Errors.Add(CanvasApiStrings.ErrorEnMigrationMatriculacion);
                                return result;
                            }
                        }
                    }
                }

                //Calcular Fechas de Tareas
                var oldAssignments = _coursesApi.GetAssignments(course.Id);
                var newAssignments = _coursesApi.GetAssignments(resCreateCourse.Value.Id);
                var periodosNoLectivos = acccountGenerate.PeriodosNoLectivos.ToList();
                CalculateDates(course.StartDate.Value, course.EndDate.Value, fechaIncioCurso.Value,
                    fechaFinCurso.Value, oldAssignments.Elements, newAssignments.Elements, periodosNoLectivos);
                foreach (var assignment in newAssignments.Elements)
                {
                    if (assignment.ExtraData != null)
                    {
                        result.Value.Assignments.Add(new MotivoDto<AssignmentCanvasDto>
                        {
                            Value = assignment,
                            Code = (int)assignment.ExtraData
                        });
                    }
                    var resUpdateAssignment = _coursesApi.UpdateAssignment(resCreateCourse.Value.Id, assignment);
                    if (resUpdateAssignment.HasErrors)
                    {
                        result.Errors.AddRange(resUpdateAssignment.Errors);
                        return result;
                    }
                }
                if (progressInfo != null)
                {
                    var value = Convert.ToInt32(avance * 100 / total);
                    progressInfo.Completion = value > 1 ? value - 1 : value;
                    _context.SaveChanges();
                }
            }
            progressInfo.Completion = 100;
            SaveLogMigration(result.Value, acccountGenerate, initialize);
            return result;
        }
        public ResultValue<ResultMigrationDto> MigrarCurso(int generateId, int courseDemoId, int progressId)
        {
            var result = new ResultValue<ResultMigrationDto> { Value = new ResultMigrationDto() };
            var rootId = int.Parse(ConfigurationManager.AppSettings["AccountRoot"]);
            var initialize = DateTime.Now;
            var pending = true;
            var courseDemo = _coursesApi.Get(courseDemoId);
            if (courseDemo.Value == null)
            {
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorCourseNotExist, courseDemoId));
                return result;
            }
            if (string.IsNullOrEmpty(courseDemo.Value.SisId))
            {
                result.Errors.Add(CanvasExtedStrings.ErrorCourseNoSisId);
                return result;
            }
            if (!courseDemo.Value.StartDate.HasValue || !courseDemo.Value.EndDate.HasValue)
            {
                result.Errors.Add(string.Format(CanvasExtedStrings.ErrorCursosSinFechaConfigurada, courseDemo.Value.Name));
                return result;
            }
            var acccountGenerate = _context.AccountGenerates.Find(generateId);
            if (acccountGenerate == null)
            {
                result.Errors.Add(string.Format(CanvasExtedStrings.ErrorNoExistsAccountPeriodo, generateId));
                return result;
            }
            var resAccountPeriodo = _accountsApi.GetAccount(generateId);
            if (resAccountPeriodo.Value == null)
            {
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorAccountNotExist, generateId));
                return result;
            }
            if (string.IsNullOrEmpty(resAccountPeriodo.Value.SisId) ||
                resAccountPeriodo.Value.SisId.Split('-').Length != 2)
            {
                result.Errors.Add(string.Format(CanvasExtedStrings.ErrorNoExistsAccountPeriodo, generateId));
                return result;
            }
            if (int.Parse(resAccountPeriodo.Value.SisId.Split('-')[0]) != acccountGenerate.IdEstudio
                || int.Parse(resAccountPeriodo.Value.SisId.Split('-')[1]) != acccountGenerate.IdPeriodoActivo)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorAccountPeriodoSisIdCorrupto);
                return result;
            }
            var resAsignaturas = _wEstudios.ObtenerGruposConPersonasPorAsignatura(acccountGenerate.IdEstudio,
                acccountGenerate.IdPeriodoActivo);
            if (resAsignaturas.EsError)
            {
                result.Errors.Add("wS: " + resAsignaturas.ErrorDescripcionInterfaz);
                return result;
            }

            if (!resAsignaturas.Respuesta.Any())
            {
                result.Errors.Add(CanvasExtedStrings.ErrorNoAsignaturasWsPorEstudioPeriodoActivo);
                return result;
            }
            var asignatura = resAsignaturas.Respuesta.FirstOrDefault(a => a.idAsignatura == int.Parse(courseDemo.Value.SisId));
            if (asignatura == null)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorAsignaturaNoEstudioPeriodoActivo);
                return result;
            }
            //validar asignatura
            if (!asignatura.listaCursos.Any())
            {
                result.Errors.Add(CanvasExtedStrings.ErrorAsignaturaSinCursos);
                return result;
            }
            //validar asignatura
            var fechaIncioCurso = asignatura.listaCursos.Min(c => c.fechaInicioCurso);
            var fechaFinCurso = asignatura.listaCursos.Max(c => c.fechaFinCurso);
            if (!fechaIncioCurso.HasValue)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorGruposSinFechaInicio);
                return result;
            }
            if (!fechaFinCurso.HasValue)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorGruposSinFechaInicio);
                return result;
            }
            if (fechaIncioCurso.Value > fechaFinCurso.Value)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorGrupoFechaFinMenorFechaInicio);
                return result;
            }

            var progress = _context.ProgressInfo.Find(progressId);
            var resCourseGenerate = _coursesApi.GetBySisId(acccountGenerate.IdPeriodoActivo + "-" + courseDemo.Value.SisId);
            if (resCourseGenerate.Value != null)
            {
                var resSection = _coursesApi.GetSecctionsCourse(resCourseGenerate.Value.Id);
                if (resSection.HasErrors)
                {
                    result.Errors.AddRange(resSection.Errors);
                    return result;
                }
                foreach (var section in resSection.Elements)
                {
                    var resUpdateSection = _coursesApi.SetSisIdSecction(section.Id, string.Empty);
                    if (resUpdateSection.HasErrors)
                    {
                        result.Errors.AddRange(resUpdateSection.Errors);
                        return result;
                    }
                }
                if (resCourseGenerate.Value.GetState() != CourseState.Deleted)
                {
                    var resProgress = _accountsApi.DeleteCoursesAccount(acccountGenerate.Id, new[] { resCourseGenerate.Value.Id });
                    while (pending)
                    {
                        var resProg = _progressApi.Get(resProgress.Value.Id);
                        if (resProg.Value.GetState() == ProgressState.Failed)
                        {
                            result.Errors.Add(string.Format(CanvasExtedStrings.ErrorTareaDeleteCoursesAccount,
                                resAccountPeriodo.Value.Name));
                            return result;
                        }
                        if (resProg.Value.GetState() == ProgressState.Completed)
                        {
                            pending = false;
                        }
                    }
                }
                var courseGenerate = acccountGenerate.CoursesGenerates.FirstOrDefault(
                    c => c.SisId == (acccountGenerate.IdPeriodoActivo + "-" + courseDemo.Value.SisId));
                if (courseGenerate != null)
                {
                    _context.CourseGenerates.Remove(courseGenerate);
                    _context.SaveChanges();
                }
            }

            if (progress != null)
            {
                progress.Completion = 20;
                _context.SaveChanges();
            }
            var resCreateCourse = _accountsApi.CreateCourse(new CourseCanvasDto
            {
                AccountId = acccountGenerate.Id,
                Name = courseDemo.Value.Name,
                StartDate = fechaIncioCurso,
                EndDate = fechaFinCurso,
            }, acccountGenerate.IdPeriodoActivo, int.Parse(courseDemo.Value.SisId));
            if (resCreateCourse.HasErrors)
            {
                result.Errors.AddRange(resCreateCourse.Errors);
                return result;
            }
            var resMigrate = _coursesApi.CreateMigrationContent(courseDemo.Value.Id, resCreateCourse.Value.Id);
            pending = true;
            while (pending)
            {
                var resProg = _coursesApi.GetMigrationContent(resCreateCourse.Value.Id, resMigrate.Value.Id);
                if (resProg.Value.GetState() == MigrationState.Failed)
                {
                    result.Errors.Add(string.Format(CanvasApiStrings.ErrorMigrationContent,
                        resAccountPeriodo.Value.Name));
                    return result;
                }
                if (resProg.Value.GetState() == MigrationState.Completed)
                {
                    pending = false;
                }
            }

            if (progress != null)
            {
                progress.Completion = 40;
                _context.SaveChanges();
            }

            if (resMigrate.HasErrors)
            {
                result.Errors.AddRange(resMigrate.Errors);
                return result;
            }
            CreateCourseGenerate(resCreateCourse.Value, acccountGenerate.Id);

            foreach (var cursoCompletoDto in asignatura.listaCursos)
            {
                var resCreateSection = _coursesApi.CreateSection(resCreateCourse.Value.Id,
                    new SectionCanvasDto
                    {
                        Name = "Grupo " + cursoCompletoDto.iNumeroGrupoReal ?? "",
                        SisId = cursoCompletoDto.idCurso.ToString()
                    });
                if (resCreateSection.HasErrors)
                {
                    result.Errors.AddRange(resCreateSection.Errors);
                    return result;
                }
            }
            if (progress != null)
            {
                progress.Completion = 60;
                _context.SaveChanges();
            }

            var alumnos = asignatura.listaCursos.SelectMany(c => c.ListaPersonas)
                .Where(a => a.idTipoPersona == 1)
                .ToList();
            if (alumnos.Any())
            {
                var alumnosNuevos = new List<PersonaDto>();
                foreach (var personaDto in alumnos)
                {
                    var resUser = _userApi.GetBySisId(personaDto.idPersona.ToString());
                    if (resUser.Value == null)
                    {
                        if (string.IsNullOrEmpty(personaDto.login) || string.IsNullOrEmpty(personaDto.sNombrePersona)
                                   || string.IsNullOrEmpty(personaDto.sApellidosPersona)
                                   || string.IsNullOrEmpty(personaDto.correoElectronico))
                        {
                            //Guardar tabla de resultados
                            result.Value.Stundents.Add(new MotivoDto<PersonaDto>
                            {
                                Value = personaDto,
                                Code = (int)CodeMigrationIrregularidad.CodeMessage4
                            });
                            continue;
                        }
                        if (string.IsNullOrEmpty(personaDto.password) || personaDto.password.Length < 8)
                        {
                            //Guardar tabla de resultados
                            result.Value.Stundents.Add(new MotivoDto<PersonaDto>
                            {
                                Value = personaDto,
                                Code = (int)CodeMigrationIrregularidad.CodeMessage5
                            });
                            continue;
                        }
                        alumnosNuevos.Add(personaDto);
                    }
                }
                //Alumnos no existentes registrados como usuarios en canvas
                if (alumnosNuevos.Count > 0)
                {
                    var resImportedUsers = _accountsApi.ImportUsers(rootId, alumnosNuevos);
                    while (resImportedUsers.Value.Progress < 100)
                    {
                        resImportedUsers = _accountsApi.GetImported(rootId, resImportedUsers.Value.Id);
                        if (resImportedUsers.Value.GetState() == SisImportState.Failed)
                        {
                            result.Errors.Add(CanvasApiStrings.ErrrorMigrationAlumnos);
                            return result;
                        }
                    }
                }
                if (progress != null)
                {
                    progress.Completion = 80;
                    _context.SaveChanges();
                }
                var enrollments = alumnos.Select(a =>
                {
                    var sectionId = asignatura.listaCursos.FirstOrDefault(
                            c => c.ListaPersonas.Any(p => p.idPersona == a.idPersona));
                    if (sectionId == null) return null;
                    return new EnrollmentCanvasDto
                    {
                        CourseId = resCreateCourse.Value.SisId,
                        UserId = a.idPersona.ToString(),
                        Role = "student",
                        SectionId = sectionId.idCurso.ToString(),
                        Status = "active"
                    };
                }).Where(e => e != null).ToList();
                //Asociar alumnos o matricularlos
                if (enrollments.Any())
                {
                    var resImportedEnrollments = _accountsApi.ImportEnrollments(rootId, enrollments);
                    while (resImportedEnrollments.Value.Progress < 100)
                    {
                        resImportedEnrollments = _accountsApi.GetImported(rootId, resImportedEnrollments.Value.Id);
                        if (resImportedEnrollments.Value.GetState() == SisImportState.Failed)
                        {
                            result.Errors.Add(CanvasApiStrings.ErrorEnMigrationMatriculacion);
                            return result;
                        }
                    }
                }
            }
            if (progress != null)
            {
                progress.Completion = 90;
                _context.SaveChanges();
            }

            //Calcular Fechas de Tareas
            var oldAssignments = _coursesApi.GetAssignments(courseDemo.Value.Id);
            var newAssignments = _coursesApi.GetAssignments(resCreateCourse.Value.Id);
            var periodosNoLectivos = acccountGenerate.PeriodosNoLectivos.ToList();
            CalculateDates(courseDemo.Value.StartDate.Value, courseDemo.Value.EndDate.Value, fechaIncioCurso.Value,
                fechaFinCurso.Value, oldAssignments.Elements, newAssignments.Elements, periodosNoLectivos);
            foreach (var assignment in newAssignments.Elements)
            {
                if (assignment.ExtraData != null)
                {
                    result.Value.Assignments.Add(new MotivoDto<AssignmentCanvasDto>
                    {
                        Value = assignment,
                        Code = (int)assignment.ExtraData
                    });
                }
                var resUpdateAssignment = _coursesApi.UpdateAssignment(resCreateCourse.Value.Id, assignment);
                if (resUpdateAssignment.HasErrors)
                {
                    result.Errors.AddRange(resUpdateAssignment.Errors);
                    return result;
                }
            }
            if (progress != null)
            {
                progress.Completion = 100;
            }
            SaveLogMigration(result.Value, acccountGenerate, initialize);
            return result;
        }
        public ResultValue<CourseCanvasDto> RemoveCourse(int id)
        {
            var result = new ResultValue<CourseCanvasDto>();
            var course = _context.CourseGenerates.Find(id);
            if (course == null)
            {
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorCourseNotExist, id));
                return result;
            }
            result.Value = new CourseCanvasDto
            {
                Id = id,
                Name = course.Name
            };
            var resCourse = _coursesApi.Get(id);
            if (resCourse.HasErrors)
            {
                result.Errors.AddRange(resCourse.Errors);
                return result;
            }
            if (resCourse.Value.GetState() == CourseState.Available)
            {
                result.Errors.Add(CanvasApiStrings.ErrorCursoPublicado);
                return result;
            }
            var resSection = _coursesApi.GetSecctionsCourse(id);
            if (resSection.HasErrors)
            {
                result.Errors.AddRange(resSection.Errors);
                return result;
            }
            foreach (var section in resSection.Elements)
            {
                var resUpdateSection = _coursesApi.SetSisIdSecction(section.Id, string.Empty);
                if (resUpdateSection.HasErrors)
                {
                    result.Errors.AddRange(resUpdateSection.Errors);
                    return result;
                }
            }
            var resProgress = _accountsApi.DeleteCoursesAccount(course.AccountId, new[] { id });
            var pending = true;
            while (pending)
            {
                var resProg = _progressApi.Get(resProgress.Value.Id);
                if (resProg.Value.GetState() == ProgressState.Failed)
                {
                    result.Errors.Add(string.Format(CanvasExtedStrings.ErrorTareaDeleteCoursesAccount, course.Name));
                    return result;
                }
                if (resProg.Value.GetState() == ProgressState.Completed)
                {
                    pending = false;
                }
            }
            _context.CourseGenerates.Remove(course);
            _context.SaveChanges();
            return result;
        }
        public ResultValue<AccountCanvasDto> RemoveAccount(int id)
        {
            var result = new ResultValue<AccountCanvasDto>();
            var resAccount = _accountsApi.GetAccount(id);
            if (resAccount.HasErrors)
            {
                result.Errors.AddRange(resAccount.Errors);
                return result;
            }
            var resCoursesPeriodo = _accountsApi.GetCoursesAll(id);
            if (resCoursesPeriodo.HasErrors)
            {
                result.Errors.AddRange(resCoursesPeriodo.Errors);
                return result;
            }
            if (resCoursesPeriodo.Elements.Any(c => c.GetState() == CourseState.Available))
            {
                result.Errors.Add(CanvasExtedStrings.ErrorCursoPublicadoNoMigrate);
                return result;
            }
            var coursesNoDelete = resCoursesPeriodo.Elements.Where(c => c.GetState() != CourseState.Deleted).ToList();
            foreach (var course in coursesNoDelete)
            {
                var resSection = _coursesApi.GetSecctionsCourse(course.Id);
                if (resSection.HasErrors)
                {
                    result.Errors.AddRange(resSection.Errors);
                    return result;
                }
                foreach (var section in resSection.Elements)
                {
                    var resUpdateSection = _coursesApi.SetSisIdSecction(section.Id, string.Empty);
                    if (resUpdateSection.HasErrors)
                    {
                        result.Errors.AddRange(resUpdateSection.Errors);
                        return result;
                    }
                }
            }
            var deleteCourses = resCoursesPeriodo.Elements
                .Where(c => c.GetState() != CourseState.Deleted)
                .Select(a => a.Id).ToArray();
            if (deleteCourses.Length > 0)
            {
                var resProgress = _accountsApi.DeleteCoursesAccount(id, deleteCourses);
                var pending = true;
                while (pending)
                {
                    var resProg = _progressApi.Get(resProgress.Value.Id);
                    if (resProg.Value.GetState() == ProgressState.Failed)
                    {
                        result.Errors.Add(string.Format(CanvasExtedStrings.ErrorTareaDeleteCoursesAccount, resAccount.Value.Name));
                        return result;
                    }
                    if (resProg.Value.GetState() == ProgressState.Completed)
                    {
                        pending = false;
                    }
                }
            }
            var resUpdateAccount = _accountsApi.Update(new AccountSaveParameters
            {
                Id = id,
                Estudio = null
            });
            if (resUpdateAccount.HasErrors)
            {
                result.Errors.AddRange(resUpdateAccount.Errors);
                return result;
            }
            var accountGenerate = _context.AccountGenerates.Find(id);
            if (accountGenerate != null)
            {
                var cursosGenerados = accountGenerate.CoursesGenerates.ToList();
                foreach (var curso in cursosGenerados)
                {
                    _context.CourseGenerates.Remove(curso);
                }
                var periodosNoLectivos = accountGenerate.PeriodosNoLectivos.ToList();
                foreach (var periodo in periodosNoLectivos)
                {
                    _context.PeriodosNoLectivos.Remove(periodo);
                }
                var migraciones = accountGenerate.Migrations.ToList();
                foreach (var migracion in migraciones)
                {
                    _context.MigrationToCanvas.Remove(migracion);
                }
                _context.AccountGenerates.Remove(accountGenerate);
                _context.SaveChanges();
            }

            return result;
        }
        public ResultValue<ProgressInfoDto> GetProgress(int progressId)
        {
            var result = new ResultValue<ProgressInfoDto>();
            var progress = _context.ProgressInfo.Find(progressId);
            if (progress != null)
            {
                result.Value = new ProgressInfoDto
                {
                    Id = progress.Id,
                    Completion = progress.Completion
                };
            }
            return result;
        }
        public ResultValue<ResultMigrationDto> GetResult(int progressId)
        {
            var result = _dataCache.Get<ResultValue<ResultMigrationDto>>(string.Format(GlobalValues.CACHE_PROGRESS_INFO, progressId));
            if (result == null)
            {
                result = new ResultValue<ResultMigrationDto>();
                result.Errors.Add(CanvasApiStrings.ErrorNoResultMigration);
                return result;
            }
            return result;
        }
        public ResultList<MigrationToCanvasDto> GetMigrations(int accountId)
        {
            var result = new ResultList<MigrationToCanvasDto>();
            var migrations = _context.MigrationToCanvas.Where(m => m.GenerateId == accountId)
                .Select(m => new MigrationToCanvasDto
                {
                    Id = m.Id,
                    Generate = new AccountGenerateDto
                    {
                        Id = m.Generate.Id,
                        Name = m.Generate.Name
                    },
                    Inicio = m.Inicio,
                    Fin = m.Fin
                }).ToList();
            result.Elements = migrations;
            return result;
        }
        public ResultValue<ResultMigrationDto> GetMigration(int migrationId)
        {
            var result = new ResultValue<ResultMigrationDto>();
            var migration = _context.MigrationToCanvas.Find(migrationId);
            if (migration == null)
            {
                result.Errors.Add(CanvasApiStrings.ErrorMigrationNotExists);
                return result;
            }
            var deserialize = JsonConvert.DeserializeObject<ResultMigrationDto>(migration.Data);
            result.Value = deserialize;
            return result;
        }
        public ResultValue<bool> HasCourseGenerate(int generateId, int courseId)
        {
            var result = new ResultValue<bool>();
            var courseDemo = _coursesApi.Get(courseId);
            if (courseDemo.Value == null)
            {
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorCourseNotExist, courseId));
                return result;
            }
            if (string.IsNullOrEmpty(courseDemo.Value.SisId))
            {
                result.Errors.Add(CanvasExtedStrings.ErrorCourseNoSisId);
                return result;
            }
            var accountGenerate = _context.AccountGenerates.FirstOrDefault(a => a.Id == generateId);
            if (accountGenerate == null)
            {
                return result;
            }
            var resCourse = _coursesApi.GetBySisId(accountGenerate.IdPeriodoActivo + "-" + courseDemo.Value.SisId);
            result.Value = resCourse.Value != null;
            return result;
        }
        private Tuple<ResultValue<AccountCanvasDto>, AccountGenerate, ResultList<CourseCanvasDto>,
            ResultList<CourseCanvasDto>, RespuestaServicioOfArrayOfAsignaturaConCursosCompletosDtoWP8jzdkm>
            ValidateMigrarCursos(int accountId, ResultValue<ResultMigrationDto> result)
        {
            #region Validaciones

            var resAccountPeriodo = _accountsApi.GetAccount(accountId);
            if (resAccountPeriodo.HasErrors)
            {
                result.Errors.AddRange(resAccountPeriodo.Errors);
                return null;
            }
            var acccountGenerate = _context.AccountGenerates.Find(accountId);
            if (acccountGenerate == null)
            {
                result.Errors.Add(string.Format(CanvasExtedStrings.ErrorNoExistsAccountPeriodo, accountId));
                return null;
            }
            if (string.IsNullOrEmpty(resAccountPeriodo.Value.SisId) ||
                resAccountPeriodo.Value.SisId.Split('-').Length != 2)
            {
                result.Errors.Add(string.Format(CanvasExtedStrings.ErrorNoExistsAccountPeriodo, accountId));
                return null;
            }

            if (int.Parse(resAccountPeriodo.Value.SisId.Split('-')[0]) != acccountGenerate.IdEstudio
                || int.Parse(resAccountPeriodo.Value.SisId.Split('-')[1]) != acccountGenerate.IdPeriodoActivo)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorAccountPeriodoSisIdCorrupto);
                return null;
            }
            var resCoursesDemo = _accountsApi.GetCourses(acccountGenerate.AccountId);
            if (resCoursesDemo.HasErrors)
            {
                result.Errors.AddRange(resCoursesDemo.Errors);
                return null;
            }
            var resCoursesPeriodo = _accountsApi.GetCoursesAll(accountId);
            if (resCoursesPeriodo.HasErrors)
            {
                result.Errors.AddRange(resCoursesPeriodo.Errors);
                return null;
            }
            if (resCoursesPeriodo.Elements.Any(c => c.GetState() == CourseState.Available))
            {
                result.Errors.Add(CanvasExtedStrings.ErrorCursoPublicadoNoMigrate);
                return null;
            }
            if (resCoursesDemo.Elements.Any(c => !c.StartDate.HasValue || !c.EndDate.HasValue))
            {
                var first = resCoursesDemo.Elements.First(c => !c.StartDate.HasValue || !c.EndDate.HasValue);
                result.Errors.Add(string.Format(CanvasExtedStrings.ErrorCursosSinFechaConfigurada, first.Name));
                return null;
            }

            var resAsignaturas = _wEstudios.ObtenerGruposConPersonasPorAsignatura(acccountGenerate.IdEstudio,
                acccountGenerate.IdPeriodoActivo);
            if (resAsignaturas.EsError)
            {
                result.Errors.Add("wS: " + resAsignaturas.ErrorDescripcionInterfaz);
                return null;
            }

            if (!resAsignaturas.Respuesta.Any())
            {
                result.Errors.Add(CanvasExtedStrings.ErrorNoAsignaturasWsPorEstudioPeriodoActivo);
                return null;
            }
            return new Tuple<ResultValue<AccountCanvasDto>, AccountGenerate, ResultList<CourseCanvasDto>,
                ResultList<CourseCanvasDto>, RespuestaServicioOfArrayOfAsignaturaConCursosCompletosDtoWP8jzdkm>
                (resAccountPeriodo, acccountGenerate, resCoursesDemo, resCoursesPeriodo, resAsignaturas);

            #endregion
        }
        private void CalculateDates(DateTime oldInicioCurso, DateTime oldFinCurso, DateTime newInicioCurso, DateTime newFinCurso,
            List<AssignmentCanvasDto> oldTasks, List<AssignmentCanvasDto> newTasks, List<PeriodoNoLectivo> noLectivos)
        {
            oldTasks = oldTasks.OrderBy(t => t.GroupId).ThenBy(t => t.Position).ToList();
            newTasks = newTasks.OrderBy(t => t.GroupId).ThenBy(t => t.Position).ToList();
            for (var index = 0; index < oldTasks.Count; index++)
            {
                var old = oldTasks[index];
                var @new = newTasks[index];
                var fueraDeRango = false;
                if (old.StartDate.HasValue && old.StartDate.Value < oldInicioCurso || old.StartDate > oldFinCurso)
                {
                    @new.StartDate = null;
                    old.StartDate = null;
                    fueraDeRango = true;
                    @new.ExtraData = (int)CodeMigrationIrregularidad.CodeMessage6;
                }
                if (old.PresentationDate.HasValue && old.PresentationDate.Value < oldInicioCurso || old.PresentationDate > oldFinCurso)
                {
                    @new.PresentationDate = null;
                    old.PresentationDate = null;
                    fueraDeRango = true;
                    @new.ExtraData = (int)CodeMigrationIrregularidad.CodeMessage7;
                }
                if (old.EndDate.HasValue && old.EndDate.Value < oldInicioCurso || old.EndDate > oldFinCurso)
                {
                    @new.EndDate = null;
                    old.EndDate = null;
                    fueraDeRango = true;
                    @new.ExtraData = (int)CodeMigrationIrregularidad.CodeMessage8;
                }
                var result = CalculateDatesTasks(oldInicioCurso, newInicioCurso, newFinCurso, old, @new, noLectivos);
                @new.Overflow = fueraDeRango || result.Item1 || result.Item2 || result.Item3;
            }
        }
        private Tuple<bool, bool, bool> CalculateDatesTasks(DateTime oldInicioCurso, DateTime newInicioCurso, DateTime newFinCurso,
            AssignmentCanvasDto oldTask, AssignmentCanvasDto newTask, List<PeriodoNoLectivo> noLectivos)
        {
            bool inicioOverflow = false, presentationOverflow = false, finOverflow = false;
            var daysOfValidate = GetDatesInRange(newInicioCurso, newFinCurso, noLectivos);
            if (oldTask.StartDate.HasValue)
            {
                var days = (oldTask.StartDate.Value - oldInicioCurso).Days;
                var dateCalc = newInicioCurso.AddDays(days).Date;

                var dateValid = daysOfValidate.FirstOrDefault(d => dateCalc == d) ??
                                  daysOfValidate.FirstOrDefault(d => d > dateCalc);
                if (dateValid.HasValue)
                {
                    newTask.StartDate = dateValid.Value.AddHours(oldTask.StartDate.Value.Hour)
                        .AddMinutes(oldTask.StartDate.Value.Minute);
                }
                else
                {
                    inicioOverflow = true;
                    newTask.StartDate = null;
                    newTask.ExtraData = (int)CodeMigrationIrregularidad.CodeMessage9;
                }
            }
            if (oldTask.PresentationDate.HasValue)
            {
                var days = (oldTask.PresentationDate.Value - oldInicioCurso).Days;
                var dateCalc = newInicioCurso.AddDays(days).Date;

                var dateValid = daysOfValidate.FirstOrDefault(d => dateCalc == d) ??
                                  daysOfValidate.FirstOrDefault(d => d > dateCalc);
                if (dateValid.HasValue)
                {
                    newTask.PresentationDate = dateValid.Value.AddHours(oldTask.PresentationDate.Value.Hour)
                        .AddMinutes(oldTask.PresentationDate.Value.Minute);
                }
                else
                {
                    presentationOverflow = true;
                    newTask.PresentationDate = null;
                    newTask.ExtraData = (int)CodeMigrationIrregularidad.CodeMessage10;
                }
            }
            if (oldTask.EndDate.HasValue)
            {
                var days = (oldTask.EndDate.Value - oldInicioCurso).Days;
                var dateCalc = newInicioCurso.AddDays(days).Date;

                var dateValid = daysOfValidate.FirstOrDefault(d => dateCalc == d) ??
                                  daysOfValidate.FirstOrDefault(d => d > dateCalc);
                if (dateValid.HasValue)
                {
                    newTask.EndDate = dateValid.Value.AddHours(oldTask.EndDate.Value.Hour)
                        .AddMinutes(oldTask.EndDate.Value.Minute);
                }
                else
                {
                    finOverflow = true;
                    newTask.EndDate = null;
                    newTask.ExtraData = (int)CodeMigrationIrregularidad.CodeMessage11;
                }
            }
            return new Tuple<bool, bool, bool>(inicioOverflow, presentationOverflow, finOverflow);
        }
        private List<DateTime?> GetDatesInRange(DateTime start, DateTime end, List<PeriodoNoLectivo> noLectivos)
        {
            var list = new List<DateTime?>();
            for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday) continue;
                if (!noLectivos.Any(n => date >= n.Inicio && date <= n.Fin))
                {
                    list.Add(date.Date);
                }
            }
            return list;
        }
        private void CreateCourseGenerate(CourseCanvasDto course, int accountId)
        {
            var courseGenerate = new CourseGenerate
            {
                Id = course.Id,
                AccountId = accountId,
                Name = course.Name,
                Inicio = course.StartDate.Value,
                Fin = course.EndDate.Value,
                SisId = course.SisId
            };

            _context.CourseGenerates.Add(courseGenerate);
            _context.SaveChanges();
        }
        private void SaveLogMigration(ResultMigrationDto migration, AccountGenerate account, DateTime initialize)
        {
            var serialize = JsonConvert.SerializeObject(migration);
            var migrate = new MigrationToCanvas
            {
                Generate = account,
                Data = serialize,
                Inicio = initialize.ToUniversalTime(),
                Fin = DateTime.UtcNow
            };
            _context.MigrationToCanvas.Add(migrate);
            _context.SaveChanges();
        }
    }
}