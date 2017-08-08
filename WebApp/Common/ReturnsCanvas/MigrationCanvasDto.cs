using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Architecture.Parameters;

namespace WebApp.Common.ReturnsCanvas
{
    public enum MigrationState
    {
        Pre_processing,
        Pre_processed,
        Running,
        Waiting_for_select,
        Completed,
        Failed
    };
    public class MigrationCanvasDto
    {
        public int Id { get; set; }
        public string ProgressUrl { get; set; }
        public string MigrationType { get; set; }
        public string WorkflowState { get; set; }
        public MigrationState GetState()
        {
            var state = (MigrationState)Enum.Parse(typeof(MigrationState), WorkflowState, true);
            return state;
        }
    }
}