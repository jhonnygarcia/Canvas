using System;

namespace DataCacheServices.AppDataCache
{
    /// <summary>
    /// Gestiona el almacenamiento de Datos en el Sistema de Caché elegido.
    /// La memoria sería compartida por la aplicación durante toda su existencia, para acceder a un almacenamiento
    /// específico por sesión use: <see cref="ISecurityService.SetSessionValue"/> y <see cref="ISecurityService.GetSessionValue"/>
    /// </summary>
    public interface IDataCacheService
    {
        /// <summary>
        /// Crea un Agrupador de Objetos identificado por un nombre
        /// </summary>
        /// <param name="name">El nombre del Agrupador</param>
        /// <returns>True si la operación se completó satisfactoriamente. False si el Agrupador ya existía</returns>
        bool CreateObjectGroup(string name);
        /// <summary>
        /// Elimina un Agrupado de Objetos previamente creado. Si existen objetos asociados a él, también los elimina.
        /// </summary>
        /// <param name="name">El nombre del Agrupador</param>
        /// <returns>True si la operación se completó satisfactoriamente. False si el Agrupador no existe.</returns>
        bool RemoveObjectGroup(string name);

        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <remarks>La información de versión del Objeto recien creado/sobreescrito.</remarks>
        IDataCacheObjectVersion Put(string key, object value);
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="group">Grupo al que se pretende asociar el objeto insertado.</param>
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        IDataCacheObjectVersion Put(string key, object value, string group);
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="timeout">Tiempo de vencimiento del objeto en Caché</param>
        /// <remarks>La información de versión del Objeto recien creado/sobreescrito.</remarks>
        IDataCacheObjectVersion Put(string key, object value, TimeSpan timeout);
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="group">Grupo al que se pretende asociar el objeto insertado.</param>
        /// <param name="timeout">Tiempo de vencimiento de los objetos pertenecientes al grupo. Los objetos existentes en el grupo actualizaran su tiempo de vencimiento a este valor</param>        
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        IDataCacheObjectVersion Put(string key, object value, string group, TimeSpan timeout);
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// En caso de existir un objeto asignato a la llave, verifica su versión contra la que se pasa como argumento. No estas no son inguales, se lanza una excepción.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="version">Versión esperada del objeto a eliminar. La ejecución falla si la versión del objeto almacenado es diferente</param>
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        IDataCacheObjectVersion Put(string key, object value, IDataCacheObjectVersion version);
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché, que esté asociado a un grupo.
        /// En caso de existir un objeto asignato a la llave, verifica su versión contra la que se pasa como argumento. No estas no son inguales, se lanza una excepción.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="group">Grupo al que pertenece el elemento</param>
        /// <param name="version">Versión esperada del objeto a eliminar. La ejecución falla si la versión del objeto almacenado es diferente</param>
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        IDataCacheObjectVersion Put(string key, object value, string group, IDataCacheObjectVersion version);
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// En caso de existir un objeto asignato a la llave, verifica su versión contra la que se pasa como argumento. No estas no son inguales, se lanza una excepción.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="version">Versión esperada del objeto a eliminar. La ejecución falla si la versión del objeto almacenado es diferente</param>
        /// <param name="timeout">Tiempo de vencimiento del objeto en Caché</param>
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        IDataCacheObjectVersion Put(string key, object value, IDataCacheObjectVersion version, TimeSpan timeout);
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché, que esté asociado a un grupo.
        /// En caso de existir un objeto asignato a la llave, verifica su versión contra la que se pasa como argumento. No estas no son inguales, se lanza una excepción.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="group">Grupo al que está asociado el elemento.</param>
        /// <param name="version">Versión esperada del objeto a eliminar. La ejecución falla si la versión del objeto almacenado es diferente</param>
        /// <param name="timeout">Tiempo de vencimiento de los objetos pertenecientes al grupo. Los objetos existentes en el grupo actualizaran su tiempo de vencimiento a este valor</param>        
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        IDataCacheObjectVersion Put(string key, object value, string group, IDataCacheObjectVersion version, TimeSpan timeout);

        /// <summary>
        /// Obtiene el valor de un objeto previamente almacenado en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <typeparam name="T">Tipo de dato para el objeto devuelto.</typeparam>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <returns>El objeto resultante, o null en caso de no existir</returns>
        T Get<T>(string key);
        /// <summary>
        /// Obtiene el valor de un objeto previamente almacenado en el Sistema de Almacenamiento en Caché y asociado a un group.
        /// </summary>
        /// <typeparam name="T">Tipo de dato para el objeto devuelto.</typeparam>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="group">Grupo al cual estaría asociado el objeto.</param>
        /// <returns>El objeto, si no existe el group o el objeto no se encuentra en el devuelve null</returns>
        T Get<T>(string key, string group);
        /// <summary>
        /// Obtiene el valor de un objeto previamente almacenado en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <typeparam name="T">Tipo de dato para el objeto devuelto.</typeparam>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="version">Versión asignada al objeto en la caché.</param>
        /// <returns>El objeto resultante, o null en caso de no existir</returns>
        T Get<T>(string key, out IDataCacheObjectVersion version);

        /// <summary>
        /// Libera el espacio ocupado por un objeto en el Sistema de Almacenamiento en Caché, eliminándolo del mismo.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <returns>True si la operación se pudo completar (el objeto se eliminó)</returns>
        bool Remove(string key);
        /// <summary>
        /// Libera el espacio ocupado por un objeto en el Sistema de Almacenamiento en Caché, eliminándolo del mismo, siempre y cuando se encuentre dentro del group especificado..
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="group">Grupo al cual estaría asociado el objeto.</param>
        /// <returns>True si la operación se completó satisfactoriamente.</returns>
        bool Remove(string key, string group);
        /// <summary>
        /// Libera el espacio ocupado por un objeto en el Sistema de Almacenamiento en Caché, eliminándolo del mismo.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="version">Versión esperada del objeto a eliminar. La ejecución falla si la versión del objeto almacenado es diferente</param>
        /// <returns>True si la operación se pudo completar (el objeto se eliminó)</returns>
        bool Remove(string key, IDataCacheObjectVersion version);

        /// <summary>
        /// Limpia la totalidad del contenido del Sistema de Almacenamiento en Caché.
        /// </summary>
        void Flush();

        /// <summary>
        /// Resetea el tiempo de supervivencia de un objeto en caché al valor establecido
        /// </summary>
        /// <param name="key">Llave para localizar al objeto</param>
        /// <param name="newTimeout">Nuevo tiempo de vida del objeto</param>
        void ResetObjectTimeout(string key, TimeSpan newTimeout);
        /// <summary>
        /// Resetea el tiempo de supervivencia de los objetos pertenecientes a un grupo
        /// </summary>
        /// <param name="group">Grupo al que pertenecen los objetos</param>
        /// <param name="newTimeout">Nuevo tiempo de vida de los objetos pertenecientes al grupo</param>
        void ResetGroupTimeout(string group, TimeSpan newTimeout);
    }
}