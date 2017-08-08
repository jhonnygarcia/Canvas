

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Model.Entity
{
    public class CourseExtend
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public int AccountId { get; set; }
        public virtual AccountExtend Account { get; set; }
        public int IdAsignatura { get; set; }
        public virtual Asignatura Asignatura { get; set; }
    }
}