/* Model for storing data handlers */
export class GitEventCodeResourcesModel {
    public gitEventCodeResources: Array<GitEventCodeResourceModel> = [];

    constructor() {
        this.addGitEventCodeResources();
    }

    private addGitEventCodeResources() {
        this.gitEventCodeResources.push({ eventCode: GitEventCode.Unknown, description: "Unknown git event" });
        this.gitEventCodeResources.push({ eventCode: GitEventCode.Error, description: "Error occured" });
        this.gitEventCodeResources.push({ eventCode: GitEventCode.GitNotInstalled, description: "Git not installed" });
        this.gitEventCodeResources.push({ eventCode: GitEventCode.CreatingWorkspace, description: "Start creating workspace" });
        this.gitEventCodeResources.push({ eventCode: GitEventCode.CreatingWorkspaceSuccess, description: "Create workspace successfully finished" });
        this.gitEventCodeResources.push({ eventCode: GitEventCode.CreatingWorkspaceFailure, description: "Create workspace error occured" });
        this.gitEventCodeResources.push({ eventCode: GitEventCode.GitCloning, description: "Starting git clone command" });
        this.gitEventCodeResources.push({ eventCode: GitEventCode.GitCloningSuccess, description: "Git clone successfully finished" });
        this.gitEventCodeResources.push({ eventCode: GitEventCode.GitCloningFailure, description: "Git clone error occured" });
        this.gitEventCodeResources.push({ eventCode: GitEventCode.GitPull, description: "Starting git pull command" });
        this.gitEventCodeResources.push({ eventCode: GitEventCode.GitPullSuccess, description: "Git pull successfully finished" });
        this.gitEventCodeResources.push({ eventCode: GitEventCode.GitPullFailure, description: "Git pull error occured" });
    }

    public getDescription(eventCode: GitEventCode) {
        for (var i = 0, length = this.gitEventCodeResources.length; i < length; i++) {
            if (this.gitEventCodeResources[i].eventCode === eventCode) {
                return this.gitEventCodeResources[i].description;
            }
        }

        return '';
    }
}

/* DataHandler identifiers,  */
export enum GitEventCode {
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

/* DataHandler */
export class GitEventCodeResourceModel implements IGitEventCodeResourceModel {
    public eventCode: GitEventCode;
    public description: string;
}

export interface IGitEventCodeResourceModel {
    eventCode: GitEventCode;
    description: string;
}

