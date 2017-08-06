import { Component, ViewChild } from '@angular/core';
import { animate, group, query, stagger, sequence, state, style, trigger, transition } from '@angular/animations';

import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { PhotoStateService } from './shared/photo-state.service';
import { PhotoNavigationService } from './shared/photo-navigation.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.css' ],
    animations: [
        trigger('routeAnimation', [
            transition('* => *', [
                // make sure the new page is hidden first
                query(':enter', style({ opacity: 0, display: 'none' }), { optional: true }),
                // animate the leave page away
                query(':leave', [
                    animate('0.5s', style({ opacity: 0 })),
                    style({ display: 'none' })
                ], { optional: true }),
                // and now reveal the enter
                query(':enter', [
                    style({ display: 'block' }),
                    animate('0.5s', style({ opacity: 1 }))
                ], { optional: true }),
            ]),
        ])
    ]
})
export class AppComponent {
    @ViewChild(PreferenceDialogComponent) private _prefsDialog: PreferenceDialogComponent;

    constructor(private _stateService: PhotoStateService,
                private _navService: PhotoNavigationService) {
        this._stateService.showPreferencesEventEmitter.subscribe(
            (val: any) => this.showPreferencesDialog()
        );
    }

    showPreferencesDialog(): void {
        this._prefsDialog.show();
    }

    prepRouteState(outlet: any) {
        return outlet.activatedRouteData['animation'] || 'home';
    }
}
