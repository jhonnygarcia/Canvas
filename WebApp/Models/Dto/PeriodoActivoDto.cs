using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Dto
{
    public class PeriodoActivoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string AnioAcademico { get; set; }
        public int? NroPeriodo { get; set; }
        public int? AccountIdPeriodo { get; set; }
        public virtual IEnumerable<AccountExtendDto> AccountExtends { get; set; }
    }
}