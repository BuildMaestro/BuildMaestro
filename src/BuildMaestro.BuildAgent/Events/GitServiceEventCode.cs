using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildMaestro.BuildAgent.Events
{
    // Corresponds to wwwroot/app/services/event-codes.model.ts
    public enum GitServiceEventCode
    {
        Unknown,
        Error,
        GitNotInstalled,
        CreatingWorkspace,
        CreatingWorkspaceSuccess,
        CreatingWorkspaceFailure,
        GitCloning,
        GitCloningSuccess,
        GitCloningFailure,
        GitPull,
        GitPullSuccess,
        GitPullFailure,
    }
}
