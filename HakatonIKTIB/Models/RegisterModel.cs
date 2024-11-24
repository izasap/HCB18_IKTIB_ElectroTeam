using System.ComponentModel.DataAnnotations;

namespace HakatonIKTIB.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Введите логин")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Введите пароль")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Подтвердите пароль")]
        public string ConfrimPassword { get; set; }
        [Required(ErrorMessage = "Введите имя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Введите Фамилию")]
        public string LastName { get; set; }
        public string SurName { get; set; }
        public int Course {  get; set; }
        public string Specialization { get; set; }
        public string University { get; set; }
    }
}
