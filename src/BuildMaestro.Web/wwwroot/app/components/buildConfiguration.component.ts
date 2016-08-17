import { BuildConfigurationModel } from '../models/build-configuration.model';
import { Component, Input, DoCheck, KeyValueDiffers } from '@angular/core';
import { PolymerElement } from '@vaadin/angular2-polymer';

import { animate, keyframes } from '@angular/core';
import { style, state } from '@angular/core';
import { transition } from '@angular/core';
import { trigger } from '@angular/core';

@Component({
    animations: [
        trigger("wapper", [
            transition("* => *", [
                animate(2000, keyframes([
                    style({ background: 'inherit' }),
                    style({ background: '#ff0000' }),
                    style({ background: '#FF7070' })
                ]))
            ])
        ])
        //trigger("wapper", [
        //    transition("* => *", [
        //        animate(300, keyframes([
        //            style({ opacity: 1, transform: 'translateX(15px)', offset: 0.3 }),
        //            style({ opacity: 1, transform: 'translateX(0)', offset: 1.0 })
        //        ]))
        //    ])
        //])
    ],
    moduleId: module.id,
    directives: [PolymerElement('paper-input')],
    providers: [],
    selector: 'build-configuration',
    styleUrls: ['buildConfiguration.component.css'],
    templateUrl: 'buildConfiguration.component.html'
})
export class BuildConfigurationComponent implements DoCheck {
    @Input() configuration: BuildConfigurationModel;
    differ: any;
    test: string;
    wapper: boolean = false;

    constructor(private differs: KeyValueDiffers) {
        this.differ = differs.find({}).create(null);
    }

    showAlert() {
        alert(this.test);
    }

    ngDoCheck() {
        var changes = this.differ.diff(this.configuration);

        if (changes) {
            this.wapper = !this.wapper;
        }
    }
}