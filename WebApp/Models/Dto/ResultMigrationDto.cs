using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using UNIR.Servicios.Integracion.Negocio.ClasesDto;
using WebApp.Common.ReturnsCanvas;

namespace WebApp.Models.Dto
{
    public sealed class ResultMigrationDto
    {
        public ResultMigrationDto()
        {
            Courses = new List<MotivoDto<CourseCanvasDto>>();
            Assignments = new List<MotivoDto<AssignmentCanvasDto>>();
            Stundents = new List<MotivoDto<PersonaDto>>();
        }
        public List<MotivoDto<CourseCanvasDto>> Courses { get; set; }
        public List<MotivoDto<AssignmentCanvasDto>> Assignments { get; set; }
        public List<MotivoDto<PersonaDto>> Stundents { get; set; }
    }

    public enum CodeMigrationIrregularidad
    {
        CodeMessage1 = 1,//El SIS-ID asociado a este curso no se encuentra en las asignaturas del estudio en su periodo
        CodeMessage2,//La asignatura del gestor asociado al curso no tiene una fecha definida en el inicio o fin de sus grupos
        CodeMessage3,//Existe un grupo en la asignatura con fecha inicio mayor a la fecha fin
        CodeMessage4,//Estudiante con algun dato personal no definido
        CodeMessage5,//Estudiante con la contraseña sin el formato correcto
        CodeMessage6,//La Fecha inicio de la tarea demo esta fuera de rango de la fecha inicio y fin del curso demo
        CodeMessage7,//La Fecha de presentación de la tarea demo esta fuera de rango de la fecha inicio y fin del curso demo
        CodeMessage8,//La Fecha fin de la tarea demo esta fuera de rango de la fecha inicio y fin del curso demo
        CodeMessage9,//La Fecha inicio de la tarea tras el calculo no se encuentra en ningun dia disponible.
        CodeMessage10,//La Fecha de presentación de la tarea tras el calculo no se encuentra en ningun dia disponible.
        CodeMessage11//La Fecha fin de la tarea tras el calculo no se encuentra en ningun dia disponible.
    }
}