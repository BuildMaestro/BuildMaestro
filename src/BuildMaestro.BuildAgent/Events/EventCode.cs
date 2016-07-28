using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent.Events
{
    // Corresponds to wwwroot/app/services/event-codes.model.ts
    public enum EventCode
    {
        Unknown,
        Error,
        GitNotInstalled,
        CreatingWorkspace,
        CreatingWorkspaceAlreadyExists,
        CreatingWorkspaceSuccess,
        CreatingWorkspaceFailure,
        GitCloning,
        GitCloningSuccess,
        GitCloningFailure,
        GitFetching,
        GitFetchingSuccess,
        GitFetchingFailure,
        GitMerging,
        GitMergingSucessMerged,
        GitMergingSucessNotMerged,
        GitMergingFailure,
        GitUpdatingLastCommit,
        GitUpdatingLastCommitNoChange,
        GitUpdatingLastCommitChanged,
        BuildStarted,
        BuildSuccess,
        BuildFailure,
        NugetRestoreStarted,
        NugetRestoreSuccess,
        NugetRestoreFailure
    }
}
