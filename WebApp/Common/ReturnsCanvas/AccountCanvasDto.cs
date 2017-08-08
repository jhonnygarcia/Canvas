using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.App_Start;
using WebApp.Architecture.Parameters;

namespace WebApp.Common.ReturnsCanvas
{
    public enum AccountState
    {
        Active,
        Deleted
    }
    public class AccountCanvasDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SisId { get; set; }
        public int ParentId { get; set; }
        public List<CourseCanvasDto> Courses { get; set; }
        public SimpleItem<int> Estudio { get; set; }
        public string WorkflowState { get; set; }
        public bool Generated { get; set; }
        public bool IsAccountPeriodo { get; set; }
        public string HtmlUrl => GlobalValues.UrlCanvas + "accounts/" + Id;
        public AccountState GetState()
        {
            var state = (AccountState)Enum.Parse(typeof(AccountState), WorkflowState, true);
            return state;
        }
    }
}