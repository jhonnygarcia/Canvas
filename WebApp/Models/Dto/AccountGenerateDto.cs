using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Common.ReturnsCanvas;
using WebApp.Models.Model.Entity;

namespace WebApp.Models.Dto
{
    public class AccountGenerateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NombrePeriodoMatriculacion { get; set; }
        public int AccountId { get; set; }
        public int IdEstudio { get; set; }
        public int IdPeriodoActivo { get; set; }
        public string AnioAcademico { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public AccountExtendDto AccountExtend { get; set; }
        public EstudioDto Estudio { get; set; }
        public PeriodoActivoDto PeriodoActivo { get; set; }
        public AccountCanvasDto AccountCanvas { get; set; }
        public IEnumerable<PeriodoNoLectivoDto> PeriodosNoLectivos { get; set; }
    }
}