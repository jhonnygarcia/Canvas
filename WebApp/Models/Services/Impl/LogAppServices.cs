using LtiLibrary.Core.Outcomes.v2;
using WebApp.Models.Dto;
using WebApp.Models.Model;
using WebApp.Models.Model.Entity;

namespace WebApp.Models.Services.Impl
{
    public class LogAppServices : ILogAppServices
    {
        private readonly CanvasExtendContenxt _context;
        public LogAppServices(CanvasExtendContenxt context)
        {
            _context = context;
        }

        public int SaveLog(LogDto log)
        {
            if (log == null) return -1;

            var entity = new Log
            {
                Message = log.Message,
                Exception = log.Exception,
                Date = log.Date,
                Level = log.Level,
                Loger = log.Loger
            };
            _context.Log.Add(entity);
            _context.SaveChanges();
            return entity.Id;
        }
    }
}