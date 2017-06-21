import { Component, ViewChild } from '@angular/core';
import { animate, group, query, stagger, sequence, style, trigger, transition, useAnimation } from '@angular/animations';

import { fadeAnimation } from '../ng_maw/shared/animation';

import { PreferenceDialogComponent } from './preference-dialog/preference-dialog.component';
import { PhotoStateService } from './shared/photo-state.service';
import { PhotoNavigationService } from './shared/photo-navigation.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.css' ],
    animations: [
        trigger('routeAnimation', [
            // no need to animate anything on load
            transition(':enter', []),

            // home to category list
            transition('* <=> *', [
                query(':enter', style({ opacity: 0 }) ),

                // animate the leave page away
                query(':leave', [
                    animate('1s', style({ opacity: 0 })),
                    style({ display: 'none' })
                ]),

                // make sure the new page is hidden first
                query(':enter', animate('2s', style({ opacity: 1 })) )
            ])
        ])
    ]
})
export class AppComponent {
    @ViewChild(PreferenceDialogComponent) private _prefsDialog: PreferenceDialogComponent;
    public routeAnimationState: string;

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
        const anim = outlet.activatedRouteData['animation'] || 'home';

        if (anim !== this.routeAnimationState) {
            this.routeAnimationState = anim;
            console.log("router animation state: " + anim);
        }

        return anim;
    }
}
