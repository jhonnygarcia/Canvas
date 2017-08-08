using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using Antlr.Runtime.Misc;
using DataCacheServices.AppDataCache;
using UNIR.Servicios.Integracion.Negocio.ClasesDto;
using WebApp.App_Start;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Common.WepApi;
using WebApp.Globalization.Services;

namespace WebApp.Models.Services.Impl
{
    public class GestorAppServices : IGestorAppServices
    {
        private readonly IEstudios _estudiosService;
        private readonly IDataCacheService _dataCache;
        public GestorAppServices(IEstudios estudiosService, IDataCacheService dataCache)
        {
            _estudiosService = estudiosService;
            _dataCache = dataCache;
        }
        public ResultList<EstudioDto> GetEstudios(string searchText, int pageIndex, int? pageCount)
        {
            var result = new ResultList<EstudioDto>();
            UNIR.Comun.Servicios.RespuestaServicioOfArrayOfEstudioDtoWP8jzdkm resEstudios;
            var cache = _dataCache.Get<UNIR.Comun.Servicios.RespuestaServicioOfArrayOfEstudioDtoWP8jzdkm>(GlobalValues.CACHE_ESTUDIO, GlobalValues.GROUP_CACHE_GESTOR);
            if (cache != null)
            {
                resEstudios = cache;
            }
            else
            {
                var configMinutes = ConfigurationManager.AppSettings["TimeMinutesWsGestor"];
                var minutes = 0;
                if (!int.TryParse(configMinutes, out minutes))
                {
                    minutes = GlobalValues.DEFAULT_MINUTE_CACHE;
                }
                resEstudios = _estudiosService.ObtenerEstudiosUNIR(0);
                _dataCache.Put(GlobalValues.CACHE_ESTUDIO, resEstudios, GlobalValues.GROUP_CACHE_GESTOR, TimeSpan.FromMinutes(minutes));
            }

            pageCount = pageCount ?? 10;
            var query = resEstudios.Respuesta.AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(
                    e => e.sNombreEstudio.ToLower().Trim().Contains(searchText.ToLower().Trim()));
            }
            var listado = query.Skip((pageIndex - 1) * pageCount.Value)
                    .Take(pageCount.Value).ToList();

            result.Elements = listado;
            result.TotalElements = query.Count();
            result.PageCount = listado.Count;

            return result;
        }
        public ResultList<AsignaturaDto> GetAsignaturas(string searchText, int pageIndex, int? pageCount, int idEstudio)
        {
            var result = new ResultList<AsignaturaDto>();
            
            UNIR.Comun.Servicios.RespuestaServicioOfArrayOfAsignaturaDtoWP8jzdkm resAsignaturas;
            var keyCache = string.Format(GlobalValues.CACHE_ASIGNATURAS_ESTUDIO, idEstudio);
            var cache = _dataCache.Get<UNIR.Comun.Servicios.RespuestaServicioOfArrayOfAsignaturaDtoWP8jzdkm>(keyCache, GlobalValues.GROUP_CACHE_GESTOR);
            if (cache != null)
            {
                resAsignaturas = cache;
            }
            else
            {
                var configMinutes = ConfigurationManager.AppSettings["TimeMinutesWsGestor"];
                var minutes = 0;
                if (!int.TryParse(configMinutes, out minutes))
                {
                    minutes = GlobalValues.DEFAULT_MINUTE_CACHE;
                }
                resAsignaturas = _estudiosService.ObtenerAsignaturasDeEstudio(idEstudio);
                _dataCache.Put(keyCache, resAsignaturas, GlobalValues.GROUP_CACHE_GESTOR, TimeSpan.FromMinutes(minutes));
            }

            if (resAsignaturas.EsError)
            {
                result.Errors.Add(string.Format(GestorWsStrings.ErrorWebServices, "ObtenerEstudiosUNIR(" + idEstudio + ")"));
                return result;
            }
            pageCount = pageCount ?? 10;
            var query = resAsignaturas.Respuesta.AsQueryable();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(
                    e => e.sNombreAsignatura.ToLower().Trim().Contains(searchText.ToLower().Trim()));
            }
            var listado = query.Skip((pageIndex - 1) * pageCount.Value)
                    .Take(pageCount.Value).ToList();

            result.Elements = listado;
            result.TotalElements = query.Count();
            result.PageCount = listado.Count;

            return result;
        }
    }
}