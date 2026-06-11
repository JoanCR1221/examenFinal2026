using System.ComponentModel.DataAnnotations;

namespace LibraryService.WebAPI.Entities
{
    /// <summary>
    /// Reporte de fraude registrado desde el sitio publico de LABCIBE-UNA.
    /// </summary>
    public class Fraud
    {
        [Key]
        public int Id { get; set; }

        /// <summary>Detalles del impostor (nombre usado, perfil, etc.).</summary>
        [Required]
        [MaxLength(1000)]
        public string ImpostorDetails { get; set; } = string.Empty;

        /// <summary>Numero, correo o usuario desde el que contacto el impostor.</summary>
        [Required]
        [MaxLength(500)]
        public string ContactInfo { get; set; } = string.Empty;

        /// <summary>Comentarios adicionales del caso.</summary>
        [MaxLength(2000)]
        public string? Comments { get; set; }

        /// <summary>Fecha y hora (UTC) en que se registro el reporte.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
