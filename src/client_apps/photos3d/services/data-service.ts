import { AuthService } from './auth-service';
import { ICategory } from '../models/icategory';
import { IPhoto } from '../models/iphoto';

// TODO: fetch not currently understood by tsc, so we wrap window as any to avoid errors, which
//       makes it easier to spot real errors introduced in the code.  once tsc understands this,
//       remove the casts below.
export class DataService {
    readonly API_BASE_URL = 'https://localhost:5011';

    constructor(private _authService) {

    }

    getCategories(): Promise<Array<ICategory>> {
        const url = `${this.API_BASE_URL}/photos/getAllCategories3D`;

        return (<any>window).fetch(url, this.getRequestOptions())
            .then(this.status)
            .then(this.json)
            .then(data => data)
            .catch((err: Error) => {
                alert(`There was an error getting data: ${err}`);
            });
    }

    getPhotos(categoryId: number): Promise<Array<IPhoto>> {
        const url = `${this.API_BASE_URL}/photos/getPhotos3D/${categoryId}`;

        return (<any>window).fetch(url, this.getRequestOptions())
            .then(this.status)
            .then(this.json)
            .then(data => data)
            .catch((err: Error) => {
                alert(`There was an error getting data: ${err}`);
            });
    }

    private getRequestOptions() {
        const headers = new Headers();

        headers.set('Authorization', `Bearer ${this._authService.getToken()}`);

        return {
            method: 'get',
            credentials: 'include',
            headers: headers
        };
    }

    // https://developers.google.com/web/updates/2015/03/introduction-to-fetch
    private status(response) {
        if (response.status >= 200 && response.status < 300) {
            return Promise.resolve(response);
        } else {
            return Promise.reject(new Error(response.statusText));
        }
    }

    private json(response) {
        return response.json();
    }
}
