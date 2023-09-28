using System.ComponentModel.DataAnnotations;

namespace flights.application.DTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "O usuário é obrigatório.", AllowEmptyStrings = false)]
        public string Username { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.", AllowEmptyStrings = false)]
        [MinLength(6,ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Password { get; set; }

    }
}
