import { Injectable, EventEmitter } from '@angular/core';
import { CONFIGURATION } from '../shared/app.constants';

declare var $;

@Injectable()
export class SignalRService {

    private proxy: HubProxy;
    private proxyName: string = 'buildagent';
    private connection;

    public bcEvent: EventEmitter<BcEvent>;
    public connectionEstablished: EventEmitter<Boolean>;
    public connectionExists: Boolean;

    constructor() {
        this.connectionEstablished = new EventEmitter<Boolean>();
        this.bcEvent = new EventEmitter<BcEvent>();
        this.connectionExists = false;

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
        this.proxy.on('bcStatusChange', (buildConfigurationId: number, eventCode: number) => {
            var eventData = new BcEvent();
            eventData.buildConfigurationId = buildConfigurationId;
            eventData.eventCode = eventCode;
            this.bcEvent.emit(eventData);
        });
    }
}

export class BcEvent {
    buildConfigurationId: number;
    eventCode: number;
}

