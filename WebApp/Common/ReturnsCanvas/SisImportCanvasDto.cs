using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Architecture.Parameters;

namespace WebApp.Common.ReturnsCanvas
{
    public enum SisImportState
    {
        Created,
        Importing,
        Cleanup_batch,
        Imported,
        Imported_with_messages,
        Failed_with_messages,
        Failed
    };
    public class SisImportCanvasDto
    {
        public int Id { get; set; }
        public int Progress { get; set; }
        public string WorkflowState { get; set; }
        public SisImportState GetState()
        {
            var state = (SisImportState)Enum.Parse(typeof(SisImportState), WorkflowState, true);
            return state;
        }
    }
}