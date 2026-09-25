
DTOs separate my API contract from my database entities. 
They allow me to control what data comes into and goes out of the API and prevent me from exposing my EF entities directly

I validate requests at the API boundary using DTOs and DataAnnotations. ASP.NET Core's ApiController automatically validates the model. I can also standardize the validation response so clients such as Blazor or Angular can easily display field-level errors.

DTO validation checks whether the incoming request has valid data.
Service validation handles business rules that should remain enforced regardless of where the request comes from.