using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginAndRegistration.Models
{
    public class UserLogin
    {
        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email bắt buộc phải nhập.")]
        public string Email { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password bắt buộc phải nhập.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Nhớ tài khoản")]
        public bool RememberMe { get; set; }

    }
}