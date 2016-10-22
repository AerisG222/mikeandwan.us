import { ICategory } from './icategory';
import { IPhoto } from './iphoto';

export class DataService {
    reqOpts = { method: 'get', credentials: 'include' };

    getCategories(): Promise<Array<ICategory>> {
        return window.fetch('/api/photos/getAllCategories3D', this.reqOpts)
            .then(this.status)
            .then(this.json)
            .then(data => data)
            .catch((err: Error) => {
                alert(`There was an error getting data: {err}`);
            });
    }

    getPhotos(categoryId: number): Promise<Array<IPhoto>> {
        return window.fetch(`/api/photos/getPhotos3D/{categoryId}`, this.reqOpts)
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
