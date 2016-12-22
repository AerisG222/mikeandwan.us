import { MouseWatcherEvent } from '../models/mouse-watcher-event';

export interface IMouseClickReceiver {
    onMouseClick(evt: MouseWatcherEvent);
}
