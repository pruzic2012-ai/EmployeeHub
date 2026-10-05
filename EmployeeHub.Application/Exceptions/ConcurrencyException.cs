using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeHub.Application.Exceptions;

public class ConcurrencyException : Exception
{   
    public ConcurrencyException(string message)
        : base(message)
    {
    }
    
}
