using System;
using DDDEfCore.Core.Common.Exceptions;

namespace DDDEfCore.ProductCatalog.Core.DomainModels.Exceptions
{
    public class DomainException : ExceptionBase
    {
        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
