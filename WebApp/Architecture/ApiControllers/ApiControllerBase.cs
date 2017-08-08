using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApp.Architecture.TransferStructs;

namespace WebApp.Architecture.ApiControllers
{
    public class ApiControllerBase : System.Web.Http.ApiController
    {
        protected IHttpActionResult ResultWithMessages<T>(BaseResult result, T content)
        {
            var negotiator = Configuration.Services.GetContentNegotiator();
            var negoResult = negotiator.Negotiate(typeof(HttpCommonResultContent<>), Request, Configuration.Formatters);

            if (negoResult == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                throw new HttpResponseException(response);
            }

            return new HttpWithMessageResult<T>(negoResult.Formatter, negoResult.MediaType, result, content);
        }
        /// <summary>
        /// Prepara y devuelve un resultado con Mensajes
        /// </summary>
        /// <param name="result">Resultado de Servicio de Aplicación</param>
        /// <returns>El resultado</returns>
        protected IHttpActionResult ResultWithMessages(BaseResult result)
        {
            return ResultWithMessages<object>(result, null);
        }

        /// <summary>
        /// Prepara y devuelve un resultado con Mensajes
        /// </summary>
        /// <param name="errors">Listado de errores para el resultado</param>
        /// <param name="code">Código del resultado</param>
        /// <returns>El resultado</returns>
        protected IHttpActionResult ResultWithMessages(IEnumerable<string> errors, HttpStatusCode code)
        {
            var negotiator = Configuration.Services.GetContentNegotiator();
            var negoResult = negotiator.Negotiate(typeof(HttpCommonResultContent<>), Request, Configuration.Formatters);

            if (negoResult == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
                throw new HttpResponseException(response);
            }

            return new HttpWithMessageResult<object>(negoResult.Formatter, negoResult.MediaType, code, errors, new string[0], new string[0], null);
        }
        /// <summary>
        /// Resultado de Listado Paginado
        /// </summary>
        /// <param name="result">Result de capa de Aplicación</param>
        /// <typeparam name="T">Tipo del objeto DTO del listado</typeparam>
        /// <returns>Los resultados</returns>
        protected IHttpActionResult OkPagedList<T>(ResultList<T> result)
            where T : class
        {
            var response = Request.CreateResponse(HttpStatusCode.OK, result.Elements);

            response.Headers.Add("X-TotalElements", result.TotalElements.ToString());
            response.Headers.Add("X-TotalPages", result.TotalPages.ToString());

            return ResponseMessage(response);
        }
        /// <summary>
        /// Resultado de listado paginado. Esta sobrecarga permite especificar directamente los valores de la paginación
        /// </summary>
        /// <typeparam name="T">Tipo de los elementos</typeparam>
        /// <param name="elements">La colección de los elementos</param>
        /// <param name="totalElements">Cantidad todoal de elementos</param>
        /// <param name="totalPages">Cantidad de páginas</param>
        /// <returns></returns>
        protected IHttpActionResult OkPagedList<T>(IEnumerable<T> elements, int? totalElements, int? totalPages)
            where T : class
        {
            var response = Request.CreateResponse(HttpStatusCode.OK, elements);

            if (totalElements.HasValue)
                response.Headers.Add("X-TotalElements", totalElements.Value.ToString());
            if (totalPages.HasValue)
                response.Headers.Add("X-TotalPages", totalPages.Value.ToString());

            return ResponseMessage(response);
        }
        protected IHttpActionResult OkList<T>(IEnumerable<T> elements)
            where T : class
        {
            var response = Request.CreateResponse(HttpStatusCode.OK, elements);
            return ResponseMessage(response);
        }
    }
}
