using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models.Model.Entity
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Level { get; set; }
        public string Loger { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
    }
}