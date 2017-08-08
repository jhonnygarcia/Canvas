using System.Collections.Generic;
using UNIR.Servicios.Integracion.Negocio.ClasesDto;
using WebApp.Architecture.TransferStructs;
using WebApp.Common.ReturnsCanvas;
using WebApp.Parameters;

namespace WebApp.Common.WepApi
{
    public interface IAccountsApiCanvas
    {
        /// <summary>
        ///Obtiene un listado de las sub cuentas de una cuenta
        /// </summary>
        /// <param name="accountId">id de Cuenta</param>
        /// <returns>
        ///{
        ///  "id": 2,
        ///  "name": "Canvas Account",
        ///  "parent_account_id": 1,
        ///  "sis_account_id": "123xyz",
        ///}
        /// </returns>
        ResultList<AccountCanvasDto> GetSubAccount(int accountId);

        ResultList<CourseCanvasDto> GetCourses(int accountId);
        ResultValue<AccountCanvasDto> GetAccount(int accountId);
        ResultValue<AccountCanvasDto> Update(AccountSaveParameters parameters);
        ResultValue<AccountCanvasDto> GetBySisId(string sisId);
        ResultValue<AccountCanvasDto> CreateAccount(AccountSaveParameters parameters);
        ResultValue<CourseCanvasDto> CreateCourse(CourseCanvasDto course, int idPeriodoMatriculacion, int idAsignatura);
        ResultList<CourseCanvasDto> GetCoursesAll(int accountId);
        ResultValue<ProgressCanvasDto> DeleteCoursesAccount(int accountId, int[] coursesIds);
        ResultValue<SisImportCanvasDto> ImportUsers(int accountId, List<PersonaDto> stundents);
        ResultValue<SisImportCanvasDto> GetImported(int accountId, int importedId);
        ResultValue<SisImportCanvasDto> ImportEnrollments(int accountId, List<EnrollmentCanvasDto> enrollments);
    }
}