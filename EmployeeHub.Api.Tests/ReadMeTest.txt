Unit tests isolate the service by mocking dependencies such as the repository and cache.

Integration tests exercise multiple real application components together. In this case, the test sends an HTTP request through authentication, authorization, the controller, service, Redis, repository, and database.