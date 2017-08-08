namespace WebApp.Architecture.TransferStructs
{
    public class ResultValue<T>: BaseResult
    {
        public T Value { get; set; }
    }
}