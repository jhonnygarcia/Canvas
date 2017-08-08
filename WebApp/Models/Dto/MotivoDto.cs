using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UNIR.Servicios.Integracion.Negocio.ClasesDto;
using WebApp.Common.ReturnsCanvas;

namespace WebApp.Models.Dto
{
    public sealed class MotivoDto<T> where T : class
    {
        public T Value { get; set; }
        public int Code { get; set; }
    }
}