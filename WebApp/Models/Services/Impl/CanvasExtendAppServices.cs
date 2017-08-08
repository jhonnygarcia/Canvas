using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DataCacheServices.AppDataCache;
using WebApp.App_Start;
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
    public class CanvasExtendAppServices : ICanvasExtendAppServices
    {
        private readonly IAccountsApiCanvas _accountsApi;
        private readonly CanvasExtendContenxt _context;
        private readonly IEstudios _wEstudios;
        private readonly ICoursesApiCanvas _coursesApi;
        public CanvasExtendAppServices(CanvasExtendContenxt context, IAccountsApiCanvas accountsApi, IEstudios wEstudios, ICoursesApiCanvas coursesApi)
        {
            _context = context;
            _accountsApi = accountsApi;
            _wEstudios = wEstudios;
            _coursesApi = coursesApi;
        }
        private bool Solapado(DateTime inicioA, DateTime finA, DateTime inicioB, DateTime finB)
        {
            return (finA >= inicioB && inicioB >= finA)
                   || (finB >= inicioA && finA >= inicioB);
        }
        public ResultValue<AccountExtendDto> GetAccountExted(int id)
        {
            var result = new ResultValue<AccountExtendDto>();
            var account = _context.AccountExtends.Where(a => a.AccountId == id).Select(a => new AccountExtendDto
            {
                AccountId = a.AccountId,
                Name = a.Name,
                IdEstudio = a.IdEstudio,
                Estudio = new EstudioDto
                {
                    Id = a.Estudio.Id,
                    Nombre = a.Estudio.Nombre
                },
                Asignaturas = a.Asignaturas.Select(aa => new AsignaturaDto
                {
                    Id = aa.Id,
                    Nombre = aa.Nombre
                }),
                PeriodoActivos = a.PeriodoActivos.Select(p => new PeriodoActivoDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    AnioAcademico = p.AnioAcademico,
                    NroPeriodo = p.NroPeriodo
                })
            }).FirstOrDefault();
            if (account == null)
            {
                result.Errors.Add(string.Format(CanvasExtedStrings.ErrorNoExisteAccountExtend, id));
                return result;
            }
            var coursesAsociados = _context.CourseExtends.Where(c => c.AccountId == id).ToList();
            foreach (var coursesAsociado in coursesAsociados)
            {
                var asignatura = account.Asignaturas.FirstOrDefault(a => a.Id == coursesAsociado.IdAsignatura);
                if (asignatura != null)
                {
                    asignatura.CourseId = coursesAsociado.CourseId;
                }
            }
            account.Asignaturas = account.Asignaturas.OrderByDescending(a => a.CourseId.HasValue).ToList();

            var accountsGenerates = _context.AccountGenerates.Where(a => a.AccountId == id).ToList();
            foreach (var accountsGenerate in accountsGenerates)
            {
                var periodoActivo = account.PeriodoActivos.FirstOrDefault(p => p.Id == accountsGenerate.IdPeriodoActivo);
                if (periodoActivo != null)
                {
                    periodoActivo.AccountIdPeriodo = accountsGenerate.Id;
                }
            }
            account.PeriodoActivos = account.PeriodoActivos.OrderByDescending(a => a.AccountIdPeriodo.HasValue).ToList();

            result.Value = account;
            return result;
        }
        public ResultValue<CourseExtendDto> GetCourseExted(int id)
        {
            var result = new ResultValue<CourseExtendDto>();
            var course = _context.CourseExtends.Where(c => c.CourseId == id).Select(c => new CourseExtendDto
            {
                IdAsignatura = c.IdAsignatura,
                CourseId = c.CourseId,
                Name = c.Name,
                Asignatura = new AsignaturaDto
                {
                    Id = c.Asignatura.Id,
                    Nombre = c.Asignatura.Nombre
                },
                Account = new AccountExtendDto
                {
                    AccountId = c.Account.AccountId,
                    Name = c.Account.Name
                }
            }).FirstOrDefault();
            if (course == null)
            {
                result.Errors.Add(string.Format(CanvasExtedStrings.ErrorNoExisteCourseExtend, id));
                return result;
            }
            result.Value = course;
            return result;
        }
        public ResultValue<bool> VerificarDatos(int accountId)
        {
            /*
             * Errors = Errores que deben detener la tarea de sincronizacion
             * Warnings = Se notifica pero se continua con la sincronizacion
             * Messages = Se notifica pero se continua con la sincronizacion
             *            Existen Cursos con un SIS-ID de una asignatura que ya no lo trae el webServices del gestor
             */
            var result = new ResultValue<bool>();
            var preData = PreValidarDatos(result, accountId);
            if (result.HasErrors)
            {
                return result;
            }
            var resAccount = preData.Item1;
            var resAccountExtend = preData.Item3;

            var resAsignaturas = _wEstudios.ObtenerAsignaturasDeEstudio(int.Parse(resAccount.Value.SisId));
            var distinctLengthAsignaturas = resAccountExtend.Value.Asignaturas.Count() !=
                       resAsignaturas.Respuesta.Length;
            if (distinctLengthAsignaturas)
            {
                result.Warnings.Add(CanvasExtedStrings.ErrorCantidadAsignaturasDiferentes);
                return result;
            }
            else
            {
                if (resAsignaturas.Respuesta.Select(a => a.idAsignatura)
                    .Except(resAccountExtend.Value.Asignaturas.Select(a => a.Id)).Any())
                {
                    result.Warnings.Add(CanvasExtedStrings.ErrorAsignaturasDiferentes);
                    return result;
                }
            }

            var resPeriodosActivos = _wEstudios.ObtenerCursosDeAsignaturasDeEstudio(int.Parse(resAccount.Value.SisId));
            var distinctLengthPeriodoActivos = resPeriodosActivos.Respuesta.listaPeriodosActivos.Length !=
                                               resAccountExtend.Value.PeriodoActivos.Count();
            if (distinctLengthPeriodoActivos)
            {
                result.Warnings.Add(CanvasExtedStrings.ErrorCantidadPeriodoActivosDiferentes);
                return result;
            }
            else
            {
                if (resPeriodosActivos.Respuesta.listaPeriodosActivos.Select(a => a.idPeriodoMatriculacion)
                    .Except(resAccountExtend.Value.PeriodoActivos.Select(a => a.Id)).Any())
                {
                    result.Warnings.Add(CanvasExtedStrings.ErrorPeriodoActivosDiferentes);
                    return result;
                }
            }
            var courses = _context.CourseExtends.Where(a => a.AccountId == accountId)
                                  .Select(a => a.IdAsignatura).ToArray();
            var noContiene = resAccountExtend.Value.Asignaturas.All(a => !courses.Contains(a.Id));
            if (noContiene)
            {
                result.Messages.Add(CanvasExtedStrings.ErrorCourseSisIdNoAsignatura);
                return result;
            }
            result.Value = !result.HasErrors && !result.HasWarnings && !result.HasMessages;
            return result;
        }
        private Tuple<ResultValue<AccountCanvasDto>, ResultList<CourseCanvasDto>, ResultValue<AccountExtendDto>> PreValidarDatos(BaseResult result, int accountId)
        {
            var resAccount = _accountsApi.GetAccount(accountId);
            if (resAccount.HasErrors)
            {
                result.Errors.AddRange(resAccount.Errors);
                return null;
            }
            var resCourses = _accountsApi.GetCourses(accountId);
            if (resCourses.HasErrors)
            {
                result.Errors.AddRange(resCourses.Errors);
                return null;
            }
            if (string.IsNullOrEmpty(resAccount.Value.SisId))
            {
                result.Errors.Add(CanvasExtedStrings.ErrorAccountNoSisId);
                return null;
            }
            var resAccountExtend = GetAccountExted(accountId);
            if (resAccountExtend.HasErrors)
            {
                result.Errors.AddRange(resAccountExtend.Errors);
                return null;
            }
            if (resAccountExtend.Value.IdEstudio != int.Parse(resAccount.Value.SisId))
            {
                result.Errors.Add(CanvasExtedStrings.ErrorSisIdAccountCambiado);
                return null;
            }
            var isCourseNotSisId = resCourses.Elements.Any(c => string.IsNullOrEmpty(c.SisId));
            if (isCourseNotSisId)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorCourseNotSisId);
                return null;
            }
            var isCoursePublish = resCourses.Elements.Any(c => c.GetState() == CourseState.Available);
            if (isCoursePublish)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorCoursePublish);
                return null;
            }
            var asignaturasAsociadas = resAccountExtend.Value.Asignaturas.Where(a => a.CourseId.HasValue).ToList();
            foreach (var courseCanvasDto in resCourses.Elements)
            {
                if (asignaturasAsociadas.All(a => a.Id != int.Parse(courseCanvasDto.SisId)))
                {
                    result.Errors.Add(string.Format(CanvasExtedStrings.ErrorCursoConSisIdFaild, courseCanvasDto.Name,
                        resAccountExtend.Value.Estudio.Nombre));
                    return null;
                }
                var asignatura = asignaturasAsociadas.FirstOrDefault(a => a.Id == int.Parse(courseCanvasDto.SisId));
                if (asignatura == null)
                {
                    result.Errors.Add(string.Format(CanvasExtedStrings.ErrorSisIdCourseCambiado, courseCanvasDto.Name));
                    return null;
                }
            }
            var returnData = new Tuple<ResultValue<AccountCanvasDto>, ResultList<CourseCanvasDto>, ResultValue<AccountExtendDto>>(
                resAccount, resCourses, resAccountExtend);
            return returnData;
        }
        public ResultValue<AccountExtendDto> UpdateAccountExtend(int id)
        {
            var result = new ResultValue<AccountExtendDto>();
            var preData = PreValidarDatos(result, id);
            if (result.HasErrors)
            {
                return result;
            }
            var resAccount = preData.Item1;
            var resCourses = preData.Item2;
            var resAccountExtend = preData.Item3;

            var estudiosGestor = _wEstudios.ObtenerEstudiosUNIR(0);
            var estudioGestor = estudiosGestor.Respuesta.FirstOrDefault(e => e.idEstudio == int.Parse(resAccount.Value.SisId));
            if (estudioGestor == null)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorEstudioNoExiste);
                return result;
            }
            foreach (var courseCanvasDto in resCourses.Elements)
            {
                var asignatura = resAccountExtend.Value.Asignaturas.FirstOrDefault(a => a.Id == int.Parse(courseCanvasDto.SisId));
                if (asignatura == null)
                {
                    _coursesApi.Update(new CourseSaveParameters
                    {
                        Id = courseCanvasDto.Id,
                        Asignatura = null
                    });
                }
            }

            var accountExtend = _context.AccountExtends.Find(id);
            accountExtend.Estudio.Nombre = estudioGestor.sNombreEstudio;
            accountExtend.PeriodoActivos.Clear();
            accountExtend.Asignaturas.Clear();

            /*****Guardar Asignaturas Asociadas al estudio*****/
            var resAsignaturas = _wEstudios.ObtenerAsignaturasDeEstudio(int.Parse(resAccount.Value.SisId));
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
            var resPeriodosActivos = _wEstudios.ObtenerCursosDeAsignaturasDeEstudio(int.Parse(resAccount.Value.SisId));
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
            return result;
        }
        public ResultList<PeriodoActivoDto> GetPeriodoActivos(string searchText, int pageIndex, int? pageCount, int accountId)
        {
            var result = new ResultList<PeriodoActivoDto>();
            var account = _context.AccountExtends.Find(accountId);
            if (account == null)
            {
                result.Errors.Add(string.Format(CanvasApiStrings.ErrorAccountNotExist, accountId));
                return result;
            }
            pageCount = pageCount ?? 10;
            var query = account.PeriodoActivos.AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(
                    e => e.Nombre.ToLower().Trim().Contains(searchText.ToLower().Trim()));
            }
            var listado = query.Skip((pageIndex - 1) * pageCount.Value)
                    .Take(pageCount.Value).ToList();

            result.Elements = listado.Select(a => new PeriodoActivoDto
            {
                Id = a.Id,
                Nombre = a.Nombre
            }).ToList();
            result.TotalElements = query.Count();
            result.PageCount = listado.Count;

            return result;
        }
        public ResultValue<AccountGenerateDto> GetAccountPeriodo(int id)
        {
            var result = new ResultValue<AccountGenerateDto>();
            var accountGenerate = _context.AccountGenerates.Find(id);
            if (accountGenerate == null)
            {
                result.Errors.Add(string.Format(CanvasExtedStrings.ErrorNoExistsAccountPeriodo, id));
                return result;
            }
            var resAccount = _accountsApi.GetAccount(id);
            if (resAccount.HasErrors)
            {
                result.Errors.AddRange(resAccount.Errors);
                return result;
            }
            result.Value = new AccountGenerateDto
            {
                Id = accountGenerate.Id,
                Name = accountGenerate.Name,
                NombrePeriodoMatriculacion = accountGenerate.NombrePeriodoMatriculacion,
                IdEstudio = accountGenerate.IdEstudio,
                IdPeriodoActivo = accountGenerate.IdPeriodoActivo,
                AnioAcademico = accountGenerate.AnioAcademico,
                FechaInicio = accountGenerate.FechaInicio,
                FechaFin = accountGenerate.FechaFin,
                AccountCanvas = new AccountCanvasDto
                {
                    Id = resAccount.Value.Id,
                    SisId = resAccount.Value.SisId,
                    Name = resAccount.Value.Name,
                    ParentId = resAccount.Value.ParentId,
                    IsAccountPeriodo = true,
                    WorkflowState = resAccount.Value.WorkflowState
                },
                PeriodosNoLectivos = accountGenerate.PeriodosNoLectivos.Select(p => new PeriodoNoLectivoDto
                {
                    Id = p.Id,
                    Inicio = p.Inicio,
                    Fin = p.Fin
                })
            };
            return result;
        }
        public ResultValue<bool> VerificarPeriodosNoLectivos(PeriodoNoLectivosSaveParameters model)
        {
            var result = new ResultValue<bool>();
            var accountPeriodo = _context.AccountGenerates.Find(model.AccountId);
            if (accountPeriodo == null)
            {
                result.Errors.Add(string.Format(CanvasExtedStrings.ErrorNoExisteAccountExtend, model.AccountId));
                return result;
            }
            var resCourses = _accountsApi.GetCourses(model.AccountId);
            if (resCourses.HasErrors)
            {
                result.Errors.AddRange(resCourses.Errors);
                return result;
            }
            var isCoursePublish = resCourses.Elements.Any(c => c.GetState() == CourseState.Available);
            if (isCoursePublish)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorCoursePublish);
                return result;
            }
            var isFechaFueraPeriodo = model.Fechas.Any(rango =>
            {
                if (rango.Inicio > accountPeriodo.FechaFin || rango.Inicio < accountPeriodo.FechaInicio)
                    return true;
                if (rango.Fin < accountPeriodo.FechaInicio || rango.Fin > accountPeriodo.FechaFin)
                    return true;
                return false;
            });
            var newId = -1;
            foreach (var rangoFecha in model.Fechas)
            {
                rangoFecha.Id = --newId;
            }
            var sobrePuesto = model.Fechas.Any(rango1 =>
            {
                var primero = model.Fechas.First(f => f.Inicio == rango1.Inicio && f.Fin == rango1.Fin);
                var rangosVerify = model.Fechas.Where(f => f.Id != primero.Id).ToList();
                var solapado = rangosVerify.Any(rango2 => Solapado(rango1.Inicio, rango1.Fin, rango2.Inicio, rango2.Fin));
                return solapado;
            });
            result.Value = !sobrePuesto && !isFechaFueraPeriodo;
            if (isFechaFueraPeriodo)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorFechasNoPeriodoActivo);
            }
            if (sobrePuesto)
            {
                result.Errors.Add(CanvasExtedStrings.ErrorFechasSobrePuestas);
            }
            return result;
        }
        public ResultValue<AccountGenerateDto> GuardarPeriodosNoLectivos(PeriodoNoLectivosSaveParameters model)
        {
            var result = new ResultValue<AccountGenerateDto>();
            var resValid = VerificarPeriodosNoLectivos(model);
            if (resValid.HasErrors)
            {
                result.Errors.AddRange(resValid.Errors);
                return result;
            }
            var accountGenerate = _context.AccountGenerates.Find(model.AccountId);
            var periodosNoLectivos = accountGenerate.PeriodosNoLectivos.ToList();
            var ids = model.Fechas.Where(f => f.Id.HasValue).Select(f => f.Id.Value).ToArray();
            var eliminados = periodosNoLectivos.Where(p => !ids.Contains(p.Id)).ToList();
            foreach (var periodoNoLectivo in eliminados)
            {
                _context.PeriodosNoLectivos.Remove(periodoNoLectivo);
            }

            foreach (var rangoFecha in model.Fechas)
            {
                var periodoNoLectivo = accountGenerate.PeriodosNoLectivos.FirstOrDefault(p => p.Id == rangoFecha.Id);
                if (periodoNoLectivo == null)
                {
                    periodoNoLectivo = new PeriodoNoLectivo
                    {
                        Inicio = rangoFecha.Inicio,
                        Fin = rangoFecha.Fin
                    };
                }
                else
                {
                    periodoNoLectivo.Inicio = rangoFecha.Inicio;
                    periodoNoLectivo.Fin = rangoFecha.Fin;
                }
                accountGenerate.PeriodosNoLectivos.Add(periodoNoLectivo);
            }
            _context.SaveChanges();
            result.Value = new AccountGenerateDto
            {
                Id = accountGenerate.Id,
                Name = accountGenerate.Name,
                PeriodosNoLectivos = accountGenerate.PeriodosNoLectivos.Select(p => new PeriodoNoLectivoDto
                {
                    Id = p.Id,
                    Inicio = p.Inicio,
                    Fin = p.Fin
                })
            };
            return result;
        }
        public ResultList<AccountGenerateDto> GetAccountGenerates(string searchText, int pageIndex, int? pageCount,
            int accountId)
        {
            var result = new ResultList<AccountGenerateDto>();
            var query = _context.AccountGenerates.Where(a => a.AccountId == accountId).AsQueryable();
            pageCount = pageCount ?? 10;
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(e => e.Name.Contains(searchText));
            }
            query = query.OrderBy(a => a.Name);
            var listado = query.Skip((pageIndex - 1)*pageCount.Value)
                .Take(pageCount.Value).Select(a => new AccountGenerateDto
                {
                    Id = a.Id,
                    Name = a.Name
                }).ToList();
            result.Elements = listado;
            result.TotalElements = query.Count();
            result.PageCount = listado.Count;
            return result;
        }
    }
}