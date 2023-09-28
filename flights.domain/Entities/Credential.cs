using flights.domain.Enum;
using System;
using System.Collections.Generic;

namespace flights.domain.Entities
{
    public class Credential
    {
        public int CredentialId { get; set; }
        public bool Active { get; set; }
        public EnvironmentType EnvironmentType { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int ProviderId { get; set; }
        public virtual Provider Provider { get; set; }
        public int AgencyId { get; set; }
        public virtual Agency Agency { get; set; }
        public virtual List<CredentialParameters> CredentialParameters { get; set; }
        public bool Deleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
