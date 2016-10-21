import { ICategory } from './icategory';
import { IPhoto } from './iphoto';
import { Promise } from 'es6-promise';

export class DataService {
    getCategories(): Promise<Array<ICategory>> {
        return window.fetch('/api/photos/getAllCategories3D', {
                method: 'get'
            }).then((response: Array<ICategory>) => {
                return response; 
            }).catch((err: Error) => {
                alert(`There was an error getting data: {err}`);
            });
    }

    getPhotos(categoryId: number): Promise<Array<IPhoto>> {
        return window.fetch(`/api/photos/getPhotos3D/{categoryId}`, {
                method: 'get'
            }).then((response: Array<IPhoto>) => {
                return response;
            }).catch((err: Error) => {
                alert("There was an error getting data: {err}");
            });
    }
}
