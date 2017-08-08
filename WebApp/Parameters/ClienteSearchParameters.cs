using WebApp.Architecture.Parameters;

namespace WebApp.Parameters
{
    public class ClienteSearchParameters
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SimpleItem<int> Estudio { get; set; }
    }
}