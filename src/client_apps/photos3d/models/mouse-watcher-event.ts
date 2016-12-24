import { ArgumentNullError } from './argument-null-error';
import { MouseWatcher } from './mouse-watcher';

export class MouseWatcherEvent {
    constructor(private _watcher: MouseWatcher,
                private _event: MouseEvent) {
        if (_watcher == null) {
            throw new ArgumentNullError('_watcher');
        }

        if (_event == null) {
            throw new ArgumentNullError('_event');
        }
    }

    get watcher(): MouseWatcher {
        return this._watcher;
    }

    get event(): MouseEvent {
        return this._event;
    }
}
