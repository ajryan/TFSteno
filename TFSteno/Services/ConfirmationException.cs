using System;

namespace TFSteno.Services
{
    public class ConfirmationException : Exception
    {
        public ConfirmationException(string email)
            : base(email + " not confirmed.")
        {
        }
    }
}