using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using AutoFixture;
using AutoFixture.AutoMoq;
using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using Moq;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests
{
    public abstract class UnitTestBase<TAggregateRoot> where TAggregateRoot : AggregateRoot
    {
        protected readonly IFixture Fixture;
        protected CancellationToken CancellationToken = default(CancellationToken);
        protected readonly Mock<IRepositoryFactory> MockRepositoryFactory;
        protected readonly Mock<IRepository<TAggregateRoot>> MockRepository;

        protected UnitTestBase()
        {
            this.Fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            this.MockRepositoryFactory = this.Fixture.Freeze<Mock<IRepositoryFactory>>();
            
            this.MockRepository = this.Fixture.Freeze<Mock<IRepository<TAggregateRoot>>>();

            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<TAggregateRoot>())
                .Returns(this.MockRepository.Object);
        }
    }
}
