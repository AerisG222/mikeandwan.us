import * as THREE from 'three';

import { IDisposable } from '../models/idisposable';

// http://stackoverflow.com/questions/22565737/cleanup-threejs-scene-leak
export class DisposalService {
    dispose(obj: any) {
        if (obj == null) {
            return;
        }

        if (this.isIDisposable(obj)) {
            (obj as IDisposable).dispose();
        }

        if (obj instanceof THREE.Object3D) {
            for (let i = 0; i < obj.children.length; i++) {
                this.dispose(obj.children[i]);
            }
        }

        if (obj.geometry) {
            obj.geometry.dispose();
            obj.geometry = null;
        }

        if (obj.material) {
            if (obj.material.map) {
                obj.material.map.dispose();
                obj.material.map = null;
            }

            obj.material.dispose();
            obj.material = null;
        }

        obj = null;
    }

    private isIDisposable(arg: any): arg is IDisposable {
        return arg.dispose !== undefined;
    }
}
