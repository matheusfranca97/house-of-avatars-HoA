using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct ValidationResult
{
    public bool isSuccess;
    public string failMessage;

    public static ValidationResult FromSuccess() => new ValidationResult { isSuccess = true, failMessage = "" };
    public static ValidationResult FromFail(string message) => new ValidationResult { isSuccess = false, failMessage = message };

}
