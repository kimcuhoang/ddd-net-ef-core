using System;
using Dapper;
using DDDEfCore.Core.Common.Models;
using System.Data;
using System.Reflection;

namespace DDDEfCore.ProductCatalog.Services.Queries
{
    public class StronglyTypedIdMapper<TIdentity> : SqlMapper.TypeHandler<TIdentity> where TIdentity : IdentityBase
    {
        #region Overrides of TypeHandler<TIdenityType>

        public override void SetValue(IDbDataParameter parameter, TIdentity value)
        {
            parameter.Value = value.Id;
        }

        public override TIdentity Parse(object value)
        {
            return (TIdentity)Activator.CreateInstance(type: typeof(TIdentity),
                bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
                binder: null,
                args: new object[] { (Guid)value },
                culture: null);

            throw new InvalidCastException();
        }

        #endregion
    }
}
