using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Model.Entity
{
    public class MigrationToCanvas
    {
        public int Id { get; set; }
        public int GenerateId { get; set; }
        public virtual AccountGenerate Generate { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public string Data { get; set; }
    }
}