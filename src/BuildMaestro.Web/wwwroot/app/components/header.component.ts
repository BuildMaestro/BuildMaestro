import { Component } from '@angular/core';
@Component({
    moduleId: module.id,
    selector: 'header',
    styleUrls: ['header.component.css'],
    templateUrl: 'header.component.html'
})
export class HeaderComponent {
    title: string;

    constructor() {
        this.title = "BuildMaestro 1.0"
    }
}