using DDDEF.Core.Projects;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit.Abstractions;

namespace DDDEF.Tests.TestPersistence;
public class SaveProject(TestCollectionFixture testCollectionFixture, ITestOutputHelper testOutput) : E2eTestBase(testCollectionFixture, testOutput)
{
    private Project Project { get; set; } = default!;

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        await this.ExecuteDbContextAsync(async db =>
        {
            await db.Projects.ExecuteDeleteAsync();
        });
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        this.Project = Project.New(p =>
        {
            p.Name = this.Faker.Lorem.Sentence(wordCount: 10);
            p.StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
            p.EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10));
        });

        await this.ExecuteDbContextAsync(async db =>
        {
            db.Add(this.Project);
            await db.SaveChangesAsync();
        });
    }


    [Fact]
    public async Task ShouldBeSuccess()
    {
        await this.ExecuteDbContextAsync(async db =>
        {
            var project = await db.Projects.SingleOrDefaultAsync();

            project
                .ShouldNotBeNull()
                .ShouldSatisfyAllConditions(_ =>
                {
                    _.ShouldBe(this.Project);
                    _.Name.ShouldBe(this.Project.Name);
                    _.StartDate.ShouldBe(this.Project.StartDate);
                    _.EndDate.ShouldBe(this.Project.EndDate);
                });
        });
    }
}
