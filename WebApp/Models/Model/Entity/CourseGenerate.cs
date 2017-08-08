using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Model.Entity
{
    public class CourseGenerate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AccountId { get; set; }
        public string SisId { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public virtual AccountGenerate Account { get; set; }
    }
}