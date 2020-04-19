using MAAV.DataContracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MAAV.Application
{
    public interface IVersionService
    {
        Task<SemanticVersion> UpdateVersionOnBranches(string organisationName, string teamName, string appName, BranchActionRequest data);
        Task<SemanticVersion> GetVersionFromSourceBranchAsync(string organisationName, string teamName, string appName, BranchActionRequest data);
        Task<KeyBranchVersionHistory> LoadHistoryFromKeyBranchName(string organisationId, string teamId, string appId, string keyBranchName);
    }
}