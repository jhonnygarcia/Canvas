

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Dto
{
    public class CourseExtendDto
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public int IdAsignatura { get; set; }
        public virtual AsignaturaDto Asignatura { get; set; }
        public AccountExtendDto Account { get; set; }
    }
}