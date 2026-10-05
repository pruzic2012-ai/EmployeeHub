using System;
using System.Collections.Generic;
using System.Text;

namespace EmployeeHub.Application.Services;

public enum UpdateEmployeeResult
{
    Updated,
    NotFound,
    ConcurrencyConflict
}
