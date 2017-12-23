import { Injectable, EventEmitter } from '@angular/core';

import { Config } from './config.model';
import { LocalStorageService } from '../shared/local-storage.service';

@Injectable()
export class VideoStateService {
    private static KEY_CONFIG = 'videoConfig';
    configUpdatedEventEmitter: EventEmitter<Config> = new EventEmitter<Config>();
    config: Config;
    lastCategoryIndex: number;

    constructor(private _storageService: LocalStorageService) {
        this.config = <Config>this._storageService.get(VideoStateService.KEY_CONFIG, new Config());
        this.lastCategoryIndex = 0;
    }

    saveConfig(): void {
        this._storageService.set(VideoStateService.KEY_CONFIG, this.config);
        this.configUpdatedEventEmitter.next(this.config);
    }
}
