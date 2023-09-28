using flights.data.sqlserver.Context;
using flights.domain.Entities;
using flights.domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace flights.data.sqlserver.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ContextDb _db;

        public UserRepository(ContextDb contextDb)
        {
            _db = contextDb;
        }

        public User GetByUsername(string username)
        {
            try
            {
                return _db.User.Where(q => q.Username == username).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter usuário: " + ex.Message);
            }
            
        }
    }
}
