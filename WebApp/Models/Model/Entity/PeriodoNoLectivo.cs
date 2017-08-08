using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Model.Entity
{
    public class PeriodoNoLectivo
    {
        public int Id { get; set; }        
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public int AccountGenerateId { get; set; }
        public virtual AccountGenerate AccountGenerate { get; set; }
    }
}