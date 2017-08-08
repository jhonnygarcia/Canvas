using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Architecture.Parameters;

namespace WebApp.Common.ReturnsCanvas
{
    public class EnrollmentCanvasDto
    {
        public string CourseId { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
        public string SectionId { get; set; }
        public string Status { get; set; }
    }
}