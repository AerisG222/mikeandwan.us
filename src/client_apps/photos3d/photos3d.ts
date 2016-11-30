import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import 'rxjs/add/observable/fromEvent';
import 'rxjs/add/operator/throttleTime';

import { BackgroundController } from './controllers/background-controller';
import { CategoryListController } from './controllers/category-list-controller';
import { DataService } from './services/data-service';
import { FrustrumCalculator } from './services/frustrum-calculator';
import { IController } from './controllers/icontroller';
import { PhotoListController } from './controllers/photo-list-controller';
import { ScaleCalculator } from './services/scale-calculator';
import { StateService } from './services/state-service';
import { StatusBarController } from './controllers/status-bar-controller';
import { VisualContext } from './models/visual-context';

export class Photos3D {
    private _axisHelper: THREE.AxisHelper;
    private _bg: IController;
    private _status: IController;
    private _catList: CategoryListController;
    private _photoList: PhotoListController;
    private _mouseoverSubscription: Subscription;
    private _mouseclickSubscription: Subscription;
    private _resizeSubscription: Subscription;
    private _stateService: StateService;

    private _ctx = new VisualContext();
    private _clock = new THREE.Clock();
    private _dataService = new DataService();
    private _frustrumCalculator = new FrustrumCalculator();
    private _scaleCalculator = new ScaleCalculator();
    private _isPaused = false;
    private _photoListMode = false;

    run() {
        // ensure scrollbars do not appear
        document.getElementsByTagName('body')[0].style.overflow = 'hidden';

        this.prepareScene();

        this._resizeSubscription = Observable
            .fromEvent<UIEvent>(window, 'resize')
            .subscribe(evt => this.onResize(evt));

        this._mouseoverSubscription = Observable
            .fromEvent<MouseEvent>(document, 'mousemove')
            // .throttleTime(10)
            .subscribe(evt => this.onMouseMove(evt));

        this._mouseclickSubscription = Observable
            .fromEvent<MouseEvent>(document, 'click')
            .subscribe(evt => this.onMouseClick(evt));

        this._stateService.categorySelectedSubject.subscribe(x => { this.enterPhotoListMode(); });

        Mousetrap.bind('esc', e => { this.exitMode(); });
        Mousetrap.bind('space', e => { this.togglePause(); });
        Mousetrap.bind('right', e => { this.moveNext(); });
        Mousetrap.bind('left', e => { this.movePrev(); });
        Mousetrap.bind('b', e => { this.toggleBackground(); });
        Mousetrap.bind('x', e => { this.toggleAxisHelper(); });
        Mousetrap.bind('s', e => { this.stepBackward(); });
        Mousetrap.bind('w', e => { this.stepForward(); });
        Mousetrap.bind('a', e => { this.strafeLeft(); });
        Mousetrap.bind('d', e => { this.strafeRight(); });

        this.animate();
    }

    private enterPhotoListMode() {
        this._photoListMode = true;
    }

    private exitMode() {
        if (this._photoListMode) {
            this._catList.enableVisuals(true);
            this._photoList.enableVisuals(false);
            this._photoListMode = false;
        }
    }

    private moveNext() {
        if (this._photoListMode) {
            this._photoList.showNext();
        } else {
            this._catList.moveNextYear();
        }
    }

    private movePrev() {
        if (this._photoListMode) {
            this._photoList.showPrev();
        } else {
            this._catList.movePrevYear();
        }
    }

    private strafeLeft() {
        this._ctx.camera.position.x -= 25;
    }

    private strafeRight() {
        this._ctx.camera.position.x += 25;
    }

    private stepForward() {
        this._ctx.camera.position.z -= 25;
    }

    private stepBackward() {
        this._ctx.camera.position.z += 25;
    }

    private togglePause() {
        this._isPaused = !this._isPaused;
        this.animate();
    }

    private toggleBackground() {
        this._bg.enableVisuals(!this._bg.areVisualsEnabled);
    }

    private toggleAxisHelper() {
        if (this._axisHelper == null) {
            this._axisHelper = new THREE.AxisHelper(500);
            this._ctx.scene.add(this._axisHelper);
        } else {
            this._ctx.scene.remove(this._axisHelper);
            this._axisHelper = null;
        }
    }

    private onResize(evt: UIEvent) {
        this._ctx.renderer.setSize(this._ctx.width, this._ctx.height);
        this._ctx.camera.aspect = this._ctx.width / this._ctx.height;
        this._ctx.camera.updateProjectionMatrix();
    }

    // http://stemkoski.github.io/Three.js/Mouse-Over.html
    private onMouseMove(evt: MouseEvent) {
        this._stateService.publishMouseover(this.getIntersects(evt));
    }

    private onMouseClick(evt: MouseEvent) {
        this._stateService.publishMouseClick(this.getIntersects(evt));
    }

    private getIntersects(evt: MouseEvent): Array<THREE.Intersection> {
        let x = ( evt.clientX / window.innerWidth ) * 2 - 1;
        let y = - ( evt.clientY / window.innerHeight ) * 2 + 1;
        let vector = new THREE.Vector3(x, y, 0.5);

        vector.unproject(this._ctx.camera);

        let ray = new THREE.Raycaster(this._ctx.camera.position, vector.sub(this._ctx.camera.position).normalize());

        // create an array containing all objects in the scene with which the ray intersects, though
        // filter the list to just objects that care about this to optimize perf
        let intersects = ray
            .intersectObjects(this._ctx.scene.children, true)
            .filter(i => i.distance);

        return intersects;
    }

    private prepareScene() {
        this._ctx.scene = new THREE.Scene();

        this._ctx.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
        this._ctx.renderer.setSize(this._ctx.width, this._ctx.height);
        document.body.appendChild(this._ctx.renderer.domElement);

        this._ctx.camera = new THREE.PerspectiveCamera(45, this._ctx.width / this._ctx.height, 0.1, 2000);
        this._ctx.camera.position.set(0, 200, 1000);
        this._ctx.camera.lookAt(new THREE.Vector3(0, 200, 0));

        this._stateService = new StateService(this._ctx);

        this._status = new StatusBarController(this._stateService);
        this._status.init();

        this._ctx.ambient = new THREE.AmbientLight(0x404040);
        this._ctx.scene.add(this._ctx.ambient);

        this._ctx.sun = new THREE.DirectionalLight(0xffffff, 0.9);
        this._ctx.sun.position.set(-1, 1, 1);
        this._ctx.sun.castShadow = true;
        this._ctx.scene.add(this._ctx.sun);

        this._bg = new BackgroundController(this._stateService);
        this._bg.init();

        this._catList = new CategoryListController(this._dataService, this._stateService, this._frustrumCalculator);
        this._catList.init();

        this._photoList = new PhotoListController(this._dataService, this._stateService, this._frustrumCalculator, this._scaleCalculator);
        this._photoList.init();

        this.animate();
    }

    private animate() {
        if (this._isPaused) {
            return;
        }

        requestAnimationFrame(() => this.animate());

        this.render();

        this._ctx.renderer.render(this._ctx.scene, this._ctx.camera);
    }

    private render() {
        let delta = this._clock.getDelta();

        this._bg.render(delta);
        this._catList.render(delta);
        this._status.render(delta);

        TWEEN.update();
    }
}
