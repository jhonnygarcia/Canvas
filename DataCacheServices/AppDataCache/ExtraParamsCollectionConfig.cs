using System.Configuration;

namespace DataCacheServices.AppDataCache
{
    /// <summary>
    /// Modelo de Configuración de la Lista de Parámteros Extras para implementaciones de Componentes Transverzales.
    /// </summary>
    [ConfigurationCollection(typeof(ExtraParamsCollectionConfig))]
    public class ExtraParamsCollectionConfig : ConfigurationElementCollection
    {
        /// <inherit source="Parent" restrict="Project" />
        protected override ConfigurationElement CreateNewElement()
        {
            return new ExtraParamConfigElement();
        }
        /// <inherit source="Parent" restrict="Project" />
        protected override object GetElementKey(ConfigurationElement element)
        {
            var template = (ExtraParamConfigElement)element;
            return template.Name;
        }

        /// <summary>
        /// Obtiene el elemento  en el índice especificado.
        /// </summary>
        /// <param name="idx">El índice del elemento</param>
        /// <returns>El elemento</returns>
        public ExtraParamConfigElement this[int idx]
        {
            get
            {
                return (ExtraParamConfigElement)BaseGet(idx);
            }
        }
        /// <summary>
        /// Obtiene el elemento a partir del KeyName especificado
        /// </summary>
        /// <param name="key">El KeyName</param>
        /// <returns>El elemento</returns>
        new public ExtraParamConfigElement this[string key]
        {
            get
            {
                return (ExtraParamConfigElement)BaseGet(key);
            }
        }    
    }
}
