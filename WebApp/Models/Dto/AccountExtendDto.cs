using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Dto
{
    public class AccountExtendDto
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public int IdEstudio { get; set; }
        public virtual EstudioDto Estudio { get; set; }
        public virtual IEnumerable<AsignaturaDto> Asignaturas { get; set; }
        public virtual IEnumerable<PeriodoActivoDto> PeriodoActivos { get; set; }
    }
}