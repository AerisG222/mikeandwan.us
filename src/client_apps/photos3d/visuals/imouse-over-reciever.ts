import { MouseWatcherEvent } from '../models/mouse-watcher-event';

export interface IMouseOverReceiver {
    onMouseOver(evt: MouseWatcherEvent);
    onMouseOut(evt: MouseWatcherEvent);
}
