export class BuildConfigurationModel {
    public id: number;
    public autoDeploy: boolean;
    public buildType: string;
    public deployTag: string;
    public gitRepository: string;
    public gitBranch: string;
    public gitCommit: string;
    public lastDeployDate;
    public name: string;
    public purpose: string;
    public statusText: string;
    public target: string;
    public targetFolders: string[];
    public targetUrls: string[];
}