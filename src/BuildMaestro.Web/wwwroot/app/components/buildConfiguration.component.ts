import { BuildConfigurationModel } from '../models/buildConfiguration.model';
import { Component, Input, DoCheck, KeyValueDiffers } from '@angular/core';
import { DataHandlerIdentifier} from '../services/data.handlers.model';
import { DataService } from '../services/data.service';

import { animate, keyframes } from '@angular/core';
import { style, state } from '@angular/core';
import { transition } from '@angular/core';
import { trigger } from '@angular/core';

@Component({
    animations: [
        trigger("wapper", [
            transition("* => *", [
                animate(300, keyframes([
                    style({ opacity: 1, transform: 'translateX(15px)', offset: 0.3 }),
                    style({ opacity: 1, transform: 'translateX(0)', offset: 1.0 })
                ]))
            ])
        ])
    ],
    moduleId: module.id,
    providers: [DataService],
    selector: 'build-configuration',
    styleUrls: [ 'buildConfiguration.component.css' ],
    templateUrl: 'buildConfiguration.component.html'
})
export class BuildConfigurationComponent implements DoCheck {
    @Input() configuration: BuildConfigurationModel;
    differ: any;
    wapper: boolean = false;

    constructor(private dataService: DataService, private differs: KeyValueDiffers) {
        this.differ = differs.find({}).create(null);
    }

    ngDoCheck() {
        var changes = this.differ.diff(this.configuration);

        if (changes) {
            console.log('changes detected');
            this.wapper = !this.wapper;
        }
    }
}