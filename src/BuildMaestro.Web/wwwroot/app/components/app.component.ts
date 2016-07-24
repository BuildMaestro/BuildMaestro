import { BuildConfigurationsComponent } from './buildConfigurations.component';
import { BuildConfigurationModel } from '../models/build-configuration.model';
import { Component, Inject, OnInit, NgZone } from '@angular/core';
import { CORE_DIRECTIVES } from '@angular/common';
import { HeaderComponent } from './header.component';

@Component({
    directives: [HeaderComponent, BuildConfigurationsComponent],
    moduleId: module.id,
    providers: [],
    selector: 'buildmaestro-app',
    styleUrls: [ 'app.component.css' ],
    templateUrl: 'app.component.html'
})
export class AppComponent {
    constructor() {
    }
}