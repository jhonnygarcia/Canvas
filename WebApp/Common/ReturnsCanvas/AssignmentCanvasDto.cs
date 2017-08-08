using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Architecture.Parameters;

namespace WebApp.Common.ReturnsCanvas
{
    public class AssignmentCanvasDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? PresentationDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Position { get; set; }
        public int GroupId { get; set; }
        public string HtmlUrl { get; set; }
        public bool Overflow { get; set; }
        public object ExtraData { get; set; }
    }
}