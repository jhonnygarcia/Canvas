using System.Configuration;

namespace DataCacheServices.AppDataCache.ConfigSection
{
    /// <summary>
    /// Modelo de Configuración del sistema de almacenamiento en Caché.
    /// </summary>
    /// <remarks>
    /// A Continuación se detallan los nombres de subelementos xml de configuración con sus correspondientes Propiedades.
    /// <table border="1">
    /// <tr><th><b>Atributos/Subelementos</b></th><th><b>Propiedades de este Modelo</b></th></tr>
    /// <tr><td>impl</td><td><see cref="Impl"/></td></tr>
    /// <tr><td>applicationPrefix</td><td><see cref="ApplicationPrefix"/></td></tr>
    /// </table>
    /// </remarks>
    public class DataCacheConfigSection : ConfigurationSection
    {
        private const string ELEMENT_EXTRA_PARAMS = "extraParams", 
                             ATTRIBUTE_IMPL = "impl",
                             ATTRIBUTE_APPLICATION_PREFIX = "applicationPrefix",
                             ATTRIBUTE_HIGH_CONCURRENT_MODE = "highConcurrentMode";

        /// <summary>
        /// Posibles valores extras requeridos por la Implementación especificada en: <see cref="Impl"/>.
        /// </summary>
        [ConfigurationProperty(ELEMENT_EXTRA_PARAMS, IsRequired = false)]
        public ExtraParamsCollectionConfig ExtraParams
        {
            get { return (ExtraParamsCollectionConfig)this[ELEMENT_EXTRA_PARAMS]; }
            set { this[ELEMENT_EXTRA_PARAMS] = value; }
        }

        /// <summary>
        /// Implementación proporcionada para el Componente de Almacenamiento en Caché (El tipo debe representarse con el Nombre Calificado).
        /// </summary>
        [ConfigurationProperty(ATTRIBUTE_IMPL, IsRequired = false)]
        public string Impl
        {
            get { return (string)this[ATTRIBUTE_IMPL]; }
            set { this[ATTRIBUTE_IMPL] = value; }
        }

        /// <summary>
        /// Si se especifica, añade este valor a todas las Keys. Util en los casos de aplicaciones replicada (Plataformas LMS)
        /// </summary>
        [ConfigurationProperty(ATTRIBUTE_APPLICATION_PREFIX, IsRequired = false)]
        public string ApplicationPrefix 
        {
            get { return (string)this[ATTRIBUTE_APPLICATION_PREFIX]; }
            set { this[ATTRIBUTE_APPLICATION_PREFIX] = value; }
        }

        /// <summary>
        /// Si se especifica, añade este valor a todas las Keys. Util en los casos de aplicaciones replicada (Plataformas LMS)
        /// </summary>
        [ConfigurationProperty(ATTRIBUTE_HIGH_CONCURRENT_MODE, IsRequired = false)]
        public bool? HighConcurrentMode
        {
            get { return (bool?)this[ATTRIBUTE_HIGH_CONCURRENT_MODE]; }
            set { this[ATTRIBUTE_HIGH_CONCURRENT_MODE] = value; }
        }
    }
}
