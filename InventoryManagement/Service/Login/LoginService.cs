using InventoryManagement.DataBase;
using InventoryManagement.Model;
using InventoryManagement.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Service.Login
{
    internal class LoginService
    {
        public PadUser Login(string userName, string passWord)
        {
            PadUser padUser = SqliteHelper.Current.db.PadUsers.Where(pad => pad.CN_LOGIN == userName).FirstOrDefault() ?? throw new Exception("没有此用户");
            string decryptPassWord = AesUtils.Decrypt(padUser.CN_Download_Password);
            if (!passWord.Equals(decryptPassWord))
                throw new Exception("用户密码不正确");
            return padUser;
        }
    }
}
