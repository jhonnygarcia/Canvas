using System.Collections.Generic;
using System.Linq;

namespace WebApp.Architecture.TransferStructs
{
    public class BaseResult
    {
        private ResultType? _resultType;
        public List<string> Errors { get; set; }
        public bool HasErrors => Errors.Any();
        public List<string> Warnings { get; set; }
        public bool HasWarnings => Warnings.Any();
        public List<string> Messages { get; set; }
        public bool HasMessages => Messages.Any();
        public BaseResult()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
            Messages = new List<string>();
        }
        /// <summary>
        /// Tipo del resultado
        /// </summary>
        public ResultType Type
        {
            get
            {
                if (_resultType.HasValue)
                {
                    return _resultType.Value;
                }

                return Errors.Any()
                    ? ResultType.ElementConflict
                    : ResultType.Ok;
            }
            set { _resultType = value; }
        }
    }
    public enum ResultType
    {
        /// <summary>
        /// La operación se complet´´o satisfactoriamente.
        /// Pueden especificarse más datos en: <see cref="BaseResult.Messages"/> ó <see cref="BaseResult.Warnings"/>
        /// </summary>
        Ok,
        /// <summary>
        /// La operación fue completada exitosamente, dando como resultado la creación de un nuevo elemento.
        /// Se deben enviar datos el elemento creado, más posibles mensajes informativos en:
        /// <see cref="BaseResult.Messages"/> ó <see cref="BaseResult.Warnings"/>
        /// </summary>
        ElementCreated,
        /// <summary>
        /// El contenido de la Solicitud de Datos en uncorrecto, ya sea por falta de los mismos o errores de validación de Dominio.
        /// Deben especificarse detalles del (o los) error(es) en la collección: <see cref="BaseResult.Errors"/>.
        /// Es posible también devolver información en: <see cref="BaseResult.Warnings"/> ó <see cref="BaseResult.Messages"/>
        /// </summary>
        ValidationError,
        /// <summary>
        /// Luego de evaluar los datos de la solicitud, se determina que la Cuenta de Usuario registrada actualmente no tiene privilegios para completar la operación.
        /// Es posible también devolver en: <see cref="BaseResult.Warnings"/> ó <see cref="BaseResult.Messages"/>
        /// </summary>
        Unauthorized,
        /// <summary>
        /// El elemento solicitado no se ha podido encontrar.
        /// Se deben devolver detalles del elemento en la colección:  <see cref="BaseResult.Errors"/>
        /// </summary>
        ElementNotFound,
        /// <summary>
        /// Se intenta insertar un elemento que ya existe.
        /// Se deben devolver detalles del elemento en la colección:  <see cref="BaseResult.Errors"/>
        /// </summary>
        ElementConflict,
        /// <summary>
        /// Error no recuperable. Se esperan más datos en la colección: <see cref="BaseResult.Errors"/>
        /// Se deben devolver detalles del elemento en la colección:  <see cref="BaseResult.Errors"/>
        /// </summary>
        InternalError
    }
}