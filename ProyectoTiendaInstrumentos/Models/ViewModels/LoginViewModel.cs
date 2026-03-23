using System.ComponentModel.DataAnnotations;

namespace ProyectoTiendaInstrumentos.Models.ViewModels;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "El email es obligatorio.")]
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "La contraseña debe tener al menos 5 caracteres.")]
    public string Password { get; set; } = string.Empty;
}
