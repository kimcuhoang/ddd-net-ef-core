using AutoFixture;
using DDDEfCore.Core.Common;
using DDDEfCore.Core.Common.Models;
using Moq;
using System.Security.Principal;
using System.Threading;

namespace DDDEfCore.ProductCatalog.Services.Commands.Tests;

public abstract class UnitTestBase<TAggregateRoot, TIdentity> where TAggregateRoot : AggregateRoot<TIdentity> where TIdentity : IdentityBase
{
    protected readonly IFixture Fixture;
    protected readonly CancellationToken CancellationToken;
    protected readonly Mock<IRepositoryFactory> MockRepositoryFactory;
    protected readonly Mock<IRepository<TAggregateRoot, TIdentity>> MockRepository;

    protected UnitTestBase()
    {
        this.Fixture = new Fixture();
        
        this.MockRepositoryFactory = new Mock<IRepositoryFactory>();
        
        this.MockRepository = new Mock<IRepository<TAggregateRoot, TIdentity>>();

        this.MockRepositoryFactory
            .Setup(x => x.CreateRepository<TAggregateRoot, TIdentity>())
            .Returns(this.MockRepository.Object);

        this.CancellationToken = new CancellationToken(false);
    }
}

public abstract class UnitTestBase
{
    protected readonly IFixture Fixture;
    protected readonly CancellationToken CancellationToken;
    protected readonly Mock<IRepositoryFactory> MockRepositoryFactory;

    protected UnitTestBase()
    {
        this.Fixture = new Fixture();

        this.MockRepositoryFactory = new Mock<IRepositoryFactory>();

        this.CancellationToken = new CancellationToken(false);
    }

    protected Mock<IRepository<T, TId>> GetRepository<T, TId>(Action<Mock<IRepository<T, TId>>> configure = null) 
            where T: AggregateRoot<TId>
            where TId: IdentityBase
    {
        var mockRepository = new Mock<IRepository<T, TId>>();

        configure?.Invoke(mockRepository);

        this.MockRepositoryFactory
            .Setup(_ => _.CreateRepository<T, TId>())
            .Returns(mockRepository.Object);

        return mockRepository;
    }
}
