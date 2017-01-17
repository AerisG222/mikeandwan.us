import { RouteMode } from './route-mode.model';

export class ModeRouteInfo {
    static MODE_RANDOM = 'random';
    static MODE_CATEGORY = 'category';
    static MODE_COMMENT = 'comment';
    static MODE_RATING = 'rating';

    static get PARAM_MODE(): string { return 'year'; };
    static get PARAM_YEAR(): string { return 'year'; };
    static get PARAM_COMMENT(): string { return 'comment'; };
    static get PARAM_RATING(): string { return 'rating'; };
    static get PARAM_TYPE(): string { return 'type'; };
    static get PARAM_ORDER(): string { return 'order'; };
    static get PARAM_CATEGORY(): string { return 'category'; };

    static RANDOM = new ModeRouteInfo(RouteMode.Random);
    static CATEGORY = new ModeRouteInfo(RouteMode.Category);
    static COMMENT = new ModeRouteInfo(RouteMode.Comment);
    static RATING = new ModeRouteInfo(RouteMode.Rating);

    static getMode(mode: string) {
        switch (mode) {
            case ModeRouteInfo.MODE_RANDOM:
                return ModeRouteInfo.RANDOM;
            case ModeRouteInfo.MODE_CATEGORY:
                return ModeRouteInfo.CATEGORY;
            case ModeRouteInfo.MODE_COMMENT:
                return ModeRouteInfo.COMMENT;
            case ModeRouteInfo.MODE_RATING:
                return ModeRouteInfo.RATING;
            default:
                throw new Error(`Invalid mode: ${mode}`);
        }
    }

    private constructor(public mode: RouteMode) {

    }
}
