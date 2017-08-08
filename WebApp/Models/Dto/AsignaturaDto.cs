using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Dto
{
    public class AsignaturaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int? CourseId { get; set; }
        public virtual IEnumerable<AccountExtendDto> AccountExtends { get; set; }
    }
}