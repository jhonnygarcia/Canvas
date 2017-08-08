using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Models.Dto;
using WebApp.Parameters;

namespace WebApp.Models.Services
{
    public interface ICanvasAppServices
    {
        ResultList<AccountCanvasDto> GetSubAccountsCourses(int accountId);
        ResultValue<AccountCanvasDto> GetAccount(int accountId);
        ResultList<CourseCanvasDto> GetCourses(int accountId);
        ResultValue<AccountCanvasDto> UpdateAccount(AccountSaveParameters parameter);
        ResultValue<CourseCanvasDto> UpdateCourse(CourseSaveParameters parameter);
        ResultValue<AccountCanvasDto> GenerarPeriodo(AccountSaveParameters parameter);
        ResultValue<ResultMigrationDto> MigrarCursos(int accountId, int progressId);
        ResultValue<ResultMigrationDto> MigrarCurso(int generateId, int courseDemoId, int progressId);
        ResultValue<ProgressInfoDto> GetProgress(int progressId);
        ResultValue<ResultMigrationDto> GetResult(int progressId);
        ResultList<MigrationToCanvasDto> GetMigrations(int accountId);
        ResultValue<ResultMigrationDto> GetMigration(int migrationId);
        ResultValue<bool> HasCourseGenerate(int generateId, int courseId);
        ResultValue<CourseCanvasDto> RemoveCourse(int id);
        ResultValue<AccountCanvasDto> RemoveAccount(int id);
    }
}