import { ICategory } from './icategory';

export class CategoryObject3D extends THREE.Object3D {
    mesh: THREE.Mesh = null;

    constructor(category: ICategory,
                endPosition: THREE.Vector3,
                public color: number) {
        super();
    }

    init() {
        let geometry = new THREE.BoxGeometry(64, 64, 1);
        let material = new THREE.MeshLambertMaterial({
            color: this.color
        });

        this.mesh = new THREE.Mesh(geometry, material);

        this.add(this.mesh);
    }
}
