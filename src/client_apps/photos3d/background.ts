export class Background {
    private scene: THREE.Scene;
    private size: string;
    private waterMesh: THREE.Mesh;
    private treeMesh: THREE.Mesh;
    private textureLoader = new THREE.TextureLoader();

    constructor(scene: THREE.Scene, size: string) {
        if(scene == null) {
            throw new Error("scene should not be null");
        }

        if(size == null) {
            throw new Error("size should not be null");
        }

        this.scene = scene;
        this.size = size;
    }

    init() {
        // water
        let waterPlane = new THREE.PlaneGeometry(2000, 621);
        this.waterMesh = new THREE.Mesh(waterPlane);
        this.waterMesh.scale.y = -1;
        this.waterMesh.position.y = -60;
        this.waterMesh.rotation.x = (Math.PI / 2) + .1;
        this.scene.add(this.waterMesh);

        this.updateWaterTexture();
        
        // trees
        let treePlane = new THREE.PlaneGeometry(2000, 506);
        this.treeMesh = new THREE.Mesh(treePlane);
        this.treeMesh.position.z = -280;
        this.treeMesh.position.y = 220;
        this.scene.add(this.treeMesh);
        
        this.updateTreeTexture();
    }
    
    setSize(size: string) {
        if(size == this.size) {
            return;
        }

        this.size = size;
        this.updateTreeTexture();
        this.updateWaterTexture();
    }

    private updateTreeTexture() {
        this.updateTexture(this.treeMesh, `/img/photos3d/${this.size}/trees.jpg`);
    }

    private updateWaterTexture() {
        this.updateTexture(this.waterMesh, `/img/photos3d/${this.size}/water.jpg`);
    }

    private updateTexture(mesh: THREE.Mesh, imgUrl: string) {
        this.textureLoader.load(imgUrl, texture => {
            mesh.material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });
        });
    }
}
