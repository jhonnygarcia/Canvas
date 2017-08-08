using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Model.Entity
{
    public class AccountExtend
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public int IdEstudio { get; set; }
        public virtual Estudio Estudio { get; set; }
        public virtual ICollection<Asignatura> Asignaturas { get; set; }
        public virtual ICollection<PeriodoActivo> PeriodoActivos { get; set; }
        public virtual ICollection<CourseExtend> Courses { get; set; }
    }
}