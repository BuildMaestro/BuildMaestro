import { BuildConfigurationsComponent } from './buildConfigurations.component';
import { BuildConfigurationModel } from '../models/buildConfiguration.model';
import { Component, Inject, OnInit, NgZone } from '@angular/core';
import { CORE_DIRECTIVES } from '@angular/common';
import { DataHandlerIdentifier} from '../services/data.handlers.model';
import { DataService } from '../services/data.service';
import { HeaderComponent } from './header.component';
import { SignalRService } from '../services/signalR.Service';

@Component({
    directives: [HeaderComponent, BuildConfigurationsComponent],
    moduleId: module.id,
    providers: [SignalRService],
    selector: 'buildmaestro-app',
    styleUrls: [ 'app.component.css' ],
    templateUrl: 'app.component.html'
})
export class AppComponent implements OnInit {
    cpuValue: number = 0;

    constructor(private signalRService: SignalRService, @Inject(NgZone) private zone: NgZone) {
        //constructor(private signalRService: SignalRService, @Inject(NgZone) private zone:NgZone) {
    }

    public ngOnInit() {
        this.subscribeToEvents();
    }

    subscribeToEvents(): void {
        this.signalRService.newCpuValue.subscribe((cpuValue: number) => {
            this.zone.run(() => {
                this.cpuValue = cpuValue
            });
        });
    }
}