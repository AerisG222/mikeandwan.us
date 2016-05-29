import { bootstrap } from '@angular/platform-browser-dynamic';
import { ROUTER_PROVIDERS } from '@angular/router-deprecated';
import { MoneySpinApp } from './components/App';
import { StateService } from './services/StateService';

bootstrap(MoneySpinApp, [ ROUTER_PROVIDERS,
                          StateService ])
    .catch(err => console.error(err));
