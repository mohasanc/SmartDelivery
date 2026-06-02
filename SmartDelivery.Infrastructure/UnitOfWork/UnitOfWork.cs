using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SmartDelivery.Application.Interfaces.Repositories;
using SmartDelivery.Infrastructure.Data;
using SmartDelivery.Infrastructure.Repositories;

namespace SmartDelivery.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        private IUserRepository? _users;
        private IRoleRepository? _roles;
        private IRestaurantRepository? _restaurants;
        private ICategoryRepository? _categories;
        private IMenuItemRepository? _menuItems;
        private IOrderRepository? _orders;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }
        public IUserRepository Users => _users ??= new UserRepository(_context);
        public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
        public IRestaurantRepository Restaurants => _restaurants ??= new RestaurantRepository(_context);
        public ICategoryRepository Categories => _categories ??= new CategoryRepository(_context);
        public IMenuItemRepository MenuItems => _menuItems ??= new MenuItemRepository(_context);
        public IOrderRepository Orders => _orders ??= new OrderRepository(_context);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            });
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is not null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
        {
            if(_transaction is not null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }

    }
}