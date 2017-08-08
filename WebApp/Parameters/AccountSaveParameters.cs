using WebApp.Architecture.Parameters;

namespace WebApp.Parameters
{
    public class AccountSaveParameters
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SimpleItem<int> Estudio { get; set; }
        public int? IdPeriodoMatriculacion { get; set; }
        public int? ParentAccountId { get; set; }
    }
}