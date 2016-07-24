import { Component, Inject, Input, OnInit, NgZone } from '@angular/core';
import { SignalRService, StatusChangeEvent } from '../services/signalR.Service';

@Component({
    moduleId: module.id,
    providers: [SignalRService],
    selector: 'header',
    styleUrls: ['header.component.css'],
    templateUrl: 'header.component.html'
})
export class HeaderComponent implements OnInit{
    title: string;
    connectionState: ConnectionState;
    ConnectionState = ConnectionState;

    constructor(private signalRService: SignalRService, @Inject(NgZone) private zone: NgZone) {
        this.connectionState = ConnectionState.Connecting;
        this.title = "BuildMaestro 1.0"
    }

    public ngOnInit() {
        this.subscribeToEvents();
    }

    subscribeToEvents(): void {
        this.signalRService.connectionEstablished.subscribe((established: boolean) => {
            this.zone.run(() => {
                if (established) {
                    this.handleConnectionEstablished();
                }
                else {
                    this.handleConnectionLost();
                }
            });
        });
    }

    handleConnectionEstablished() {
        this.connectionState = ConnectionState.Connected
    }

    handleConnectionLost() {
        var self = this;

        this.connectionState = ConnectionState.Disconnected;

        setTimeout(function () {
            this.connectionState = ConnectionState.Connecting;
            self.signalRService.startConnection();
        }, 5000);
    }
}

export enum ConnectionState {
    Disconnected,
    Connecting,
    Connected
}