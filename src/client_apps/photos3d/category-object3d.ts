import { ICategory } from './icategory';

export class CategoryObject3D extends THREE.Object3D {
    mesh: THREE.Mesh = null;

    constructor(private category: ICategory,
                private endPosition: THREE.Vector3,
                public color: number) {
        super();
    }

    init() {
        let loader = new THREE.TextureLoader();
        let geometry = new THREE.BoxGeometry(32, 32, 1);
        let material = new THREE.MeshLambertMaterial({
            color: this.color
        });

        this.mesh = new THREE.Mesh(geometry, material);

        loader.load(this.category.teaserImage.path, texture => {
            this.mesh.material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });
        });

        this.add(this.mesh);
    }
}
