import { DataService } from '../services/data-service';
import { ICategory } from '../models/icategory';
import { IController } from './icontroller';
import { IPhoto } from '../models/iphoto';
import { PhotoVisual } from '../visuals/photo-visual';
import { StateService } from '../services/state-service';

export class PhotoListController implements IController {
    private _visualsEnabled = true;
    private _photos: Array<IPhoto> = [];

    private _idx = 0;
    private _activePhoto: PhotoVisual;

    constructor(private _dataService: DataService,
                private _stateService: StateService) {
        
    }

    get areVisualsEnabled(): boolean {
        return this._visualsEnabled;
    }

    init(): void {
        this._stateService.categorySelectedSubject.subscribe(cat => { this.loadPhotos(cat); } );
    }

    render(): void {

    }

    enableVisuals(areEnabled: boolean): void {

    }

    showNext(): void {
        if(this._idx >= this._photos.length) {
            return;
        }

        this._idx++;
        this.showPhoto();
    }

    showPrev(): void {
        if(this._idx <= 0) {
            return;
        }

        this._idx--;
        this.showPhoto();
    }

    private loadPhotos(category: ICategory): void {
        this._dataService
            .getPhotos(category.id)
            .then(photos => {
                this._photos = photos;
                this.showPhoto();
            });
    }

    private showPhoto(): void {
        let oldPhoto = this._activePhoto;
        let newPhoto = new PhotoVisual(this._photos[this._idx], this._stateService);
        
        newPhoto.init();

        if(oldPhoto != null) {
            // remove from view
            this._stateService.visualContext.scene.remove(oldPhoto);
        }

        this._stateService.visualContext.scene.add(newPhoto);

        this._activePhoto = newPhoto;
    }
}
