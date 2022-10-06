using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginAndRegistration.Models
{
    public class ResetPasswordModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Mật khẩu bắt buộc phải nhập.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Độ dài mật khẩu tối thiểu 6 ký tự.")]
        [MaxLength(100, ErrorMessage = "Mật khẩu quá dài.")]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu không trùng nhau.")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetCode { get; set; }
    }
}