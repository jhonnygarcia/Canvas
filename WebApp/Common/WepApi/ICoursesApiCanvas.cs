using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Parameters;

namespace WebApp.Common.WepApi
{
    public interface ICoursesApiCanvas
    {
        ResultValue<CourseCanvasDto> Update(CourseSaveParameters parameters);
        ResultValue<CourseCanvasDto> GetBySisId(string sisId);
        ResultValue<CourseCanvasDto> Get(int id);
        ResultValue<MigrationCanvasDto> CreateMigrationContent(int courseFromId, int courseToId);
        ResultValue<CourseCanvasDto> Delete(int id);
        ResultList<AssignmentCanvasDto> GetAssignments(int courseId);
        ResultValue<AssignmentCanvasDto> UpdateAssignment(int courseId, AssignmentCanvasDto parameters);
        ResultValue<SectionCanvasDto> CreateSection(int courseId, SectionCanvasDto section);

        ResultList<SectionCanvasDto> GetSecctionsCourse(int courseId);
        ResultValue<SectionCanvasDto> SetSisIdSecction(int sectionId, string sisId);
        ResultValue<SectionCanvasDto> DeleteSecction(int sectionId);
        ResultValue<MigrationCanvasDto> GetMigrationContent(int courseId, int migrationId);
    }
}