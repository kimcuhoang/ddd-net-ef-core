using DDDEfCore.Infrastructures.EfCore.Common.Migration;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DDDEfCore.ProductCatalog.Services.Commands.MigrateDatabaseCommands
{
    public class CommandHandler : AsyncRequestHandler<MigrateDatabaseCommand>
    {
        private readonly DatabaseMigration _databaseMigration;

        public CommandHandler(DatabaseMigration databaseMigration)
            => this._databaseMigration = databaseMigration;

        #region Overrides of AsyncRequestHandler<MigrateDatabaseCommand>

        protected override async Task Handle(MigrateDatabaseCommand request, CancellationToken cancellationToken)
        {
            await this._databaseMigration.ApplyMigration();
        }

        #endregion
    }
}
