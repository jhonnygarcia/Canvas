using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UNIR.Servicios.Integracion.Negocio.ClasesDto;
using WebApp.App_Start;
using WebApp.Architecture.Parameters;

namespace WebApp.Common.ReturnsCanvas
{
    public enum CourseState
    {
        Unpublished,
        Available,
        Completed,
        Deleted
    }
    public class CourseCanvasDto
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public int TotalStudents { get; set; }
        public string SisId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public CourseState GetState()
        {
            var state = (CourseState)Enum.Parse(typeof(CourseState), WorkflowState, true);
            return state;
        }
        public string WorkflowState { get; set; }
        public SimpleItem<int> Asignatura { get; set; }
        public string HtmlUrl => GlobalValues.UrlCanvas + "courses/" + Id;
    }
}