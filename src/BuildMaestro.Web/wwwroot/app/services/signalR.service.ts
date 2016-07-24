import { Injectable, EventEmitter } from '@angular/core';
import { CONFIGURATION } from '../shared/app.constants';

declare var $;

@Injectable()
export class SignalRService {

    private proxy: HubProxy;
    private proxyName: string = 'buildagent';
    private connection;

    public statusChanged: EventEmitter<StatusChangeEvent>;
    public connectionEstablished: EventEmitter<Boolean>;
    public connectionExists: Boolean;

    constructor() {
        this.connectionEstablished = new EventEmitter<Boolean>();
        this.connectionExists = false;
        this.statusChanged = new EventEmitter<StatusChangeEvent>();

        this.connection = $.hubConnection(CONFIGURATION.baseUrls.server + 'signalr/');
        this.proxy = this.connection.createHubProxy(this.proxyName);

        this.registerOnServerEvents();

        this.startConnection();
    }

    private startConnection(): void {
        this.connection.start().done((data) => {
            console.log('Now connected ' + data.transport.name + ', connection ID= ' + data.id);
            this.connectionEstablished.emit(true);
            this.connectionExists = true;
        }).fail((error) => {
            console.log('Could not connect ' + error);
            this.connectionEstablished.emit(false);
        });
    }

    private registerOnServerEvents(): void {
        this.proxy.on('statusChange', (buildConfigurationId: number, eventCode: number, data: any) => {
            var statusChangeEvent = new StatusChangeEvent();
            statusChangeEvent.buildConfigurationId = buildConfigurationId;
            statusChangeEvent.eventCode = eventCode;
            statusChangeEvent.data = JSON.parse(data);
            this.statusChanged.emit(statusChangeEvent);
        });
    }
}

export class StatusChangeEvent {
    buildConfigurationId: number;
    eventCode: number;
    data: any;
}

