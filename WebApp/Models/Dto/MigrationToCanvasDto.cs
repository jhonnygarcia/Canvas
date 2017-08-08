using System;

namespace WebApp.Models.Dto
{
    public class MigrationToCanvasDto
    {
        public int Id { get; set; }
        public int GenerateId { get; set; }
        public virtual AccountGenerateDto Generate { get; set; }
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
        public string Data { get; set; }
    }
}