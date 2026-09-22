
using EmployeeHub.Api.Models;
using Microsoft.AspNetCore.Identity;

var user = new User
{
    Username = "user1"
};

var hasher = new PasswordHasher<User>();

var password = "User123!";

var hash = hasher.HashPassword(user, password);

Console.WriteLine(hash);