using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDDEfCore.ProductCatalog.Services.Queries.SqlConverter
{
    public sealed class SqlQueryBuilder
    {
        public enum  JoinType
        {
            INNER,
            LEFT,
            RIGHT
        }

        private string _fromObject;

        private List<string> _selectedFields = new List<string>();

        private List<JoinInformation> _joinObjects  = new List<JoinInformation>();

        private SqlQueryBuilder(List<string> fields, string fromObject)
        {
            if (string.IsNullOrWhiteSpace(fromObject)) throw new ArgumentNullException(nameof(fromObject));

            if (fields == null || !fields.Any()) throw new ArgumentNullException(nameof(fields));

            this._fromObject = fromObject;
            this._selectedFields = fields;
        }

        public static SqlQueryBuilder Select(List<string> fields, string fromObject)
            => new SqlQueryBuilder(fields, fromObject);

        public SqlQueryBuilder AddField(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                this._selectedFields.Add(fieldName);
            }
            return this;
        }

        public SqlQueryBuilder RemoveField(string fieldName)
        {
            if (!string.IsNullOrWhiteSpace(fieldName))
            {
                this._selectedFields = this._selectedFields
                    .Where(x => !x.Equals(fieldName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return this;
        }

        public SqlQueryBuilder Join(JoinType joinType, string joinWithObject, string joinCondition, List<string> selectedFields)
        {
            var joinInfo = new JoinInformation(joinType, joinWithObject, joinCondition, selectedFields);
            this._joinObjects.Add(joinInfo);
            return this;
        }

        public string Build()
        {
            var sqlClause = new StringBuilder();

            sqlClause.Append("SELECT ");
            sqlClause.Append(this._selectedFields.Select(x => $"{this._fromObject}.{x} "));
            sqlClause.Append($"FROM {this._fromObject} ");

            return sqlClause.ToString();
        }

        private class JoinInformation
        {
            public JoinType JoinType { get; }
            public string JoinWithObject { get; }
            public string JoinCondition { get; }
            public List<string> SelectedFields { get; }

            public JoinInformation(JoinType joinType, string joinWithObject, string joinCondition, List<string> selectedFields)
            {
                this.JoinType = joinType;
                this.JoinWithObject = joinWithObject;
                this.JoinCondition = joinCondition;
                this.SelectedFields = selectedFields;
            }

            public string JoinClause
                => $"{this.JoinType.ToString()} {this.JoinWithObject} AS {this.JoinWithObject} ON";
        }
    }
}
