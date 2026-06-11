using System.ComponentModel.DataAnnotations;

namespace LibraryService.WebAPI.DTO;

/// <summary>
/// Datos enviados desde el formulario publico de reporte de fraudes.
/// </summary>
public class FraudForm
{
    [Required(ErrorMessage = "Los detalles del impostor son obligatorios.")]
    [MaxLength(1000, ErrorMessage = "Los detalles del impostor no pueden superar 1000 caracteres.")]
    public string ImpostorDetails { get; set; } = string.Empty;

    [Required(ErrorMessage = "El numero, correo o usuario de contacto es obligatorio.")]
    [MaxLength(500, ErrorMessage = "La informacion de contacto no puede superar 500 caracteres.")]
    public string ContactInfo { get; set; } = string.Empty;

    [MaxLength(2000, ErrorMessage = "Los comentarios no pueden superar 2000 caracteres.")]
    public string? Comments { get; set; }
}
