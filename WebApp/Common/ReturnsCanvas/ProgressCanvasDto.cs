using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Architecture.Parameters;

namespace WebApp.Common.ReturnsCanvas
{
    public enum ProgressState
    {
        Queued,
        Running,
        Completed,
        Failed
    };
    public class ProgressCanvasDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int Completion { get; set; }
        public string WorkflowState { get; set; }
        public ProgressState GetState()
        {
            var state = (ProgressState)Enum.Parse(typeof(ProgressState), WorkflowState, true);
            return state;
        }
    }
}