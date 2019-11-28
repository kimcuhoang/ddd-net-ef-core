using Dapper;
using DDDEfCore.Core.Common.Models;
using System.Data;

namespace DDDEfCore.ProductCatalog.Services.Queries
{
    public class StronglyTypedIdMapper<TIdenity> : SqlMapper.TypeHandler<TIdenity> where TIdenity : IdentityBase
    {
        #region Overrides of TypeHandler<TIdenityType>

        public override void SetValue(IDbDataParameter parameter, TIdenity value)
        {
            parameter.Value = value.Id;
        }

        public override TIdenity Parse(object value)
        {
            return IdentityFactory.Create<TIdenity>(value);
        }

        #endregion
    }
}
