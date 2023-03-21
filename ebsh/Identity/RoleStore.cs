using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using eBSH.Core;

namespace Identity.Core
{
    public class RoleStore<TRole> : IRoleStore<TRole, Guid>, IQueryableRoleStore<TRole, Guid>
        where TRole : IdentityRole
    {
        protected IDbSession DbSession;
        public RoleStore(IDbSession DbSession)
        {
            this.DbSession = DbSession;
        }

        public Task CreateAsync(TRole role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            string sql = @"
            INSERT INTO [dbo].[IdentityRole]
                       ([RoleId]
                       ,[Name])
                 VALUES
                       (@ROLEID
                       ,@NAME)";

            if (role.RoleId == default(Guid))
                role.RoleId = Guid.NewGuid();

            return Task.FromResult(DbSession.Connection.Execute(sql, role));
        }

        public Task DeleteAsync(TRole role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            string sql = @"
            DELETE FROM IdentityRole 
            WHERE RoleId=@ROLEID";

            return Task.FromResult(DbSession.Connection.Execute(sql, role));
        }

        public Task<TRole> FindByIdAsync(Guid roleId)
        {
            var sql = @"
            SELECT 
                *
            FROM 
                IdentityRole   
            WHERE 
                RoleId=@ROLEID";

            return Task.FromResult<TRole>(DbSession.Connection.Query<TRole>(sql, roleId).SingleOrDefault());
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            if (String.IsNullOrWhiteSpace(roleName))
                throw new ArgumentNullException("roleName");

            var sql = @"
            SELECT 
                *
            FROM 
                IdentityRole   
            WHERE 
                Name=@NAME";

            return Task.FromResult<TRole>(DbSession.Connection.Query<TRole>(sql, new { Name = roleName }).SingleOrDefault());
        }

        public Task UpdateAsync(TRole role)
        {
            if (role == null)
                throw new ArgumentNullException("role");

            string sql = @"
            UPDATE IdentityRole SET 
	            [Name] = @NAME
            WHERE 
                RoleId = @ROLEID";

            return Task.FromResult(DbSession.Connection.Execute(sql, role));
        }

        public void Dispose()
        {
            if (DbSession != null)
            {
                DbSession = null;
            }
        }

        /* IQueryableRoleStore
        ---------------------------*/

        public IQueryable<TRole> Roles
        {
            get
            {
                var sql = @"
                SELECT 
                    * 
                FROM IdentityRole";

                var result = DbSession.Connection.Query<TRole>(sql);

                return result.AsQueryable();

            }
        }
    }
}
