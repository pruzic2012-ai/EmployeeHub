
using EmployeeHub.Domain.Entities;

using Microsoft.AspNetCore.Identity;

var user = new User
{
    Username = "user1"
};

var hasher = new PasswordHasher<User>();

Console.Write("Enter password to hash: ");
var password = Console.ReadLine() ?? string.Empty;

var hash = hasher.HashPassword(user, password);

Console.WriteLine(hash);