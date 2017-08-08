using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Model.Entity
{
    public class Asignatura
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public virtual ICollection<AccountExtend> AccountExtends { get; set; }
    }
}