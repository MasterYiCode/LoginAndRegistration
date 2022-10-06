using Model.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Dao
{
    public class UserDao
    {
        private LoginAndRegistrationDbContext context = null;
        
        public UserDao()
        {
            context = new LoginAndRegistrationDbContext();
        }

        public void Insert(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }

        public User GetUserByEmail(string email)
        {
            return context.Users.FirstOrDefault(x => x.Email == email);
        }

        public User GetUserResetPasswordCode(string resetPasswordCode)
        {
            return context.Users.FirstOrDefault(x => x.ResetPasswordCode == resetPasswordCode);
        }

        // Tìm email, trả về false nếu kh có, true nếu có
        public bool FindEmailFirstOrDefault(string email)
        {
            var u = context.Users.FirstOrDefault(x => x.Email == email);
            return u != null;
        }

        // Xác thực tài khoản
        public bool VerifyAccount(string id)
        {
            // This line I have added here to avoid
            // Confirm password dose not match issue on save changes
            context.Configuration.ValidateOnSaveEnabled = false;

            var user = context.Users.FirstOrDefault(u => u.ActivationCode == new Guid(id));
            
            if(user != null)
            {
                user.IsEmailVerified = true;
                context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public bool UpdateResetPasswordCode(string email , string resetCode)
        {
            // This line I have added here to avoid confirm password not match issue, as we had added a confirm password property
            // in cur model class in part 1
            context.Configuration.ValidateOnSaveEnabled = false;
            var user = context.Users.FirstOrDefault(u => u.Email == email);
            if(user != null)
            {
                user.ResetPasswordCode = resetCode;
                context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool UpdatePassword(string resetPasswordCode, string newPasswordMD5Hash)
        {
            context.Configuration.ValidateOnSaveEnabled = false;
            var user = context.Users.FirstOrDefault(u => u.ResetPasswordCode == resetPasswordCode);
            if(user != null)
            {
                user.Password = newPasswordMD5Hash;
                user.ResetPasswordCode = "";
                context.SaveChanges();
                return true;
            }
            return false;

        }
    }
}
