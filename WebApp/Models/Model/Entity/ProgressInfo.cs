using System;
using WebApp.Models.Dto;

namespace WebApp.Models.Model.Entity
{
    public enum ProgressInfoState
    {
        Queued,
        Running,
        Completed,
        Failed,
        HasErrors
    }
    public class ProgressInfo
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Completion { get; set; }
        public string Message { get; set; }
        public string State { get; set; }
        public string Exception { get; set; }

        public ProgressInfoState GetState()
        {
            var state = (ProgressInfoState)Enum.Parse(typeof(ProgressInfoState), State, true);
            return state;
        }
    }
}