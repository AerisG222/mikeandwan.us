import { ICategory } from '../models/icategory';
import { IPhoto } from '../models/iphoto';

// TODO: fetch not currently understood by tsc, so we wrap window as any to avoid errors, which
//       makes it easier to spot real errors introduced in the code.  once tsc understands this,
//       remove the casts below.
export class DataService {
    reqOpts = { method: 'get', credentials: 'include' };

    getCategories(): Promise<Array<ICategory>> {
        return (<any>window).fetch('/api/photos/getAllCategories3D', this.reqOpts)
            .then(this.status)
            .then(this.json)
            .then(data => data)
            .catch((err: Error) => {
                alert(`There was an error getting data: {err}`);
            });
    }

    getPhotos(categoryId: number): Promise<Array<IPhoto>> {
        return (<any>window).fetch(`/api/photos/getPhotos3D/{categoryId}`, this.reqOpts)
            .then(this.status)
            .then(this.json)
            .then(data => data)
            .catch((err: Error) => {
                alert(`There was an error getting data: {err}`);
            });
    }

    // https://developers.google.com/web/updates/2015/03/introduction-to-fetch
    private status(response) {
        if(response.status >= 200 && response.status < 300) {
            return Promise.resolve(response);
        }
        else {
            return Promise.reject(new Error(response.statusText))
        }
    }

    private json(response) {
        return response.json();
    }
}
