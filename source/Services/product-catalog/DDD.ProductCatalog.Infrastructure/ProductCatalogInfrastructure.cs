using Dapper;
using DDD.ProductCatalog.Application.Commands;
using DDD.ProductCatalog.Application.Queries;
using DDD.ProductCatalog.Core;
using DDD.ProductCatalog.Infrastructure.EfCore;
using DNK.DDD.Infrastructure;
using DNK.DDD.Infrastructure.Dapper.MsSqlServer;
using DNK.DDD.Infrastructure.EntityFrameworkCore.MediatR;
using DNK.DDD.Infrastructure.EntityFrameworkCore.MsSqlServer;
using Microsoft.AspNetCore.Builder;
using System.ComponentModel;

namespace DDD.ProductCatalog.Infrastructure;
public static class ProductCatalogInfrastructure
{
    public static WebApplicationBuilder AddProductCatalogInfrastructure(this WebApplicationBuilder builder, string nameOfConnectionString = "Default")
    {
        var commandAssembly = typeof(IProductCatalogCommand<>).Assembly;
        var queryAssembly = typeof(IProductCatalogQueryRequest<>).Assembly;

        builder
            .AddCustomMediatR(commandAssembly, queryAssembly)
            .AddDbContext<ProductCatalogDbContext>(nameOfConnectionString);

        builder
            .AddCustomDapper(nameOfConnectionString);

        StronglyTypedIdTypeDescriptor.AddStronglyTypedIdConverter(idType =>
        {
            var typeOfIdentity = typeof(StronglyTypedIdConverter<>).MakeGenericType(idType);
            TypeDescriptor.AddAttributes(idType, new TypeConverterAttribute(typeOfIdentity));

            var idTypeHandler = typeof(StronglyTypedIdMapper<>).MakeGenericType(idType);
            var idTypeHandlerInstance = (SqlMapper.ITypeHandler)Activator.CreateInstance(idTypeHandler);
            SqlMapper.AddTypeHandler(idType, idTypeHandlerInstance);
        });

        return builder;
    }
}
