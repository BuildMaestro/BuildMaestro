import { GitCommitModel } from './git-commit.model';

export class BuildConfigurationModel {
    public active: boolean;
    public id: number;
    public autoDeploy: boolean;
    public buildType: string;
    public deployTag: string;
    public gitRepository: string;
    public gitBranch: string;
    public latestGitCommit: GitCommitModel;
    public lastDeployDate;
    public name: string;
    public purpose: string;
    public statusText: string;
    public target: string;
    public targetFolders: string[];
    public targetUrls: string[];
}