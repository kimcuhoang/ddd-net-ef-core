using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture.Xunit2;
using DDD.ProductCatalog.Core.Exceptions;
using DDD.ProductCatalog.Core.Products;
using Shouldly;
using Xunit;

namespace DDD.ProductCatalog.Core.Tests.TestProduct
{
    public class TestProductCreation
    {
        [Theory(DisplayName = "Create Product Successfully")]
        [AutoData]
        public void CreateProductSuccessfully(string productName)
        {
            var product = Product.Create(productName);

            product.ShouldNotBeNull();
            product.Id.ShouldNotBeNull();
            product.Id.ShouldBeAssignableTo<ProductId>();
            product.Name.ShouldBe(productName);
        }

        [Fact(DisplayName = "Create Product with Empty Name Should Throw Exception")]
        public void CreateProduct_With_EmptyName_ShouldThrow_Exception()
            => Should.Throw<DomainException>(() => Product.Create(string.Empty));
    }
}
