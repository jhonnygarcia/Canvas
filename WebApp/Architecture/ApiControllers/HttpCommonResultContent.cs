using System.Collections.Generic;

namespace WebApp.Architecture.ApiControllers
{
    /// <summary>
    /// Estructura de un resultado con Mensajes y Contenido a una llamada WebApi
    /// </summary>
    public class HttpCommonResultContent<T>
    {
        /// <summary>
        /// Contenido del Resultado. Puede ser null
        /// </summary>
        public T Content { get; set; }

        /// <summary>
        /// Listado de Mensajes de Error
        /// </summary>
        public IEnumerable<string> Errors { get; set; }
        /// <summary>
        /// Listado de Mensajes de Advertencia
        /// </summary>
        public IEnumerable<string> Warnings { get; set; }
        /// <summary>
        /// Listado de Mensajes Informativos
        /// </summary>
        public IEnumerable<string> Messages { get; set; }
    }
}
