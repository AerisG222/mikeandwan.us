import { Injectable, EventEmitter } from '@angular/core';

import { LocalStorageService } from './local-storage.service';
import { ResponsiveService } from './responsive.service';
import { Config } from '../models/config.model';

@Injectable()
export class PhotoStateService {
    private static KEY_CONFIG = 'photoConfig';
    showPreferencesEventEmitter: EventEmitter<any> = new EventEmitter<any>();
    configUpdatedEventEmitter: EventEmitter<Config> = new EventEmitter<Config>();
    toggleMapsEventEmitter: EventEmitter<boolean> = new EventEmitter<boolean>();
    config: Config;
    lastCategoryIndex: number;

    constructor(private _storageService: LocalStorageService,
                private _responsiveService: ResponsiveService) {
        this.config = <Config>_storageService.get(PhotoStateService.KEY_CONFIG, new Config());
        this.lastCategoryIndex = 0;
    }

    showPreferencesDialog(): void {
        this.showPreferencesEventEmitter.next(null);
    }

    toggleMapsView(showMaps: boolean): void {
        this.toggleMapsEventEmitter.next(showMaps);
    }

    saveConfig(): void {
        this._storageService.set(PhotoStateService.KEY_CONFIG, this.config);
        this.configUpdatedEventEmitter.next(this.config);
    }
}
