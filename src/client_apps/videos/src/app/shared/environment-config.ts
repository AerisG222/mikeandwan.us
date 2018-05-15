export class EnvironmentConfig {
    readonly apiUrl: string;
    readonly authUrl: string;
    readonly wwwUrl: string;

    // do this dynamically rather than at build time to allow for testing the production
    // build in a non-production environment
    constructor() {
        if (window.location.hostname.indexOf('dev') > 0) {
            this.apiUrl = 'https://apidev.mikeandwan.us:5011';
            this.authUrl = 'https://authdev.mikeandwan.us:5001';
            this.wwwUrl = 'https://wwwdev.mikeandwan.us:5021';
        } else {
            this.apiUrl = 'https://api.mikeandwan.us';
            this.authUrl = 'https://auth.mikeandwan.us';
            this.wwwUrl = 'https://www.mikeandwan.us';
        }
    }
}
