//**************************************************
// Identifier.cs
//
// Orion Games LLC 2019-2020
//
// Author: Jur Kampman
// Creation Date: March 27, 2020
//**************************************************

using System;

[AttributeUsage(AttributeTargets.Field)]
public class Identifier : Attribute
{
    public readonly string enumIdentifier;

    public Identifier(string enumIdentifier)
    {
        this.enumIdentifier = enumIdentifier;
    }
}
