using WebApp.Architecture.Parameters;

namespace WebApp.Parameters
{
    public class CourseSaveParameters
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AccountId { get; set; }
        public string Event { get; set; }
        public SimpleItem<int> Asignatura { get; set; }
    }
}