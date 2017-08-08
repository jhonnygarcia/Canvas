using System.Configuration;

namespace DataCacheServices.AppDataCache
{
    /// <summary>
    /// Representa a un Parámetro y su valor para llenar el listado de PArámetros Extra de implementaciones de Componentes transverzales
    /// </summary>
    /// <remarks>
    /// A Continuación se detallan los nombres de subelementos y/o atributos xml de configuración con sus correspondientes Propiedades.
    /// <table border="1">
    /// <tr><th><b>Atributos</b></th><th><b>Propiedades de este Modelo</b></th></tr>
    /// <tr><td>name</td><td><see cref="Name"/></td></tr>
    /// <tr><td>value</td><td><see cref="Value"/></td></tr>
    /// </table>
    /// </remarks>
    public class ExtraParamConfigElement : ConfigurationElement
    {
        private const string ATTRIBUTE_NAME = "name";
        private const string ATTRIBUTE_VALUE = "value";

        /// <summary>
        /// Nombre del Parámetro extra.
        /// </summary>
        [ConfigurationProperty(ATTRIBUTE_NAME, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this[ATTRIBUTE_NAME]; }
            set { this[ATTRIBUTE_NAME] = value; }
        }
        /// <summary>
        /// Valor en forma de cadena del Parámetro extra.
        /// </summary>
        [ConfigurationProperty(ATTRIBUTE_VALUE, IsRequired = true, IsKey = false)]
        public string Value
        {
            get { return (string)this[ATTRIBUTE_VALUE]; }
            set { this[ATTRIBUTE_VALUE] = value; }
        }
    }
}
