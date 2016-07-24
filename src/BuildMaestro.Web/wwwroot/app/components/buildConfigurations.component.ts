import { BuildConfigurationComponent } from './buildConfiguration.component';
import { BuildConfigurationModel } from '../models/build-configuration.model';
import { Component, Inject, NgZone, OnInit } from '@angular/core';
import { DataHandlerIdentifier} from '../services/data-handlers.model';
import { DataService } from '../services/data.service';
import { EventCodeResourcesModel, EventCode } from '../services/event-code-resources.model';
import { SignalRService, StatusChangeEvent } from '../services/signalR.Service';

@Component({
    directives: [BuildConfigurationComponent],
    moduleId: module.id,
    providers: [DataService, EventCodeResourcesModel, SignalRService],
    selector: 'build-configurations',
    styleUrls: [ 'buildConfigurations.component.css' ],
    templateUrl: 'buildConfigurations.component.html'
})
export class BuildConfigurationsComponent implements OnInit {
    buildConfigurations: BuildConfigurationModel[];
    gitEventCodeResources: EventCodeResourcesModel;

    constructor(private dataService: DataService,
                private signalRService: SignalRService,
                @Inject(NgZone) private zone: NgZone) {

        this.buildConfigurations = [];
        this.gitEventCodeResources = new EventCodeResourcesModel();
        this.getData();
    }

    public ngOnInit() {
        this.subscribeToEvents();
    }

    start() {
        this.dataService
            .sendData(DataHandlerIdentifier.StartBuildAgent, {}, this.handleStart);
    }

    stop() {
        this.dataService
            .sendData(DataHandlerIdentifier.StopBuildAgent, {}, this.handleStop);
    }

    handleStart(value: any) {
        if (value) {
            console.log('Build agent start success');
        }
    }

    handleStop(value: any) {
        if (value) {
            console.log('Build agent stop success');
        }
    }

    getData() {
        this.dataService
            .getData(DataHandlerIdentifier.GetBuildConfigurations, response => this.buildConfigurations = response);
    }

    subscribeToEvents(): void {
        this.signalRService.statusChanged.subscribe((event: StatusChangeEvent) => {
            this.zone.run(() => {
                if (event.buildConfigurationId != 0) {
                    this.handleStatusChange(event.buildConfigurationId, event.eventCode, event.data);
                }
            });
        });
    }

    handleStatusChange(buildconfigurationId: number, eventCode: number, data: any): void {
        var buildConfiguration = this.buildConfigurations.find((config) => config.id === buildconfigurationId);

        if (buildConfiguration) {
            buildConfiguration.statusText = this.gitEventCodeResources.getDescription(eventCode);

            if (eventCode === EventCode.GitUpdatingLastCommitChanged) {
                buildConfiguration.latestGitCommit = data;
            }
        }
    }
}