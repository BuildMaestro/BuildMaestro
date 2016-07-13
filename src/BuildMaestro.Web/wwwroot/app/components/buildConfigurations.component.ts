import { BuildConfigurationComponent } from './buildConfiguration.component';
import { BuildConfigurationModel } from '../models/buildConfiguration.model';
import { Component, Inject, NgZone, OnInit } from '@angular/core';
import { DataHandlerIdentifier} from '../services/data-handlers.model';
import { GitEventCodeResourcesModel } from '../services/git-event-code-resources.model';
import { DataService } from '../services/data.service';
import { SignalRService, BcEvent } from '../services/signalR.Service';

@Component({
    directives: [BuildConfigurationComponent],
    moduleId: module.id,
    providers: [DataService, GitEventCodeResourcesModel, SignalRService],
    selector: 'build-configurations',
    styleUrls: [ 'buildConfigurations.component.css' ],
    templateUrl: 'buildConfigurations.component.html'
})
export class BuildConfigurationsComponent implements OnInit {
    buildConfigurations: BuildConfigurationModel[];
    gitEventCodeResources: GitEventCodeResourcesModel;

    constructor(private dataService: DataService,
                private signalRService: SignalRService,
                @Inject(NgZone) private zone: NgZone) {

        this.buildConfigurations = [];
        this.gitEventCodeResources = new GitEventCodeResourcesModel();
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
        this.signalRService.bcEvent.subscribe((data: BcEvent) => {
            this.zone.run(() => {
                if (data.buildConfigurationId != 0) {
                    this.dododo(data.buildConfigurationId, data.eventCode);
                }
            });
        });
    }

    dododo(buildconfigurationId: number, eventCode: number): void {
        for (var i = 0, length = this.buildConfigurations.length; i < length; i++) {
            if (this.buildConfigurations[i].id == buildconfigurationId) {
                this.buildConfigurations[i].statusText = this.gitEventCodeResources.getDescription(eventCode);
            }
        }
    }
}