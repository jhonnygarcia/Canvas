using System;
using System.Collections.Generic;
using WebApp.Architecture.Parameters;

namespace WebApp.Parameters
{
    public class PeriodoNoLectivosSaveParameters
    {
        public int AccountId { get; set; }
        public List<RangoFecha> Fechas { get; set; }
    }

    public class RangoFecha
    {
        public int? Id { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
    }
    public class RangoFechaTarea
    {
        public int Id { get; set; }
        public DateTime? Inicio { get; set; }
        public DateTime? Presentacion { get; set; }
        public DateTime? Fin { get; set; }
    }
}