using flights.data.sqlserver.Context;
using flights.domain.Entities;
using flights.domain.Enum;
using flights.domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;


namespace flights.data.sqlserver.Repositories
{
    public class CredentialRepository : ICredentialRepository
    {
        private readonly ContextDb _db;

        public CredentialRepository(ContextDb contextDb)
        {
            _db = contextDb;
        }

        public Credential GetCredentialById(int credencialId)
        {
            try
            {
                return _db.Credential.Include(i => i.Provider).Where(q => q.CredentialId == credencialId).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consultar credencial: " + ex.Message);
            }
        }

        public Credential GetCredentialByProvider(string provider)
        {
            try
            {
                return _db.Credential.Include(i => i.Provider).Where(q => q.Provider.Initials == provider && !q.Deleted).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consultar credencial: " + ex.Message);
            }
        }

        public List<CredentialParameters> GetCredentialParameters(int credentialId)
        {
            try
            {
                return _db.CredentialParameters.Where(q => q.Credential.CredentialId == credentialId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consultar os parâmetros da credencial: " + ex.Message);
            }
        }

        public CredentialContext GetCredentialContext(int applicationId, int providerId)
        {
            try
            {
                EnvironmentType environment;
                switch(new crosscutting.AppConfig.AppConfiguration("Environment", "value").Configuration)
                {
                    case "1":
                        environment = EnvironmentType.Production;
                        break;
                    case "2":
                    default:
                        environment = EnvironmentType.Homologation;
                        break;
                }

                var t = _db.CredentialContext.Where(q => q.Application.ApplicationId == applicationId && q.Provider.ProviderId == providerId && q.Credential.EnvironmentType == environment)
                    .Include(x => x.Credential)
                    .ToList();
                return t.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao consultar o contexto da credencial: " + ex.Message);
            }           
        }
    }
}