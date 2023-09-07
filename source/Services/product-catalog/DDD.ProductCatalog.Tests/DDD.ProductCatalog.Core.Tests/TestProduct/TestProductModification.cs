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
    public class TestProductModification
    {
        [Theory(DisplayName = "Change Product's Name Successfully")]
        [AutoData]
        public void ChangeProductNameSuccessfully(string originProductName, string productName)
        {
            var product = Product
                .Create(originProductName)
                .ChangeName(productName);

            product.Name.ShouldBe(productName);
        }

        [Theory(DisplayName = "Change Product's Name to empty should throw Exception")]
        [AutoData]
        public void Change_ProductName_To_Empty_Should_ThrowException(string originProductName)
        {
            var product = Product.Create(originProductName);
            Should.Throw<DomainException>(() => product.ChangeName(string.Empty));
        }
    }
}
