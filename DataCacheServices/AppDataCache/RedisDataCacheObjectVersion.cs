namespace DataCacheServices.AppDataCache
{
    /// <summary>
    ///  Implementación para Redis de <see cref="IDataCacheObjectVersion"/>
    /// </summary>
    public class RedisDataCacheObjectVersion : IDataCacheObjectVersion
    {
        /// <summary>
        /// Implementación del método correspondiente: <see cref="IDataCacheObjectVersion.GetComparableUniqueObject"/>
        /// </summary>
        /// <returns>El objeto.</returns>
        public object GetComparableUniqueObject()
        {
            return null;
        }
    }
}
