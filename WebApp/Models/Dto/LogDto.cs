using System;

namespace WebApp.Models.Dto
{
    public class LogDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Level { get; set; }
        public string Loger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public int StatusCode { get; set; }
    }
}