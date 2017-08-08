using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using WebApp.Architecture.TransferStructs;

namespace WebApp.Architecture.ApiControllers
{
    internal class HttpWithMessageResult<T> : IHttpActionResult
    {
        private readonly BaseResult _resultDto;
        private readonly T _content;
        private readonly HttpStatusCode _code;

        private readonly MediaTypeFormatter _formatter;
        private readonly MediaTypeHeaderValue _mediaType;

        internal HttpWithMessageResult(MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, BaseResult resultDto)
        {
            _code = GetHttpCode(resultDto.Type);
            _resultDto = resultDto;

            _formatter = formatter;
            _mediaType = mediaType;
        }
        internal HttpWithMessageResult(MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, BaseResult resultDto, T content)
        {
            _code = GetHttpCode(resultDto.Type);
            _resultDto = resultDto;
            _content = content;

            _formatter = formatter;
            _mediaType = mediaType;
        }

        internal HttpWithMessageResult(MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType, HttpStatusCode code, IEnumerable<string> errors, IEnumerable<string> warnings, IEnumerable<string> messages, T content)
        {
            _code = code;
            _resultDto = new BaseResult
            {
                Errors = errors.ToList(),
                Warnings = warnings.ToList(),
                Messages = messages.ToList()
            };
            _content = content;

            _formatter = formatter;
            _mediaType = mediaType;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_code)
            {
                Content = new ObjectContent<HttpCommonResultContent<T>>(
                    new HttpCommonResultContent<T>
                    {
                        Errors = _resultDto.Errors.Any() ? _resultDto.Errors : null,
                        Warnings = _resultDto.Warnings.Any() ? _resultDto.Warnings : null,
                        Messages = _resultDto.Messages.Any() ? _resultDto.Messages : null,
                        Content = _content
                    }, _formatter, _mediaType)
            };


            return Task.FromResult(response);
        }

        private static HttpStatusCode GetHttpCode(ResultType resultType)
        {
            var result = HttpStatusCode.OK;

            switch (resultType)
            {
                case ResultType.Ok:
                    result = HttpStatusCode.OK;
                    break;
                case ResultType.ElementCreated:
                    result = HttpStatusCode.Created;
                    break;
                case ResultType.ValidationError:
                    result = HttpStatusCode.BadRequest;
                    break;
                case ResultType.Unauthorized:
                    result = HttpStatusCode.Unauthorized;
                    break;
                case ResultType.ElementNotFound:
                    result = HttpStatusCode.NotFound;
                    break;
                case ResultType.ElementConflict:
                    result = HttpStatusCode.Conflict;
                    break;
                case ResultType.InternalError:
                    result = HttpStatusCode.InternalServerError;
                    break;
            }

            return result;
        }
    }
}
