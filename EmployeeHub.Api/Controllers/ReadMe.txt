How does dependency injection connect everything?

I register the interfaces and implementations with the DI container. 
The controller receives IEmployeeService, the service receives IEmployeeRepository, 
and the repository receives AppDbContext