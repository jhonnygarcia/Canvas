using WebApp.Architecture.Parameters;

namespace WebApp.Parameters
{
    public class MigrateCoursesParameters
    {
        public int AccountId { get; set; }
        public int IdEstudio { get; set; }
        public int IdPeriodoMatriculacion { get; set; }
        public int AccountParentId { get; set; }
    }
}