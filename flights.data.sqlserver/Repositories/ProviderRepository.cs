using flights.data.sqlserver.Context;
using flights.domain.Entities;
using flights.domain.Interfaces.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace flights.data.sqlserver.Repositories
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly ContextDb _db;
        public ProviderRepository(ContextDb contextDb)
        {
            _db = contextDb;
        }


        public IEnumerable<Provider> GetProvider()
        {
            try
            {
                return _db.Provider.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consultar o fornecedor: " + ex.Message);
            }
        }

        public Provider GetProviderByInitials(string initials)
        {
            try
            {
                return _db.Provider.Where(q => q.Initials == initials).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consultar o fornecedor: " + ex.Message);
            }
        }
    }
}
