using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using mvc.Models;
using System.Data.SqlClient;
using System.Data.Common;
using Dapper;

namespace mvc.Security
{
    public class MvcUserStore : IUserStore<MvcUser>, IUserPasswordStore<MvcUser>
    {
        public async Task<IdentityResult> CreateAsync(MvcUser user, CancellationToken cancellationToken)
        {
            using(var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "insert into MvcUsers ([Id], [UserName], [NormalizedUserName], [PasswordHash]) values (@id, @userName, @normalizedUserName, @passwordHash)",
                    new{
                        id = user.Id,
                        userName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        passwordHash = user.PasswordHash
                    }
                );
            }

            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(MvcUser user, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public void Dispose()
        {
        }

        public static DbConnection GetOpenConnection()
        {
            var connection = new SqlConnection("Data Source=.;database=dummy;trusted_connection=yes;");
            connection.Open();
            return connection;
        }

        public async Task<MvcUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            using(var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<MvcUser>(
                    "select * from MvcUsers where [Id] = @id",
                    new{
                        id = userId
                    }
                );
            }
        }

        public async Task<MvcUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            using(var connection = GetOpenConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<MvcUser>(
                    "select * from MvcUsers where [NormalizedUserName] = @userName",
                    new{
                        userName = normalizedUserName
                    }
                );
            }
        }

        public Task<string> GetNormalizedUserNameAsync(MvcUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(MvcUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(MvcUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(MvcUser user, string normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(MvcUser user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(MvcUser user, CancellationToken cancellationToken)
        {
            using(var connection = GetOpenConnection())
            {
                await connection.ExecuteAsync(
                    "update MvcUsers set [Id] = @id, [UserName] = @userName, [NormalizedUserName] = @normalizedUserName, [PasswordHash] = @passwordHash where [Id] = @id",
                    new{
                        id = user.Id,
                        userName = user.UserName,
                        normalizedUserName = user.NormalizedUserName,
                        passwordHash = user.PasswordHash
                    }
                );
            }

            return IdentityResult.Success;
        }

        public Task SetPasswordHashAsync(MvcUser user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(MvcUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(MvcUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }
    }
}