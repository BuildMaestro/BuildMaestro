import { BuildConfigurationComponent } from './buildConfiguration.component';
import { BuildConfigurationModel } from '../models/buildConfiguration.model';
import { Component } from '@angular/core';
import { DataHandlerIdentifier} from '../services/data.handlers.model';
import { DataService } from '../services/data.service';

@Component({
    directives: [BuildConfigurationComponent],
    moduleId: module.id,
    providers: [DataService],
    selector: 'build-configurations',
    styleUrls: [ 'buildConfigurations.component.css' ],
    templateUrl: 'buildConfigurations.component.html'
})
export class BuildConfigurationsComponent {
    buildConfigurations: BuildConfigurationModel[];

    constructor(private dataService: DataService) {
        this.buildConfigurations = [];
        this.getData();
    }

    test1() {
        if (this.buildConfigurations.length > 0) {
            this.buildConfigurations[0].name = this.buildConfigurations[0].name + '.';
        }
    }

    test2() {
        this.dataService
            .sendData(DataHandlerIdentifier.Test, {}, this.handleTest2);
    }

    handleTest2(value: any) {
        if (value) {
            console.log('Test send success');
        }
    }

    getData() {
        this.dataService
            .getData(DataHandlerIdentifier.GetBuildConfigurations, response => this.buildConfigurations = response);
    }
}