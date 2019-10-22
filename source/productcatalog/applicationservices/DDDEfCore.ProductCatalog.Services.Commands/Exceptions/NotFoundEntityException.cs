using System;

namespace DDDEfCore.ProductCatalog.Services.Commands.Exceptions
{
    public class NotFoundEntityException : ApplicationServiceCommandsException
    {
        public NotFoundEntityException(string message) : base(message)
        {
        }

        public NotFoundEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
