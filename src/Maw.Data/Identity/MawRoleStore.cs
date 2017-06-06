using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Maw.Domain.Identity;


namespace Maw.Data.Identity
{
    public class MawRoleStore
		: IRoleStore<MawRole>, IDisposable
    {
		readonly IUserRepository _repo;
		readonly ILogger _log;


		#region ctor
		public MawRoleStore(IUserRepository repo, ILoggerFactory loggerFactory)
		{
			if(loggerFactory == null)
			{
				throw new ArgumentNullException(nameof(loggerFactory));
			}

			_repo = repo ?? throw new ArgumentNullException(nameof(repo));
			_log = loggerFactory.CreateLogger<MawRoleStore>();
		}
		#endregion


		#region IRoleStore
        public async Task<IdentityResult> CreateAsync(MawRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
			if(role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			var result = await _repo.CreateRoleAsync(role.Name, role.Description).ConfigureAwait(false);
            
			return result ? IdentityResult.Success : IdentityResult.Failed();
        }


		public Task<IdentityResult> UpdateAsync(MawRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }


		public async Task<IdentityResult> DeleteAsync(MawRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
			if(role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			var result = await _repo.RemoveRoleAsync(role.Name).ConfigureAwait(false);

			return result ? IdentityResult.Success : IdentityResult.Failed();
        }


        public Task<string> GetRoleIdAsync(MawRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
			if(role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			return Task.FromResult(role.Id.ToString());
        }


		public Task<string> GetRoleNameAsync(MawRole role, CancellationToken cancellationToken = default(CancellationToken))
		{
			if(role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			return Task.FromResult(role.Name);
		}


        public Task<string> GetNormalizedRoleNameAsync(MawRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
			if(role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			return Task.FromResult(role.Name);
        }


		public Task SetNormalizedRoleNameAsync(MawRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
		{
			if(role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			return Task.FromResult(0);
		}


        public Task SetRoleNameAsync(MawRole role, string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
			if(role == null)
			{
				throw new ArgumentNullException(nameof(role));
			}

			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("roleName must not be null and have a value.", nameof(roleName));
			}

			role.Name = roleName;

			return Task.FromResult(0);
        }


        public Task<MawRole> FindByIdAsync(string roleId, CancellationToken cancellationToken = default(CancellationToken))
        {
			if(string.IsNullOrEmpty(roleId))
			{
				throw new ArgumentException("Invalid roleId", nameof(roleId));
			}

			short id;

			if(short.TryParse(roleId, out id))
			{
				return _repo.GetRoleAsync(id);
			}

			throw new ArgumentException("roleId should be able to be parsed into a short.", nameof(roleId));
        }


        public Task<MawRole> FindByNameAsync(string roleName, CancellationToken cancellationToken = default(CancellationToken))
        {
			if(string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentException("Invalid roleName", nameof(roleName));
			}

			return _repo.GetRoleAsync(roleName);
        }
		#endregion


		#region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing){
                // clean up managed resources
            }
        }
		#endregion
    }
}

