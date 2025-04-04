using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A bio é obrigatória")]
        public string Bio { get; set; }

        [Required(ErrorMessage = "A image é obrigatória")]
        public string Image { get; set; }
    }
}
