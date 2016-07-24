export class GitCommitModel implements IGitCommitModel {
    public author: string;
    public dateTime: string;
    public hash: string;
    public message: string;
}

export interface IGitCommitModel {
    author: string;
    dateTime: string;
    hash: string;
    message: string;
}