Business rules go in the service layer. 
The controller handles HTTP concerns, the service handles business logic, and the repository handles data access.


How did you implement authentication and authorization?

I implemented JWT-based authentication. The user logs in with a username and password, and I verify the password using ASP.NET Core's PasswordHasher. If authentication succeeds, I generate a JWT containing claims such as the user ID, username, and role. The client sends the token in the Authorization Bearer header. ASP.NET Core's JWT middleware validates the token, and then authorization policies determine whether the authenticated user has permission to access a particular endpoint