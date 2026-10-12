
using EmployeeHub.Domain.Entities;

using Microsoft.AspNetCore.Identity;

var user = new User
{
    Username = "user1"
};

var hasher = new PasswordHasher<User>();

var password = "***REMOVED***";

var hash = hasher.HashPassword(user, password);

Console.WriteLine(hash);