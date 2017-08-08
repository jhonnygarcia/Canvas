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
    [RoutePrefix("api/v1/clientes")]
    public class ClienteController : ApiControllerBase
    {
        public ClienteController() { }

        [HttpPost]
        [Route("advanced-search")]
        public IHttpActionResult Search(ClienteSearchParameters model)
        {
            Func<DateTime> randomDateTime = () =>
            {
                var gen = new Random();
                var start = new DateTime(1995, 1, 1);
                int range = (DateTime.Today - start).Days;
                return start.AddDays(gen.Next(range));
            };

            var entity = new
            {
                Id = new Random().Next(1, 100),
                Nombre = Guid.NewGuid().ToString(),
                Direccion = Guid.NewGuid().ToString(),
                Salario = new Random().Next(100, 1000),
                Pais = Guid.NewGuid().ToString(),
                FechaNacimiento = randomDateTime()
            };

            var resultado = new List<object>();
            for (var i = 0; i < 10; i++)
            {
                resultado.Add(entity);
            }
            return Ok(new
            {
                Data = resultado,
                TotalElements = resultado.Count * 3
            });
        }
    }
}