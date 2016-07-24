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

    public startConnection(): void {
        var self = this;

        self.connection.start().done((data) => {
            console.log('Now connected ' + data.transport.name + ', connection ID= ' + data.id);

            self.connectionEstablished.emit(true);
            self.connectionExists = true;
        }).fail((error) => {
            console.log('Could not connect ' + error);

            self.connectionEstablished.emit(false);
        });

        self.connection.disconnected(function () {
            console.log('Now disconnected');

            self.connectionEstablished.emit(false);
            self.connectionExists = false;

            setTimeout(function () {
                self.startConnection();
            }, 5000); 
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

