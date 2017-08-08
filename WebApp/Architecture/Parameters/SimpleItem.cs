namespace WebApp.Architecture.Parameters
{
    public class SimpleItem<T> where T : struct
    {
        public T Id { get; set; }
        public string Value { get; set; }
    }
}