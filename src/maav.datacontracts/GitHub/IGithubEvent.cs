using System;
using System.Collections.Generic;
using System.Text;

namespace MAAV.DataContracts.GitHub
{
    public interface IGithubEvent
    {
        string OrganisationId { get; set; }
        string TeamId { get; set; }
        string ApplicationId { get; set; }
    }
}
