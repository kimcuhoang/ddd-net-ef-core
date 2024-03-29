﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace DNK.DDD.Infrastructure.EntityFrameworkCore.MsSqlServer.Conventions;

public class TableNameConvention : IModelFinalizingConvention
{
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
        modelBuilder.Metadata
            .GetEntityTypes()
            .ToList()
            .ForEach(e => e.SetTableName(e.GetDefaultTableName()));
    }
}
