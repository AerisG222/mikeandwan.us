import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import * as THREE from 'three';

import { IDisposable } from './idisposable';
import { IMouseClickReceiver } from '../visuals/imouse-click-reciever';
import { IMouseOverReceiver } from '../visuals/imouse-over-reciever';
import { MouseWatcherEvent } from './mouse-watcher-event';
import { VisualContext } from './visual-context';

export class MouseWatcher implements IDisposable {
    private _mouseoverSubscription: Subscription;
    private _mouseclickSubscription: Subscription;
    private _currMouseOverReceiver: IMouseOverReceiver;

    constructor(private _ctx: VisualContext) {
        this.startWatching();
    }

    get ignoreMouseEvents(): boolean {
        return this._mouseoverSubscription != null;
    }

    set ignoreMouseEvents(ignore: boolean) {
        if (ignore) {
            this.stopWatching();
        } else {
            this.startWatching();
        }
    }

    dispose() {
        this.stopWatching();
    }

    private startWatching(): void {
        if (this._mouseoverSubscription == null) {
            this._mouseoverSubscription = Observable
                .fromEvent<MouseEvent>(document, 'mousemove')
                .subscribe(evt => this.onMouseMove(evt));
        }

        if (this._mouseclickSubscription == null) {
            this._mouseclickSubscription = Observable
                .fromEvent<MouseEvent>(document, 'click')
                .subscribe(evt => this.onMouseClick(evt));
        }
    }

    private stopWatching(): void {
        if (this._mouseoverSubscription != null) {
            this._mouseoverSubscription.unsubscribe();
            this._mouseoverSubscription = null;
        }

        if (this._mouseclickSubscription != null) {
            this._mouseclickSubscription.unsubscribe();
            this._mouseclickSubscription = null;
        }
    }

    private onMouseMove(evt: MouseEvent): void {
        let handled = false;
        let intersects = this.getIntersects(evt);

        for (let i = 0; i < intersects.length; i++) {
            if (intersects[i].object.parent != null && this.isIMouseOverReceiver(intersects[i].object.parent)) {
                let newReceiver = (intersects[i].object.parent as any as IMouseOverReceiver);

                // mouseout always called before mouseover
                if (newReceiver !== this._currMouseOverReceiver) {
                    if (this._currMouseOverReceiver != null) {
                        this._currMouseOverReceiver.onMouseOut(new MouseWatcherEvent(this, evt));
                        this._currMouseOverReceiver = newReceiver;
                    }

                    this._currMouseOverReceiver = newReceiver;
                }

                newReceiver.onMouseOver(new MouseWatcherEvent(this, evt));

                handled = true;
            }
        }

        if (!handled && this._currMouseOverReceiver != null) {
            this._currMouseOverReceiver.onMouseOut(new MouseWatcherEvent(this, evt));
            this._currMouseOverReceiver = null;
        }
    }

    private onMouseClick(evt: MouseEvent): void {
        let intersects = this.getIntersects(evt);

        for (let i = 0; i < intersects.length; i++) {
            if (intersects[i].object.parent != null && this.isIMouseClickReceiver(intersects[i].object.parent)) {
                (intersects[i].object.parent as any as IMouseClickReceiver).onMouseClick(new MouseWatcherEvent(this, evt));
            }
        }
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
            .filter(i => i.object.parent != null); // we currently expect events to be targeted to our custom classes that extend Object3D

        return intersects;
    }

    // http://aliolicode.com/2016/04/23/type-checking-typescript/
    private isIMouseClickReceiver(arg: any): arg is IMouseClickReceiver {
        return arg.onMouseClick !== undefined;
    }

    private isIMouseOverReceiver(arg: any): arg is IMouseOverReceiver {
        return arg.onMouseOver !== undefined;
    }
}
