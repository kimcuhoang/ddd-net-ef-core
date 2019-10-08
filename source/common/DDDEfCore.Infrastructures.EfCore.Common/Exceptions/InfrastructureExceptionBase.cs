using System;

namespace DDDEfCore.Infrastructures.EfCore.Common.Exceptions
{
    public abstract class InfrastructureExceptionBase : Exception
    {
        protected InfrastructureExceptionBase(string message) : base(message) { }
        protected InfrastructureExceptionBase(string message, Exception innerException) : base(message, innerException) { }
    }
}
