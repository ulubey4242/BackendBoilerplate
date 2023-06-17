using System;

namespace Core.Attributes
{
    internal class IdentifierAttribute : Attribute
    {
        public string Identifier;

        public IdentifierAttribute(string Identifier)
        {
            this.Identifier = Identifier;
        }
    }
}