import { RouteMode } from './route-mode.model';

export class ModeRouteInfo {
    static get PARAM_YEAR(): string { return 'year'; };
    static get PARAM_COMMENT(): string { return 'comment'; };
    static get PARAM_RATING(): string { return 'rating'; };
    static get PARAM_TYPE(): string { return 'type'; };
    static get PARAM_ORDER(): string { return 'order'; };
    static get PARAM_CATEGORY(): string { return 'category'; };

    constructor(public mode: RouteMode) {

    }
}
