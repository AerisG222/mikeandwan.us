import { ArgumentNullError } from '../models/argument-null-error';
import { DataService } from '../services/data-service';
import { FrustrumCalculator } from '../services/frustrum-calculator';
import { ICategory } from '../models/icategory';
import { IController } from './icontroller';
import { IPhoto } from '../models/iphoto';
import { PhotoBackgroundVisual } from '../visuals/photo-background-visual';
import { PhotoVisual } from '../visuals/photo-visual';
import { ScaleCalculator } from '../services/scale-calculator';
import { StateService } from '../services/state-service';

export class PhotoListController implements IController {
    private _photoZ: number;
    private _activePhoto: PhotoVisual;
    private _targetWidth: number;
    private _targetHeight: number;
    private _bg: PhotoBackgroundVisual;

    private _oldPhotos: Array<PhotoVisual> = [];
    private _visualsEnabled = true;
    private _photos: Array<IPhoto> = [];
    private _idx = 0;

    constructor(private _dataService: DataService,
                private _stateService: StateService,
                private _frustrumCalculator: FrustrumCalculator,
                private _scaleCalculator: ScaleCalculator) {
        if (_dataService == null) {
            throw new ArgumentNullError('_dataService');
        }

        if (_stateService == null) {
            throw new ArgumentNullError('_stateService');
        }

        if (_frustrumCalculator == null) {
            throw new ArgumentNullError('_frustrumCalculator');
        }

        this._photoZ = _frustrumCalculator.calculateZForFullFrame(this._stateService.visualContext.camera);
        let bounds = _frustrumCalculator.calculateBounds(this._stateService.visualContext.camera, this._photoZ);
        this._targetWidth = bounds.x;
        this._targetHeight = bounds.y;
    }

    get areVisualsEnabled(): boolean {
        return this._visualsEnabled;
    }

    init(): void {
        this._stateService.categorySelectedObservable.subscribe(cat => {
            this._idx = 0;
            this.loadPhotos(cat);
            this._visualsEnabled = true;
        } );
    }

    render(clockDelta: number, elapsed: number): void {
        if (this._activePhoto != null) {
            this._activePhoto.render(clockDelta, elapsed);
        }

        for (let i = 0; i < this._oldPhotos.length; i++) {
            let oldPhoto = this._oldPhotos[i];

            oldPhoto.render(clockDelta, elapsed);

            if (oldPhoto.isHidden) {
                this._stateService.visualContext.scene.remove(oldPhoto);
            }
        }

        for (let i = this._oldPhotos.length - 1; i >= 0; i--) {
            let oldPhoto = this._oldPhotos[i];

            if (oldPhoto.isHidden) {
                this._stateService.visualContext.scene.remove(oldPhoto);
                this._oldPhotos.splice(i, 1);
            }
        }
    }

    enableVisuals(areEnabled: boolean): void {
        if (!areEnabled && this.areVisualsEnabled) {
            this._stateService.visualContext.scene.remove(this._bg);
            this._stateService.visualContext.scene.remove(this._activePhoto);

            this._bg = null;
            this._activePhoto = null;
        }
    }

    showNext(): void {
        if (this._idx >= this._photos.length - 1) {
            return;
        }

        this._idx++;
        this.showPhoto();
    }

    showPrev(): void {
        if (this._idx <= 0) {
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
        this.ensureBackground();

        if (this._activePhoto != null) {
            this._activePhoto.hide();
            this._oldPhotos.push(this._activePhoto);
        }

        let newPhoto = new PhotoVisual(this._photos[this._idx],
                                       this._stateService,
                                       this._scaleCalculator,
                                       this._targetHeight,
                                       this._targetWidth,
                                       this._photoZ);

        newPhoto.init();

        this._stateService.visualContext.scene.add(newPhoto);

        this._activePhoto = newPhoto;
    }

    private ensureBackground(): void {
        if (this._bg == null) {
            this._bg = new PhotoBackgroundVisual(this._stateService, this._frustrumCalculator, this._photoZ - 0.1);
            this._bg.init();

            this._stateService.visualContext.scene.add(this._bg);
        }
    }
}
