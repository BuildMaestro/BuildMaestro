﻿import { BuildConfigurationModel } from '../models/build-configuration.model';
import { Component, Input, DoCheck, KeyValueDiffers } from '@angular/core';

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
    providers: [],
    selector: 'build-configuration',
    styleUrls: [ 'buildConfiguration.component.css' ],
    templateUrl: 'buildConfiguration.component.html'
})
export class BuildConfigurationComponent implements DoCheck {
    @Input() configuration: BuildConfigurationModel;
    differ: any;
    wapper: boolean = false;

    constructor(private differs: KeyValueDiffers) {
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