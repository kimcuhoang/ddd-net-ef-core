using System;
using System.Collections.Generic;
using System.Text;
using DDDEfCore.Core.Common.Models;
using DDDEfCore.Core.DomainModels.Exceptions;

namespace DDDEfCore.Core.DomainModels.Catalogs
{
    public class Catalog : AggregateRoot
    {
        public CatalogId CatalogId => (CatalogId)this.Id;

        public string DisplayName { get; private set; }

        #region Constructors

        private Catalog(CatalogId catalogId, string catalogName) : base(catalogId)
        {
            if (string.IsNullOrWhiteSpace(catalogName))
            {
                throw new DomainException($"{nameof(catalogName)} is empty.");
            }

            this.DisplayName = catalogName;
        }

        private Catalog() { }

        #endregion

        #region Creations

        public static Catalog Create(string catalogName) => new Catalog(CatalogId.New(), catalogName);

        #endregion

        #region Behaviors

        public Catalog ChangeDisplayName(string catalogName)
        {
            if (string.IsNullOrWhiteSpace(catalogName))
            {
                throw new DomainException($"{nameof(catalogName)} is empty.");
            }

            this.DisplayName = catalogName;
            return this;
        }

        #endregion
    }
}
