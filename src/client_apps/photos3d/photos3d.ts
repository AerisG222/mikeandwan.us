import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import 'rxjs/add/observable/fromEvent';
import 'rxjs/add/operator/throttleTime';

import { AxisHelper, Clock, Scene, WebGLRenderer, PerspectiveCamera, Vector3, AmbientLight, DirectionalLight } from 'three';

import * as TWEEN from 'tween.js';

import * as Mousetrap from 'mousetrap';

import { BackgroundController } from './controllers/background-controller';
import { CategoryListController } from './controllers/category-list-controller';
import { DataService } from './services/data-service';
import { DisposalService } from './services/disposal-service';
import { FrustrumCalculator } from './services/frustrum-calculator';
import { HelpController } from './controllers/help-controller';
import { IController } from './controllers/icontroller';
import { MouseWatcher } from './models/mouse-watcher';
import { PhotoListController } from './controllers/photo-list-controller';
import { PointLightController } from './controllers/point-light-controller';
import { ScaleCalculator } from './services/scale-calculator';
import { StateService } from './services/state-service';
import { StatusBarController } from './controllers/status-bar-controller';
import { VisualContext } from './models/visual-context';

export class Photos3D {
    private _axisHelper: AxisHelper;
    private _bg: IController;
    private _blurSubscription: Subscription;
    private _categorySelectedSubscription: Subscription;
    private _catList: CategoryListController;
    private _clock = new Clock();
    private _dataService = new DataService();
    private _dialogSubscription: Subscription;
    private _disposalService = new DisposalService();
    private _focusSubscription: Subscription;
    private _frustrumCalculator = new FrustrumCalculator();
    private _help: IController;
    private _isPaused = false;
    private _mouseWatcher: MouseWatcher;
    private _photoList: PhotoListController;
    private _photoListMode = false;
    private _pointLights: IController;
    private _resizeSubscription: Subscription;
    private _scaleCalculator = new ScaleCalculator();
    private _status: IController;
    private _unloadSubscription: Subscription;

    private _ctx = new VisualContext(this._disposalService);
    private _stateService = new StateService(this._ctx);

    run() {
        // ensure scrollbars do not appear
        document.getElementsByTagName('body')[0].style.overflow = 'hidden';

        this._mouseWatcher = new MouseWatcher(this._ctx);

        this.prepareScene();

        this._status = new StatusBarController(this._stateService);
        this._status.init();

        this._help = new HelpController(this._stateService);
        this._help.init();

        this._resizeSubscription = Observable
            .fromEvent<UIEvent>(window, 'resize')
            .subscribe(evt => this.onResize(evt));

        this._unloadSubscription = Observable
            .fromEvent<Event>(window, 'unload')
            .subscribe(evt => this.unload(evt));

        this._blurSubscription = Observable
            .fromEvent<Event>(window, 'blur')
            .subscribe(evt => this.onBlur(evt));

        this._focusSubscription = Observable
            .fromEvent<Event>(window, 'focus')
            .subscribe(evt => this.onFocus(evt));

        this._categorySelectedSubscription = this._stateService.categorySelectedObservable.subscribe(x => { this.enterPhotoListMode(); });
        this._dialogSubscription = this._stateService.dialogDisplayedObservable.subscribe(x => { this.onDialogDisplayed(x); });

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
        Mousetrap.bind('h', e => { this.toggleStatus(); });
        Mousetrap.bind('p', e => { this.togglePointLights(); });
        Mousetrap.bind('?', e => { this.toggleHelp(); });

        this.animate();
    }

    private onBlur(evt: Event) {
        this.pause(true);
    }

    private onFocus(evt: Event) {
        this.pause(false);
    }

    private onDialogDisplayed(isDisplayed: boolean) {
        this._mouseWatcher.ignoreMouseEvents = isDisplayed;
    }

    private togglePointLights() {
        this._pointLights.enableVisuals(!this._pointLights.areVisualsEnabled);
    }

    private toggleHelp() {
        this._help.enableVisuals(!this._help.areVisualsEnabled);
    }

    private toggleStatus() {
        this._status.enableVisuals(!this._status.areVisualsEnabled);
    }

    private enterPhotoListMode() {
        this._photoListMode = true;
    }

    private exitMode() {
        if (this._photoListMode) {
            this._catList.enableVisuals(true);
            this._photoList.enableVisuals(false);
            this._photoListMode = false;
            this._mouseWatcher.ignoreMouseEvents = false;
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
        this.pause(!this._isPaused);
    }

    private toggleBackground() {
        this._bg.enableVisuals(!this._bg.areVisualsEnabled);
    }

    private toggleAxisHelper() {
        if (this._axisHelper == null) {
            this._axisHelper = new AxisHelper(500);
            this._ctx.scene.add(this._axisHelper);
        } else {
            this.removeAxisHelper();
        }
    }

    private removeAxisHelper() {
        if (this._axisHelper == null) {
            return;
        }

        this._ctx.scene.remove(this._axisHelper);
        this._disposalService.dispose(this._axisHelper);
    }

    private onResize(evt: UIEvent) {
        this._ctx.renderer.setSize(this._ctx.width, this._ctx.height);
        this._ctx.camera.aspect = this._ctx.width / this._ctx.height;
        this._ctx.camera.updateProjectionMatrix();
    }

    private pause(doPause: boolean) {
        this._isPaused = doPause;

        if (this._isPaused) {
            this._clock.stop();
        } else {
            this._clock.start();
            this.animate();
        }

        this._stateService.publishPaused(this._isPaused);
    }

    private prepareScene() {
        this._ctx.scene = new Scene();

        this._ctx.renderer = new WebGLRenderer({ antialias: true, alpha: true });
        this._ctx.renderer.setSize(this._ctx.width, this._ctx.height);
        document.body.appendChild(this._ctx.renderer.domElement);

        this._ctx.camera = new PerspectiveCamera(45, this._ctx.width / this._ctx.height, 0.1, 2000);
        this._ctx.camera.position.set(0, 200, 1000);
        this._ctx.camera.lookAt(new Vector3(0, 200, 0));

        this._ctx.ambient = new AmbientLight(0x404040);
        this._ctx.scene.add(this._ctx.ambient);

        this._ctx.sun = new DirectionalLight(0xffffff, 0.9);
        this._ctx.sun.position.set(-1, 1, 1);
        this._ctx.sun.castShadow = true;
        this._ctx.scene.add(this._ctx.sun);

        this._bg = new BackgroundController(this._stateService, this._disposalService);
        this._bg.init();

        this._pointLights = new PointLightController(this._stateService, this._frustrumCalculator, this._disposalService);
        this._pointLights.init();

        this._catList = new CategoryListController(this._dataService, this._stateService, this._frustrumCalculator, this._disposalService);
        this._catList.init();

        this._photoList = new PhotoListController(this._dataService, this._stateService, this._frustrumCalculator, this._scaleCalculator,
                                                  this._disposalService, this._mouseWatcher);
        this._photoList.init();

        this.animate();
    }

    private animate() {
        if (this._isPaused) {
            return;
        }

        this.render();

        this._ctx.renderer.render(this._ctx.scene, this._ctx.camera);

        requestAnimationFrame(() => this.animate());
    }

    private render() {
        let clockDelta = this._clock.getDelta();
        let elapsed = this._clock.getElapsedTime();

        this._bg.render(clockDelta, elapsed);
        this._catList.render(clockDelta, elapsed);
        this._photoList.render(clockDelta, elapsed);
        this._pointLights.render(clockDelta, elapsed);

        TWEEN.update();
    }

    private unload(evt: Event) {
        this.pause(true);

        this._blurSubscription.unsubscribe();
        this._blurSubscription = null;

        this._categorySelectedSubscription.unsubscribe();
        this._categorySelectedSubscription = null;

        this._dialogSubscription.unsubscribe();
        this._dialogSubscription = null;

        this._focusSubscription.unsubscribe();
        this._focusSubscription = null;

        this._resizeSubscription.unsubscribe();
        this._resizeSubscription = null;

        this._unloadSubscription.unsubscribe();
        this._unloadSubscription = null;

        Mousetrap.unbind('esc');
        Mousetrap.unbind('space');
        Mousetrap.unbind('right');
        Mousetrap.unbind('left');
        Mousetrap.unbind('b');
        Mousetrap.unbind('x');
        Mousetrap.unbind('s');
        Mousetrap.unbind('w');
        Mousetrap.unbind('a');
        Mousetrap.unbind('d');
        Mousetrap.unbind('h');
        Mousetrap.unbind('p');
        Mousetrap.unbind('?');

        TWEEN.removeAll();

        this.removeAxisHelper();

        this._disposalService.dispose(this._bg);
        this._disposalService.dispose(this._catList);
        this._disposalService.dispose(this._mouseWatcher);
        this._disposalService.dispose(this._photoList);
        this._disposalService.dispose(this._pointLights);
        this._disposalService.dispose(this._ctx);
    }
}
