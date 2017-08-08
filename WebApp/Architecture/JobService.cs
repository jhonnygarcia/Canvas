using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac;
using DataCacheServices.AppDataCache;
using log4net;
using WebApp.App_Start;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Globalization.Services;
using WebApp.Models.Dto;
using WebApp.Models.Model;
using WebApp.Models.Model.Entity;

namespace WebApp.Architecture
{
    public class JobService : IJobService
    {
        private readonly CanvasExtendContenxt _contenxt;
        private readonly IDataCacheService _cache;
        public JobService(CanvasExtendContenxt contenxt, IDataCacheService cache)
        {
            _contenxt = contenxt;
            _cache = cache;
        }

        public void EnqueueProcess(Expression<Action> action, int? progressId = null)
        {
            var call = action.Body as MethodCallExpression;
            if (call == null) throw new Exception("La expresión no corresponde a una llamada a un método");
            if (call.Object == null) throw new Exception("La expresión no corresponde a un método de instancia");

            var methodInfo = call.Method;
            var argsValues = call.Arguments.Select(a =>
            {
                var argAsObj = Expression.Convert(a, typeof(object));
                return Expression.Lambda<Func<object>>(argAsObj, null)
                    .Compile()();
            });
            var targetOnly = Expression.Lambda(call.Object, null);
            var compiled = targetOnly.Compile();
            var result = compiled.DynamicInvoke(null);
            var type = result.GetType();

            Task.Run(() =>
            {
                using (var container = AutofacConfig.Container.BeginLifetimeScope())
                {
                    var service = container.Resolve(type);
                    try
                    {
                        var invokeData = methodInfo.Invoke(service, argsValues.ToArray());
                        var state = ProgressInfoState.Completed;
                        if (invokeData != null && progressId.HasValue)
                        {
                            var baseResult = invokeData as BaseResult;
                            if (baseResult != null)
                            {
                                state = baseResult.HasErrors ? ProgressInfoState.HasErrors : state;
                            }
                            _cache.Put(string.Format(GlobalValues.CACHE_PROGRESS_INFO, progressId), invokeData, TimeSpan.FromMinutes(30));
                        }
                        var progress = _contenxt.ProgressInfo.Find(progressId);
                        if (progress != null)
                        {
                            progress.State = state.ToString();
                            progress.Completion = 100;
                            _contenxt.SaveChanges();
                        }
                    }
                    catch (Exception exception)
                    {
                        var log = LogManager.GetLogger(typeof(JobService));
                        log.Error("Migracion", exception);

                        var logEntity = new Log
                        {
                            Date = DateTime.Now,
                            Loger = typeof(JobService).FullName,
                            Message = exception.Message,
                            Exception = exception.ToString(),
                            Level = GlobalValues.ERROR
                        };
                        _contenxt.Log.Add(logEntity);
                        _contenxt.SaveChanges();

                        if (progressId.HasValue)
                        {
                            var progress = _contenxt.ProgressInfo.Find(progressId);
                            if (progress != null)
                            {
                                progress.State = ProgressInfoState.Failed.ToString();
                                progress.Message = string.Format(CanvasApiStrings.ErrorJobServices, logEntity.Id);
                                progress.Exception = exception.ToString();
                                progress.Completion = 100;
                                _contenxt.SaveChanges();
                            }
                        }
                    }
                }
            });
        }

        public int CreateProgress()
        {
            var progress = new ProgressInfo
            {
                Date = DateTime.Now,
                Completion = 0,
                Exception = "",
                Message = "",
                State = ProgressInfoState.Queued.ToString()
            };
            _contenxt.ProgressInfo.Add(progress);
            _contenxt.SaveChanges();
            return progress.Id;
        }
    }
}
