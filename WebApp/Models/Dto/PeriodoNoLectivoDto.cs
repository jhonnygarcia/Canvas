using System;
using WebApp.Models.Model.Entity;

namespace WebApp.Models.Dto
{
    public class PeriodoNoLectivoDto
    {
        public int Id { get; set; }        
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public int AccountGenerateId { get; set; }
        public virtual AccountGenerate AccountGenerate { get; set; }
    }
}