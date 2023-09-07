using Dapper;
using DNK.DDD.Core.Models;
using System.Data;
using System.Reflection;

namespace DNK.DDD.Infrastructure.Dapper.MsSqlServer;

public class StronglyTypedIdMapper<TIdentity> : SqlMapper.TypeHandler<TIdentity> where TIdentity : notnull, IdentityBase
{
    #region Overrides of TypeHandler<TIdenityType>

    public override void SetValue(IDbDataParameter parameter, TIdentity value)
    {
        parameter.Value = value.Id;
    }

    public override TIdentity Parse(object value)
    {
        return (TIdentity?)Activator.CreateInstance(type: typeof(TIdentity),
            bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null,
            args: new object[] { (Guid)value },
            culture: null) ?? throw new InvalidCastException();
    }

    #endregion
}
