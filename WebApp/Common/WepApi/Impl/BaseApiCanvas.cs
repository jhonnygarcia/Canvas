using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebApp.App_Start;
using WebApp.Architecture.TransferStructs;
using WebApp.Models.Dto;
using WebApp.Models.Model;
using WebApp.Models.Model.Entity;

namespace WebApp.Common.WepApi.Impl
{
    public class BaseApiCanvas
    {
        public dynamic Get(string api, HttpStatusCode correct = HttpStatusCode.OK)
        {
            using (var cliente = new HttpClient())
            {
                cliente.DefaultRequestHeaders.Accept.Clear();
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalValues.TokenCanvas);
                cliente.DefaultRequestHeaders.Add("Connection", "keep-alive");
                cliente.BaseAddress = new Uri(GlobalValues.UrlCanvas);
                var request = cliente.GetAsync(api).Result;
                if (request.StatusCode == correct)
                {
                    var response = request.Content.ReadAsAsync<dynamic>().Result;
                    return response;
                }
                else
                {
                    var statusCode = Convert.ToInt32(request.StatusCode);
                    var status = request.StatusCode.ToString();
                    var exception = request.Content.ReadAsStringAsync().Result;
                    var logEntity = new LogDto
                    {
                        Date = DateTime.Now,
                        Loger = api,
                        Message = "GET " + status + ":" + statusCode,
                        Exception = exception,
                        Level = GlobalValues.WARNING,
                        StatusCode = statusCode
                    };
                    return logEntity;
                }
            }
        }
        public dynamic GetAndHeaders(string api, HttpStatusCode correct = HttpStatusCode.OK)
        {
            using (var cliente = new HttpClient())
            {
                cliente.DefaultRequestHeaders.Accept.Clear();
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalValues.TokenCanvas);
                cliente.DefaultRequestHeaders.Add("Connection", "keep-alive");
                cliente.BaseAddress = new Uri(GlobalValues.UrlCanvas);
                var request = cliente.GetAsync(api).Result;
                if (request.StatusCode == correct)
                {
                    var response = request.Content.ReadAsAsync<dynamic>().Result;
                    return new
                    {
                        Content = response,
                        request.Headers
                    };
                }
                else
                {
                    var statusCode = Convert.ToInt32(request.StatusCode);
                    var status = request.StatusCode.ToString();
                    var exception = request.Content.ReadAsStringAsync().Result;
                    var logEntity = new LogDto
                    {
                        Date = DateTime.Now,
                        Loger = api,
                        Message = "GET " + status + ":" + statusCode,
                        Exception = exception,
                        Level = GlobalValues.WARNING,
                        StatusCode = statusCode
                    };
                    return logEntity;
                }
            }
        }
        public dynamic Post(string api, object param, HttpStatusCode correct = HttpStatusCode.OK)
        {
            using (var cliente = new HttpClient())
            {
                cliente.DefaultRequestHeaders.Accept.Clear();
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalValues.TokenCanvas);
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                cliente.BaseAddress = new Uri(GlobalValues.UrlCanvas);
                var request = cliente.PostAsJsonAsync(api, param).Result;
                if (request.StatusCode == correct)
                {
                    var response = request.Content.ReadAsAsync<dynamic>().Result;
                    return response;
                }
                else
                {
                    var statusCode = Convert.ToInt32(request.StatusCode);
                    var status = request.StatusCode.ToString();
                    var exception = request.Content.ReadAsStringAsync().Result;
                    var logEntity = new LogDto
                    {
                        Date = DateTime.Now,
                        Loger = api,
                        Message = "POST " + status + ":" + statusCode,
                        Exception = exception,
                        Level = GlobalValues.WARNING,
                        StatusCode = statusCode
                    };
                    return logEntity;
                }
            }
        }
        public dynamic Put(string api, object param, HttpStatusCode correct = HttpStatusCode.OK)
        {
            using (var cliente = new HttpClient())
            {
                cliente.DefaultRequestHeaders.Accept.Clear();
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalValues.TokenCanvas);
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                cliente.BaseAddress = new Uri(GlobalValues.UrlCanvas);
                var request = cliente.PutAsJsonAsync(api, param).Result;
                if (request.StatusCode == correct)
                {
                    var response = request.Content.ReadAsAsync<dynamic>().Result;
                    return response;
                }
                else
                {
                    var statusCode = Convert.ToInt32(request.StatusCode);
                    var status = request.StatusCode.ToString();
                    var exception = request.Content.ReadAsStringAsync().Result;
                    var logEntity = new LogDto
                    {
                        Date = DateTime.Now,
                        Loger = api,
                        Message = "GET " + status + ":" + statusCode,
                        Exception = exception,
                        Level = GlobalValues.WARNING,
                        StatusCode = statusCode
                    };
                    return logEntity;
                }
            }
        }
        public dynamic Put(string api, object param, params HttpStatusCode[] respuestas)
        {
            using (var cliente = new HttpClient())
            {
                cliente.DefaultRequestHeaders.Accept.Clear();
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalValues.TokenCanvas);
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                cliente.BaseAddress = new Uri(GlobalValues.UrlCanvas);
                var request = cliente.PutAsJsonAsync(api, param).Result;
                if (respuestas.Any(r => r == request.StatusCode))
                {
                    var response = request.Content.ReadAsAsync<dynamic>().Result;
                    return response;
                }
                else
                {
                    var statusCode = Convert.ToInt32(request.StatusCode);
                    var status = request.StatusCode.ToString();
                    var exception = request.Content.ReadAsStringAsync().Result;
                    var logEntity = new LogDto
                    {
                        Date = DateTime.Now,
                        Loger = api,
                        Message = "GET " + status + ":" + statusCode,
                        Exception = exception,
                        Level = GlobalValues.WARNING,
                        StatusCode = statusCode
                    };
                    return logEntity;
                }
            }
        }
        public dynamic Put(string api, FormCollection body , HttpStatusCode correct = HttpStatusCode.OK)
        {
            using (var cliente = new HttpClient())
            {
                cliente.DefaultRequestHeaders.Accept.Clear();
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalValues.TokenCanvas);
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                cliente.BaseAddress = new Uri(GlobalValues.UrlCanvas);
                var content = new FormUrlEncodedContent(body.AllKeys.ToDictionary(x => x, p => body[p]));
                var request = cliente.PutAsync(api, content).Result;
                if (request.StatusCode == correct)
                {
                    var response = request.Content.ReadAsAsync<dynamic>().Result;
                    return response;
                }
                else
                {
                    var statusCode = Convert.ToInt32(request.StatusCode);
                    var status = request.StatusCode.ToString();
                    var exception = request.Content.ReadAsStringAsync().Result;
                    var logEntity = new LogDto
                    {
                        Date = DateTime.Now,
                        Loger = api,
                        Message = "GET " + status + ":" + statusCode,
                        Exception = exception,
                        Level = GlobalValues.WARNING,
                        StatusCode = statusCode
                    };
                    return logEntity;
                }
            }
        }
        public dynamic Delete(string api, HttpStatusCode correct = HttpStatusCode.OK)
        {
            using (var cliente = new HttpClient())
            {
                cliente.DefaultRequestHeaders.Accept.Clear();
                cliente.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalValues.TokenCanvas);
                cliente.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                cliente.BaseAddress = new Uri(GlobalValues.UrlCanvas);

                var request = cliente.DeleteAsync(api).Result;
                if (request.StatusCode == correct)
                {
                    var response = request.Content.ReadAsAsync<dynamic>().Result;
                    return response;
                }
                else
                {
                    var statusCode = Convert.ToInt32(request.StatusCode);
                    var status = request.StatusCode.ToString();
                    var exception = request.Content.ReadAsStringAsync().Result;
                    var logEntity = new LogDto
                    {
                        Date = DateTime.Now,
                        Loger = api,
                        Message = "DELETE " + status + ":" + statusCode,
                        Exception = exception,
                        Level = GlobalValues.WARNING,
                        StatusCode = statusCode
                    };
                    return logEntity;
                }
            }
        }
    }
}