using System.ComponentModel.DataAnnotations;

namespace ControleDeContatos.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Digite o login para cadastro")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Digite a senha para cadastro")]
        public string Senha { get; set; }
    }
}
