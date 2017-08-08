using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApp.App_Start
{
    public static class GlobalValues
    {
        public static string UrlCanvas = ConfigurationManager.AppSettings["UrlCanvas"];
        public static string TokenCanvas = ConfigurationManager.AppSettings["TokenCanvas"];
        public const string ERROR = "ERROR";
        public const string WARNING = "WARNING";
        public const string INFO = "INFO";

        public const int DEFAULT_MINUTE_CACHE = 60;
        public const string CACHE_ESTUDIO = "get-estudios";
        public const string CACHE_ASIGNATURAS_ESTUDIO = "get-asignaturas-estudio-{0}";
        public const string GROUP_CACHE_GESTOR = "group-ws-gestor";

        public const string CACHE_GET_ACCOUNT = "get-account-{0}";
        public const string GROUP_CACHE_ACCOUT = "group-account";


        public const string CACHE_PROGRESS_INFO = "progress-info-{0}";
    }
}