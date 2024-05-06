using System.ComponentModel.DataAnnotations;

namespace PustokMVC.ViewModels
{
    public class UserLoginViewModel
    {
        [Required]
        public string UsernameOrEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
