import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';

import { DisposalService } from '../services/disposal-service';
import { IDisposable } from '../models/idisposable';
import { IMouseClickReceiver } from './imouse-click-reciever';
import { IMouseOverReceiver } from './imouse-over-reciever';
import { IVisual } from './ivisual';
import { MouseWatcherEvent } from '../models/mouse-watcher-event';

export class ArrowVisual extends THREE.Object3D implements IDisposable, IMouseClickReceiver, IMouseOverReceiver, IVisual {
    static readonly SPEED_OFF = 0.02;
    static readonly SPEED_ON = 0.1;

    private _clickSubject = new Subject();
    private _isDisposed = false;
    private _mesh: THREE.Mesh;
    private _rotateSpeed = ArrowVisual.SPEED_OFF;

    constructor(private _disposalService: DisposalService) {
        super();
    }

    get width(): number {
        if (this._mesh == null) {
            return null;
        }

        let box = new THREE.Box3();

        box.setFromObject(this._mesh);

        return Math.max(box.max.x, box.max.y);
    }

    get clickObservable(): Observable<null> {
        return this._clickSubject.asObservable();
    }

    init(): void {
        this.createArrow();
    }

    render(clockDelta: number, elapsed: number): void {
        if (this._mesh != null) {
            this._mesh.rotation.x += this._rotateSpeed;
        }
    }

    dispose(): void {
        if(this._isDisposed) {
            return;
        }

        this.remove(this._mesh);
        this._disposalService.dispose(this._mesh);
        this._mesh = null;
    }

    onMouseClick(evt: MouseWatcherEvent): void {
        this._clickSubject.next();
    }

    onMouseOver(evt: MouseWatcherEvent): void {
        this._rotateSpeed = ArrowVisual.SPEED_ON;
    }

    onMouseOut(evt: MouseWatcherEvent): void {
        this._rotateSpeed = ArrowVisual.SPEED_OFF;
    }

    private createArrow(): void {
        let geometry = new THREE.ConeGeometry(6, 18, 5);
        let material = new THREE.MeshPhongMaterial({color: 0xffff00});

        this._mesh = new THREE.Mesh(geometry, material);
        this._mesh.rotateZ(-Math.PI / 2);

        this.add(this._mesh);
    }
}