import { NavEventType } from './nav-event-type';
import { NavItem } from './nav-item';

export class NavEvent {
    constructor(public navEventType: NavEventType,
                public navItem: NavItem) {

    }
}
