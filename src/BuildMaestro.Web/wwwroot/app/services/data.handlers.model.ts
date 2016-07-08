/* Model for storing data handlers */
export class DataHandlersModel {
    public handlers: Array<DataHandlerModel> = [];

    constructor() {
        this.addApplicationHandlers();
    }

    public addApplicationHandlers() {
        this.handlers.push({ identifier: DataHandlerIdentifier.GetBuildConfigurations, url: "/Data/BuildConfigurations" });
        this.handlers.push({ identifier: DataHandlerIdentifier.Test, url: "/Data/Test" });
    }
}

/* DataHandler identifiers */
export enum DataHandlerIdentifier {
    GetBuildConfigurations,
    Test
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

