import { AppComponent } from './components/app.component';
import { APP_BASE_HREF, HashLocationStrategy, LocationStrategy } from '@angular/common';
import { bootstrap }    from '@angular/platform-browser-dynamic';
import { HTTP_PROVIDERS } from '@angular/http';
import { ROUTER_PROVIDERS } from '@angular/router-deprecated';
import { provide, bind } from '@angular/core';
import { SignalRService } from './services/signalR.service';

import 'rxjs/Rx';

bootstrap(AppComponent, [
    HTTP_PROVIDERS,
    ROUTER_PROVIDERS,
    SignalRService,
    bind(LocationStrategy).toClass(HashLocationStrategy),
    provide(APP_BASE_HREF, { useValue: '/' })
]);
