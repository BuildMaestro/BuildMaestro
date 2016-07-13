import {DataHandlersModel, DataHandlerIdentifier, IDataHandler} from './data-handlers.model';
import { Injectable }     from '@angular/core';
import { Http, Response } from '@angular/http';
import 'rxjs/Rx'
import 'rxjs/add/operator/map'
import { Observer } from 'rxjs/Observer';
import { Observable }     from 'rxjs/Observable';

@Injectable()
export class DataService {
    private handlers: Array<IDataHandler>;
    private http: any; // actually : Http

    constructor(http: Http) {
        var dataHandlersModel = new DataHandlersModel();

        this.handlers = dataHandlersModel.handlers;
        this.http = http;
    }

    public getData(identifier: DataHandlerIdentifier, observableOrNext: Observer<any> | ((value: any) => void)): void {
        var handler: IDataHandler = this.getHandler(identifier);

        if (handler !== null) {
            this.http.get(handler.url)
                .map(res => res.json())
                .subscribe(observableOrNext);
        }
    }

    public sendData(identifier: DataHandlerIdentifier, data: any, observableOrNext: Observer<any> | ((value: any) => void)): void {
        var handler: IDataHandler = this.getHandler(identifier);

        if (handler !== null) {
            this.http.post(handler.url, data)
                .map(res => res.json())
                .subscribe(observableOrNext);
        }
    }

    private getHandler(identifier: DataHandlerIdentifier): IDataHandler {
        for (var i = 0, length = this.handlers.length; i < length; i++) {
            if (this.handlers[i].identifier === identifier) {
                return this.handlers[i];
            }
        }

        return null;
    }
}
