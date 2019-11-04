using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using Moq;
using System.Threading;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests
{
    public abstract class UnitTestBase<TAggregateRoot> where TAggregateRoot : AggregateRoot
    {
        protected readonly IFixture Fixture;
        protected readonly CancellationToken CancellationToken;
        protected readonly Mock<IRepositoryFactory> MockRepositoryFactory;
        protected readonly Mock<IRepository<TAggregateRoot>> MockRepository;

        protected UnitTestBase()
        {
            this.Fixture = new Fixture();
            
            this.MockRepositoryFactory = new Mock<IRepositoryFactory>();
            
            this.MockRepository = new Mock<IRepository<TAggregateRoot>>();

            this.MockRepositoryFactory
                .Setup(x => x.CreateRepository<TAggregateRoot>())
                .Returns(this.MockRepository.Object);

            this.CancellationToken = new CancellationToken(false);
        }
    }
}
