using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ProyectoTiendaInstrumentos.Models.ViewModels;

public sealed class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "La contraseña debe tener al menos 5 caracteres.")]
    public string Password { get; set; } = string.Empty;

    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
    public string? Telefono { get; set; }

    [StringLength(200)]
    public string? Direccion { get; set; }

    public IFormFile? Imagen { get; set; }
}
