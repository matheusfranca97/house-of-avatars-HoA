using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class ValidationRules
{
    //Contains validation rule methods

    public static ValidationResult IsEmailValid(string email)
    {
        //Check for exactly one @
        //Check recipient name (max 64 chars, special char cannot be
        //first or last or appear consecutively two+ times)
        //Check domain name (max 253 chars)
        //Check top level domain (exists)

        string[] splitEmail = email.Split('@'); //0 - Recipient name, 1 - Domain name + top level domain
        if (splitEmail.Length != 2) return ValidationResult.FromFail("Email must contain exactly one @ symbol");
        else if (splitEmail[0].Length == 0) return ValidationResult.FromFail("Recipient name cannot be empty");

        char lastChar = ' ';
        if (splitEmail[0].Length > 64) return ValidationResult.FromFail("Recipient name can only contain 64 characters");
        else if (splitEmail[0].Any(c => { bool fail = (char.IsSymbol(c) && c == lastChar); lastChar = c; return fail; }))
            return ValidationResult.FromFail("Recipient name cannot feature two or more consecutive special characters");

        List<string> splitDomain = splitEmail[1].Split('.').ToList();
        if (splitDomain.Count < 2) return ValidationResult.FromFail("Domain must contain a top-level domain (.com, .co.uk, .net, etc)");
        else if (splitDomain[0] == string.Empty)
            return ValidationResult.FromFail("@ Symbol must be followed by domain name");
        splitDomain.RemoveAt(splitDomain.Count - 1);
        if (string.Join(".", splitDomain).Length > 253) return ValidationResult.FromFail("Domain can only contain 253 characters");

        return ValidationResult.FromSuccess();
    }

    public static ValidationResult IsPasswordValid(string password)
    {
        //Password has to have 8-32 chars
        //Contain atleast one upper-case and one lower-case character
        //Contain atleast one number
        //Contain atleast one symbol
        if (password.Length < 8) return ValidationResult.FromFail("Password must have at least 8 characters");
        else if (password.Length > 32) return ValidationResult.FromFail("Password can only have up to 32 characters");
        else if (!password.Any(x => char.IsLower(x)) || !password.Any(x => char.IsUpper(x)))
            return ValidationResult.FromFail("Password must contain at least one upper-case character and at least one lower-case character");
        else if (!password.Any(x => char.IsNumber(x))) return ValidationResult.FromFail("Password must contain at least one number");
        else if (!password.Any(x => char.IsSymbol(x) || char.IsPunctuation(x))) return ValidationResult.FromFail("Password must contain at least one special character (!, /, _, -, ?, £, $, etc)");

        return ValidationResult.FromSuccess();
    }

    public static ValidationResult IsUsernameValid(string username)
    {
        //TO-DO: Create username validation, including support for blocking bad names and their variations
        if (username.Length < 4) return ValidationResult.FromFail("Username must be at least 4 characters");
        return ValidationResult.FromSuccess();
    }
}
