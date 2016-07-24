export class EventCodeResourcesModel {
    public eventCodeResources: Array<EventCodeResourceModel> = [];

    constructor() {
        this.addGitEventCodeResources();
    }

    private addGitEventCodeResources() {
        this.eventCodeResources.push({ eventCode: EventCode.Unknown, description: "Unknown git event" });
        this.eventCodeResources.push({ eventCode: EventCode.Error, description: "Error occured" });
        this.eventCodeResources.push({ eventCode: EventCode.GitNotInstalled, description: "Git not installed" });
        this.eventCodeResources.push({ eventCode: EventCode.CreatingWorkspace, description: "Start creating workspace" });
        this.eventCodeResources.push({ eventCode: EventCode.CreatingWorkspaceSuccess, description: "Create workspace successfully finished" });
        this.eventCodeResources.push({ eventCode: EventCode.CreatingWorkspaceFailure, description: "Create workspace error occured" });
        this.eventCodeResources.push({ eventCode: EventCode.GitCloning, description: "Starting git clone command" });
        this.eventCodeResources.push({ eventCode: EventCode.GitCloningSuccess, description: "Git clone successfully finished" });
        this.eventCodeResources.push({ eventCode: EventCode.GitCloningFailure, description: "Git clone error occured" });
        this.eventCodeResources.push({ eventCode: EventCode.GitPull, description: "Starting git pull command" });
        this.eventCodeResources.push({ eventCode: EventCode.GitPullSuccess, description: "Git pull successfully finished" });
        this.eventCodeResources.push({ eventCode: EventCode.GitPullFailure, description: "Git pull error occured" });
        this.eventCodeResources.push({ eventCode: EventCode.GitUpdatingLastCommit, description: "Checking Git commit for change" });
        this.eventCodeResources.push({ eventCode: EventCode.GitUpdatingLastCommitChanged, description: "Git commit changed" });
        this.eventCodeResources.push({ eventCode: EventCode.GitUpdatingLastCommitNoChange, description: "Latest git commit not changed" });
    }

    public getDescription(eventCode: EventCode) {
        for (var i = 0, length = this.eventCodeResources.length; i < length; i++) {
            if (this.eventCodeResources[i].eventCode === eventCode) {
                return this.eventCodeResources[i].description;
            }
        }

        return '';
    }
}

export enum EventCode {
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
    GitUpdatingLastCommit,
    GitUpdatingLastCommitNoChange,
    GitUpdatingLastCommitChanged
}

export class EventCodeResourceModel implements IEventCodeResourceModel {
    public eventCode: EventCode;
    public description: string;
}

export interface IEventCodeResourceModel {
    eventCode: EventCode;
    description: string;
}

