namespace DNK.DDD.Infrastructure;

public class InfrastructureException : Exception
{
    private InfrastructureException(string message) : base(message) { }

    public static InfrastructureException New(string message) => new(message);
}
