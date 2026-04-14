using System;
using System.Collections.Generic;
using CheckMatePOS.DAL;
using CheckMatePOS.Interfaces;
using CheckMatePOS.Models;

namespace CheckMatePOS.BLL
{
    public class AuthService
    {
        private readonly IUserRepository userRepo;
        public User? CurrentUser { get; private set; }

        public AuthService(IUserRepository userRepository)
        {
            userRepo = userRepository;
        }

        public bool Login(string username, string password)
        {
            var user = userRepo.GetUserByUsername(username);

            if (user == null)
            {
                Console.WriteLine("  [Error] User not found.");
                return false;
            }

            if (!user.IsActive)
            {
                Console.WriteLine("  [Error] Account is deactivated.");
                return false;
            }

            string hashedInput = DatabaseHelper.HashPassword(password);

            if (user.PasswordHash != hashedInput)
            {
                Console.WriteLine("  [Error] Wrong password.");
                return false;
            }

            CurrentUser = user;
            Console.WriteLine($"  Welcome, {user.Username}! Role: {user.Role}");
            return true;
        }

        public void Logout()
        {
            Console.WriteLine($"  Goodbye, {CurrentUser?.Username}!");
            CurrentUser = null;
        }

        public bool IsAdmin()
        {
            return CurrentUser != null && CurrentUser.Role == "Admin";
        }

        public bool IsCashier()
        {
            return CurrentUser != null && CurrentUser.Role == "Cashier";
        }

        public void CreateCashier(string username, string password)
        {
            if (userRepo.UsernameExists(username))
            {
                Console.WriteLine("  [Error] Username already exists!");
                return;
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("  [Error] Username and password cannot be empty!");
                return;
            }

            if (password.Length < 4)
            {
                Console.WriteLine("  [Error] Password must be at least 4 characters!");
                return;
            }

            var user = new User
            {
                Username = username,
                PasswordHash = DatabaseHelper.HashPassword(password),
                Role = "Cashier",
                IsActive = true
            };

            userRepo.CreateUser(user);
            Console.WriteLine($"  Cashier '{username}' created successfully!");
        }

        public void ToggleUserStatus(int userId, bool activate)
        {
            userRepo.SetUserActive(userId, activate);
            string status = activate ? "activated" : "deactivated";
            Console.WriteLine($"  User ID {userId} {status}.");
        }

        public List<User> GetAllUsers()
        {
            return userRepo.GetAllUsers();
        }
    }
}