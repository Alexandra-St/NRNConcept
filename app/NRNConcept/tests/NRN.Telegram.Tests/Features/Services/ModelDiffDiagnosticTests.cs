using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Internal;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using NRN.Telegram.Features.Services.Persistence;

namespace NRN.Telegram.Tests.Features.Services;

public sealed class ModelDiffDiagnosticTests
{
    [Fact]
    public void SnapshotMatchesCurrentModel()
    {
        var options = new DbContextOptionsBuilder<NrnServicesDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        using var database = new NrnServicesDbContext(options);
        var assembly = typeof(NrnServicesDbContext).Assembly;
        var snapshotType = assembly.GetType(
            "NRN.Telegram.Features.Services.Persistence.Migrations.NrnServicesDbContextModelSnapshot",
            throwOnError: true)!;
        var snapshot = (ModelSnapshot)Activator.CreateInstance(snapshotType, nonPublic: true)!;
        var current = database.GetService<IDesignTimeModel>().Model;
        var initializedSnapshot = database.GetService<IModelRuntimeInitializer>()
            .Initialize(snapshot.Model, designTime: true, validationLogger: null);
        var differ = database.GetService<IMigrationsModelDiffer>();
        var differences = differ.GetDifferences(
            initializedSnapshot.GetRelationalModel(),
            current.GetRelationalModel());

        Assert.True(
            differences.Count == 0,
            string.Join(Environment.NewLine, differences.Select(item => item switch
            {
                AlterColumnOperation column =>
                    $"Alter {column.Table}.{column.Name}: {column.ColumnType}/{column.IsNullable} " +
                    $"from {column.OldColumn.ColumnType}/{column.OldColumn.IsNullable}",
                _ => item.GetType().Name
            })));
    }
}
