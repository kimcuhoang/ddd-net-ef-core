

using DNK.DDD.Core.Exceptions;

namespace DDD.ProductCatalog.Core.Exceptions;

public class DomainException : ExceptionBase
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
