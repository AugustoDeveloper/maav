using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAAV.Application
{
    public interface IVersionService
    {
        Task<string> GetVersionFromSourceBranchAsync(string organisationName, string teamName, string appName, string source, object data);
        Task<string> GetVersionFromSourceAndTagetBranchAsync(string organisationName, string teamName, string appName,string source, string target, object data);
    }
}