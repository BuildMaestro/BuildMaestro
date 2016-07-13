/* Model for storing data handlers */
export class DataHandlersModel {
    public handlers: Array<DataHandlerModel> = [];

    constructor() {
        this.addApplicationHandlers();
    }

    private addApplicationHandlers() {
        this.handlers.push({ identifier: DataHandlerIdentifier.GetBuildConfigurations, url: "/Data/BuildConfigurations" });
        this.handlers.push({ identifier: DataHandlerIdentifier.StartBuildAgent, url: "/Data/StartBuildAgent" });
        this.handlers.push({ identifier: DataHandlerIdentifier.StartBuildAgent, url: "/Data/StopBuildAgent" });
    }
}

/* DataHandler identifiers */
export enum DataHandlerIdentifier {
    GetBuildConfigurations,
    StartBuildAgent,
    StopBuildAgent
}

/* DataHandler */
export class DataHandlerModel implements IDataHandler {
    public identifier: DataHandlerIdentifier;
    public url: string;
}

export interface IDataHandler {
    identifier: DataHandlerIdentifier;
    url: string;
}

