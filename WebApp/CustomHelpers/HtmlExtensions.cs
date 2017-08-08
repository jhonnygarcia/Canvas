using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using WebApp.Globalization.Scripts;

namespace WebApp.CustomHelpers
{
    public static class HtmlExtensions
    {
        private const string SCRIPT_BLOCK_TEMPLATE = "<script type=\"text/javascript\">{0}</script>";
        private const string JAVASCRIPT_ALERT_NO_RESOURCE_FOR_TYPE_TEMPLATE = "alert('No se pudieron encontrar las cadenas de globalización correspondientes para: {0}');";
        private static readonly ConcurrentDictionary<string, string> ResourcesTypesCache = new ConcurrentDictionary<string, string>();
        public static MvcHtmlString RenderGlobalizationMessagesContent(this HtmlHelper htmlHelper, params Type[] resourcesTypes)
        {
            return new MvcHtmlString(
                string.Format(SCRIPT_BLOCK_TEMPLATE, GetGlobalizationMessagesObject(resourcesTypes.Select(t => t.FullName).ToArray())));
        }
        internal static string GetGlobalizationMessagesObject(string[] resourcesTypeName)
        {
            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var cultureName = ConfigurationManager.AppSettings["DefaultCulture"];
             var cacheKeyHash = GetMd5CacheName(string.Join(string.Empty, resourcesTypeName) + cultureName);
            if (ResourcesTypesCache.ContainsKey(cacheKeyHash))
            {
                return ResourcesTypesCache[cacheKeyHash];
            }

            var allScripts = new StringBuilder();
            foreach (var resourceType in resourcesTypeName)
            {
                var type = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a =>
                    {
                        var ts = new Type[0];
                        try
                        {
                            ts = a.GetTypes();
                        }
                        catch (System.Reflection.ReflectionTypeLoadException ryle)
                        {
                            foreach (var loaderException in ryle.LoaderExceptions)
                            {
                                throw loaderException;
                            }
                        }
                        return ts;
                    }).FirstOrDefault(t => resourceType == t.FullName);

                if (type == null)
                {
                    return string.Format(JAVASCRIPT_ALERT_NO_RESOURCE_FOR_TYPE_TEMPLATE, resourceType);
                }

                var resourceManager = new ResourceManager(type);

                var resourceSet = resourceManager.GetResourceSet(currentCulture, true, true);
                var linesList = resourceSet.Cast<DictionaryEntry>()
                                       .Select(e => string.Format(ScriptTemplates.KeyValueGlobalizationTemplate, e.Key, e.Value));
                allScripts.AppendLine(ScriptTemplates.GlobalizationScript
                    .Replace(ScriptTemplates.KeyCurrentCulture, cultureName)
                    .Replace(ScriptTemplates.KeyMessages, string.Join((char)0x2C + Environment.NewLine, linesList)));
            }

            var result = allScripts.ToString();
            ResourcesTypesCache.TryAdd(cacheKeyHash, result);
            return result;
        }
        private static string GetMd5CacheName(string cacheKeyName)
        {
            using (var md5 = MD5.Create())
            {
                var hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(cacheKeyName));
                var sb = new StringBuilder();
                foreach (var b in hashBytes)
                    sb.Append(b.ToString("X2"));

                return sb.ToString();
            }
        }
    }
}