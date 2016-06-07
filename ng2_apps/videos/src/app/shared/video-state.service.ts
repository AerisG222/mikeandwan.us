import { Injectable, EventEmitter } from '@angular/core';

import { LocalStorageService } from '../../../../ng_maw/src/app/shared/index';

import { Config } from './index';

@Injectable()
export class VideoStateService {
    private static KEY_CONFIG : string = 'videoConfig';
	  showPreferencesEventEmitter : EventEmitter<any> = new EventEmitter<any>();
    configUpdatedEventEmitter : EventEmitter<Config> = new EventEmitter<Config>();
    config : Config;
    lastCategoryIndex : number;
    
	constructor (private _storageService : LocalStorageService) {
        this.config = <Config>this._storageService.get(VideoStateService.KEY_CONFIG, new Config());
        this.lastCategoryIndex = 0;
	}
    
    saveConfig() : void {
        this._storageService.set(VideoStateService.KEY_CONFIG, this.config);
        this.configUpdatedEventEmitter.next(this.config);
    }
	
	showPreferencesDialog() : void {
		  this.showPreferencesEventEmitter.next(null);
	}
}
