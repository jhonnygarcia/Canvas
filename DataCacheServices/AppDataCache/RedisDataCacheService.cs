using System;
using System.Configuration;
using System.Data;
using System.Linq;
using DataCacheServices.AppDataCache.ConfigSection;
using StackExchange.Redis;

namespace DataCacheServices.AppDataCache
{
    /// <summary>
    /// Implementación que utiliza Redis para el amacenamiento de Datos en Memoria Caché.
    /// </summary>
    public class RedisDataCacheService : IDataCacheService
    {
        private static readonly DataCacheConfigSection Configuration = ConfigurationManager.GetSection("dataCacheManager") as DataCacheConfigSection;

        /// <summary>
        /// Formato para la serialización de objetos
        /// </summary>
        internal SerializationFormat SerializationFormat { get; set; }

        /// <summary>
        /// Inicializa una nueva instancia de este tipo.
        /// </summary>
        public RedisDataCacheService()
        {
            SerializationFormat = SerializationFormat.Binary;
        }

        /// <summary>
        /// Prefijo para el almacenamiento en Caché. Se toma de la configuración de la aplicación, pero puede ser sobreescrito por un heredero.
        /// </summary>
        protected virtual string ApplicationPrefix
        {
            get
            {
                return Configuration != null && !string.IsNullOrEmpty(Configuration.ApplicationPrefix)
                    ? Configuration.ApplicationPrefix : string.Empty;
            }
        }
        /// <summary>
        /// Cadena de conexión del Servidor
        /// </summary>
        protected virtual string ConnectionString
        {
            get
            {
                //Chequear los Parámetros Extra para esta implementación.
                if (!Configuration.ExtraParams.ElementInformation.IsPresent)
                {
                    throw new ConfigurationErrorsException("Falta elemento: 'extraParams' dentro de 'dataCacheManager'");
                }
                return Configuration.ExtraParams["host"].Value + ":" + Configuration.ExtraParams["port"].Value + ", allowAdmin=true";
            }
        }

        private ConnectionMultiplexer CreateConnection()
        {
            return ConnectionMultiplexer.Connect(ConnectionString);
        }

        /// <summary>
        /// Crea un Agrupador de Objetos identificado por un nombre
        /// </summary>
        /// <param name="name">El nombre del Agrupador</param>
        /// <returns>True si la operación se completó satisfactoriamente. False si el Agrupador ya existía</returns>
        public bool CreateObjectGroup(string name)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                return db.SetLength(ApplicationPrefix + name) == 0;
            }
        }
        /// <summary>
        /// Elimina un Agrupado de Objetos previamente creado. Si existen objetos asociados a él, también los elimina.
        /// </summary>
        /// <param name="name">El nombre del Agrupador</param>
        /// <returns>True si la operación se completó satisfactoriamente. False si el Agrupador no existe.</returns>
        public bool RemoveObjectGroup(string name)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                var group = db.SetMembers(ApplicationPrefix + name); ;
                var res = group.Any();
                foreach (var key in group)
                {
                    db.SetRemove(ApplicationPrefix + name, key);
                    db.KeyDelete(ApplicationPrefix + name + ":" + key);
                }
                return res;
            }
        }
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        public IDataCacheObjectVersion Put(string key, object value)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                db.StringSet(ApplicationPrefix + key, value.Serialize(SerializationFormat));
                return new RedisDataCacheObjectVersion();
            }
        }
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="group">Grupo al que se pretende asociar el objeto insertado.</param>
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>        
        public IDataCacheObjectVersion Put(string key, object value, string @group)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                db.SetAdd(ApplicationPrefix + group, key);
                db.StringSet(ApplicationPrefix + group + ":" + key, value.Serialize(SerializationFormat));
                return new RedisDataCacheObjectVersion();
            }
        }
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="timeout">Tiempo de vencimiento del objeto en Caché</param>
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        public IDataCacheObjectVersion Put(string key, object value, TimeSpan timeout)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                db.StringSet(ApplicationPrefix + key, value.Serialize(SerializationFormat), timeout);
                return new RedisDataCacheObjectVersion();
            }
        }
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="group">Grupo al que se pretende asociar el objeto insertado.</param>
        /// <param name="timeout">Tiempo de vencimiento de los objetos pertenecientes al grupo. Los objetos existentes en el grupo actualizaran su tiempo de vencimiento a este valor</param>        
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>        
        public IDataCacheObjectVersion Put(string key, object value, string @group, TimeSpan timeout)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                db.StringSet(ApplicationPrefix + group + ":" + key, value.Serialize(SerializationFormat));
                db.SetAdd(group, key);
                var list = db.SetMembers(ApplicationPrefix + group);
                foreach (var redisKey in list)
                {
                    db.KeyExpire(ApplicationPrefix + group + ":" + redisKey, timeout);
                }
                return new RedisDataCacheObjectVersion();
            }
        }
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// En caso de existir un objeto asignato a la llave, verifica su versión contra la que se pasa como argumento. Si estas no son inguales, se lanza una excepción.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="version">Versión esperada del objeto a eliminar. La ejecución falla si la versión del objeto almacenado es diferente</param>
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        public IDataCacheObjectVersion Put(string key, object value, IDataCacheObjectVersion version)
        {
            if (version == null) return Put(key, value);
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                var transaction = db.CreateTransaction();
                transaction.StringSetAsync(ApplicationPrefix + key, value.Serialize(SerializationFormat));
                if (!transaction.Execute()) throw new IncorrectObjectVersionException("Versión incorrecta del Objeto.");
                return new RedisDataCacheObjectVersion();
            }
        }
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché, que esté asociado a un grupo.
        /// En caso de existir un objeto asignato a la llave, verifica su versión contra la que se pasa como argumento. Si estas no son inguales, se lanza una excepción.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="group">Grupo al que pertenece el elemento</param>
        /// <param name="version">Versión esperada del objeto a eliminar. La ejecución falla si la versión del objeto almacenado es diferente</param>
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        public IDataCacheObjectVersion Put(string key, object value, string @group, IDataCacheObjectVersion version)
        {
            if (version == null) return Put(key, value, group);
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                var transaction = db.CreateTransaction();
                transaction.SetAddAsync(ApplicationPrefix + group, key);
                transaction.StringSetAsync(ApplicationPrefix + group + ":" + key, value.Serialize(SerializationFormat));
                if (!transaction.Execute()) throw new IncorrectObjectVersionException("Versión incorrecta del Objeto.");
                return new RedisDataCacheObjectVersion();
            }
        }
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché.
        /// En caso de existir un objeto asignato a la llave, verifica su versión contra la que se pasa como argumento. Si estas no son inguales, se lanza una excepción.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="version">Versión esperada del objeto a eliminar. La ejecución falla si la versión del objeto almacenado es diferente</param>
        /// <param name="timeout">Tiempo de vencimiento del objeto en Caché</param>
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        public IDataCacheObjectVersion Put(string key, object value, IDataCacheObjectVersion version, TimeSpan timeout)
        {
            if (version == null) return Put(key, value);
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                var transaction = db.CreateTransaction();
                transaction.StringSetAsync(ApplicationPrefix + key, value.Serialize(SerializationFormat), timeout);
                if (!transaction.Execute()) throw new IncorrectObjectVersionException("Versión incorrecta del Objeto.");
                return new RedisDataCacheObjectVersion();
            }
        }
        /// <summary>
        /// Añade o sobreescribe un valor en el Sistema de Almacenamiento en Caché, que esté asociado a un grupo.
        /// En caso de existir un objeto asignato a la llave, verifica su versión contra la que se pasa como argumento. Si estas no son inguales, se lanza una excepción.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="value">Valor a insertar/sobreescribir</param>
        /// <param name="group">Grupo al que está asociado el elemento.</param>
        /// <param name="version">Versión esperada del objeto a eliminar. La ejecución falla si la versión del objeto almacenado es diferente</param>
        /// <param name="timeout">Tiempo de vencimiento de los objetos pertenecientes al grupo. Los objetos existentes en el grupo actualizaran su tiempo de vencimiento a este valor</param>        
        /// <returns>La información de versión del Objeto recien creado/sobreescrito.</returns>
        public IDataCacheObjectVersion Put(string key, object value, string @group, IDataCacheObjectVersion version, TimeSpan timeout)
        {
            if (version == null) return Put(key, value, group, timeout);
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                var transaction = db.CreateTransaction();
                transaction.SetAddAsync(ApplicationPrefix + group, key);
                transaction.StringSetAsync(ApplicationPrefix + group + ":" + key, value.Serialize(SerializationFormat));
                if (!transaction.Execute()) throw new IncorrectObjectVersionException("Versión incorrecta del Objeto.");
                var list = db.SetMembers(ApplicationPrefix + group);
                foreach (var redisKey in list)
                {
                    db.KeyExpire(ApplicationPrefix + group + ":" + redisKey, timeout);
                }
                return new RedisDataCacheObjectVersion();
            }
        }
        /// <summary>
        /// Obtiene el valor de un objeto previamente almacenado en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <typeparam name="T">Tipo de dato para el objeto devuelto.</typeparam>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <returns>El objeto resultante, o null en caso de no existir</returns>
        public T Get<T>(string key)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                var strObj = (string)db.StringGet(ApplicationPrefix + key);
                return string.IsNullOrEmpty(strObj) ? default(T) : strObj.Deserialize<T>(SerializationFormat);
            }
        }
        /// <summary>
        /// Obtiene el valor de un objeto previamente almacenado en el Sistema de Almacenamiento en Caché y asociado a un group.
        /// </summary>
        /// <typeparam name="T">Tipo de dato para el objeto devuelto.</typeparam>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="group">Grupo al cual estaría asociado el objeto.</param>
        /// <returns>El objeto, si no existe el group o el objeto no se encuentra en el devuelve null</returns>
        public T Get<T>(string key, string @group)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                var strObj = (string)db.StringGet(ApplicationPrefix + group + ":" + key);
                return string.IsNullOrEmpty(strObj) ? default(T) : strObj.Deserialize<T>(SerializationFormat);
            }
        }
        /// <summary>
        /// Obtiene el valor de un objeto previamente almacenado en el Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <typeparam name="T">Tipo de dato para el objeto devuelto.</typeparam>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="version">Versión asignada al objeto en la caché.</param>
        /// <returns>El objeto resultante, o null en caso de no existir</returns>
        public T Get<T>(string key, out IDataCacheObjectVersion version)
        {
            using (var cache = CreateConnection())
            {
                version = new RedisDataCacheObjectVersion();
                var db = cache.GetDatabase();
                var strObj = (string)db.StringGet(ApplicationPrefix + key);
                return string.IsNullOrEmpty(strObj) ? default(T) : strObj.Deserialize<T>(SerializationFormat);
            }
        }
        /// <summary>
        /// Libera el espacio ocupado por un objeto en el Sistema de Almacenamiento en Caché, eliminándolo del mismo.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <returns>True si la operación se pudo completar (el objeto se eliminó)</returns>
        public bool Remove(string key)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                return db.KeyDelete(ApplicationPrefix + key);
            }
        }
        /// <summary>
        /// Libera el espacio ocupado por un objeto en el Sistema de Almacenamiento en Caché, eliminándolo del mismo, siempre y cuando se encuentre dentro del group especificado..
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="group">Grupo al cual estaría asociado el objeto.</param>
        /// <returns>True si la operación se completó satisfactoriamente.</returns>        
        public bool Remove(string key, string @group)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                return db.KeyDelete(ApplicationPrefix + group + ":" + key);
            }
        }
        /// <summary>
        /// Libera el espacio ocupado por un objeto en el Sistema de Almacenamiento en Caché, eliminándolo del mismo.
        /// </summary>
        /// <param name="key">Llave, en forma de cadena, del elemento.</param>
        /// <param name="version">Versión esperada del objeto a eliminar. La ejecución falla si la versión del objeto almacenado es diferente</param>
        /// <returns>True si la operación se pudo completar (el objeto se eliminó)</returns>
        public bool Remove(string key, IDataCacheObjectVersion version)
        {
            if (version == null) return Remove(key);
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                var transaction = db.CreateTransaction();
                transaction.KeyDeleteAsync(ApplicationPrefix + key);
                if (!transaction.Execute()) throw new IncorrectObjectVersionException("Versión incorrecta del Objeto.");
                return true;
            }
        }
        /// <summary>
        /// Limpia la totalidad del contenido del Sistema de Almacenamiento en Caché.
        /// </summary>
        /// <remarks>
        /// Este método no elimina a los Grupos de Caché, solo lo hace con su contenido
        /// </remarks>
        public void Flush()
        {
            using (var cache = CreateConnection())
            {
                cache.GetServer(cache.GetEndPoints().FirstOrDefault()).FlushDatabase();
            }
        }
        /// <summary>
        /// Resetea el tiempo de supervivencia de un objeto en caché al valor establecido
        /// </summary>
        /// <param name="key">Llave para localizar al objeto</param>
        /// <param name="newTimeout">Nuevo tiempo de vida del objeto</param>
        public void ResetObjectTimeout(string key, TimeSpan newTimeout)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                db.KeyExpire(ApplicationPrefix + key, newTimeout);
            }
        }
        /// <summary>
        /// Resetea el tiempo de supervivencia de los objetos pertenecientes a un grupo
        /// </summary>
        /// <param name="group">Grupo al que pertenecen los objetos</param>
        /// <param name="newTimeout">Nuevo tiempo de vida de los objetos pertenecientes al grupo</param>
        public void ResetGroupTimeout(string @group, TimeSpan newTimeout)
        {
            using (var cache = CreateConnection())
            {
                var db = cache.GetDatabase();
                var list = db.SetMembers(ApplicationPrefix + group);
                foreach (var redisKey in list)
                {
                    db.KeyExpire(ApplicationPrefix + group + ":" + redisKey, newTimeout);
                }
            }
        }
    }
}
