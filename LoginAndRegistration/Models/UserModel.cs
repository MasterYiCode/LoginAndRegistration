using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LoginAndRegistration.Models
{
    public class UserModel
    {
        [Display(Name = "Tên người dùng")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Tên người dùng bắt buộc phải nhập")]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "Email")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email bắt buộc phải nhập")]
        [DataType(DataType.EmailAddress)]
        [StringLength(254)]
        public string Email { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password bắt buộc phải nhập")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password phải có tối thiểu 6 ký tự.")]
        public string Password { get; set; }

        [Display(Name = "Xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu và mật khẩu không khớp.")]
        public string ConfirmPassword { get; set; }

    }
}