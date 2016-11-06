import { ICategory } from './icategory';

export class CategoryObject3D extends THREE.Object3D {
    private static EDGE_LENGTH = 36;
    private static BORDER_WIDTH = 3;
    private static loader = new THREE.TextureLoader();

    private backgroundMesh: THREE.Mesh = null;
    private imageMesh: THREE.Mesh = null;

    constructor(private category: ICategory,
                private endPosition: THREE.Vector3,
                public color: number) {
        super();
    }

    init() {
        CategoryObject3D.loader.load(this.category.teaserImage.path, texture => {
            this.createObject(texture);
        });
    }

    private createObject(texture: THREE.Texture) {
        this.createBackground();
        this.createImage(texture);

        this.add(this.backgroundMesh);
        this.add(this.imageMesh);
    }

    private createBackground() {
        let len = CategoryObject3D.EDGE_LENGTH;
        let geometry = new THREE.BoxGeometry(len, len, 1);
        let material = new THREE.MeshLambertMaterial({ color: this.color });

        this.backgroundMesh = new THREE.Mesh(geometry, material);
    }

    private createImage(texture: THREE.Texture) {
        let len = CategoryObject3D.EDGE_LENGTH - CategoryObject3D.BORDER_WIDTH;
        let geometry = new THREE.BoxGeometry(len, len, 3);
        let material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });

        this.imageMesh = new THREE.Mesh(geometry, material);
    }
}
