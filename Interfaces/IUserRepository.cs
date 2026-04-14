using System.Collections.Generic;
using CheckMatePOS.Models;

namespace CheckMatePOS.Interfaces
{
    public interface IUserRepository
    {
        User? GetUserByUsername(string username);
        void CreateUser(User user);
        List<User> GetAllUsers();
        void SetUserActive(int userId, bool isActive);
        bool UsernameExists(string username);
    }
}