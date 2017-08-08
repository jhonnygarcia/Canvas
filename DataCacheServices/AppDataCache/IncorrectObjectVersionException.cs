using System;

namespace DataCacheServices.AppDataCache
{
    /// <summary>
    /// Se lanza cuando la versión de un objeto que se desea insertar en el Sistema de Datos en Caché no coincide con el que actualmente esté en el almacén
    /// </summary>
    public class IncorrectObjectVersionException : Exception
    {
        /// <summary>
        /// Crea e inicializa una instancia de esta clase.
        /// </summary>
        /// <param name="message">Mensage de la excepción</param>
        public IncorrectObjectVersionException(string message)
            : base(message)
        {
        }
        /// <summary>
        /// Crea e inicializa una instancia de esta clase.
        /// </summary>
        /// <param name="message">Mensage de la excepción</param>
        /// <param name="innerException">Excepción interna (opcional)</param>
        public IncorrectObjectVersionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
