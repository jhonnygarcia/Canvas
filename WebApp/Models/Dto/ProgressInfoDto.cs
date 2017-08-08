using System;

namespace WebApp.Models.Dto
{
    public class ProgressInfoDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Completion { get; set; }
        public string Message { get; set; }
        public string State { get; set; }
        public string Exception { get; set; }
        public bool Finished => Completion >= 100;
    }
}