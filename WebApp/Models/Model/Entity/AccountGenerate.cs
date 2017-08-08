using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Model.Entity
{
    public class AccountGenerate
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
        public virtual AccountExtend Account { get; set; }
        public virtual Estudio Estudio { get; set; }
        public virtual  PeriodoActivo PeriodoActivo { get; set; }
        public virtual ICollection<PeriodoNoLectivo> PeriodosNoLectivos { get; set; }
        public virtual  ICollection<CourseGenerate> CoursesGenerates { get; set; }
        public virtual ICollection<MigrationToCanvas> Migrations { get; set; }
    }
}