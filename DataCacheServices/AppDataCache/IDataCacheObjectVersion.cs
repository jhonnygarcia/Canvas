namespace DataCacheServices.AppDataCache
{
    /// <summary>
    /// Representa a la información de Versión de un Objeto guardado en el sistema de Almacenamiento en Cache.
    /// </summary>
    public interface IDataCacheObjectVersion
    {
        /// <summary>
        /// Devuelve al objeto único que identifica a la Versión Actual.
        /// </summary>
        /// <returns>La referencia al objeto.</returns>
        object GetComparableUniqueObject();
    }
}
