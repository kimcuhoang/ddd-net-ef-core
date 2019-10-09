using System;

namespace DDDEfCore.Infrastructures.EfCore.Common.Exceptions
{
    public class DatabaseCannotConnectException : InfrastructureExceptionBase
    {
        public DatabaseCannotConnectException() : base("Database cannot connect.")
        {
        }

        public DatabaseCannotConnectException(string message) : base(message)
        {
        }

        public DatabaseCannotConnectException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
