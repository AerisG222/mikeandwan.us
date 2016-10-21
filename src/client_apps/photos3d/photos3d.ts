import { DataService } from './data-service';

export class Photos3D {
    scene: THREE.Scene;
    camera: THREE.PerspectiveCamera;
    renderer: THREE.Renderer;
    ambientLight: THREE.AmbientLight;
    cube: THREE.Mesh;
    dataService = new DataService();
    isPaused = false;

    run() {
        this.prepareScene();
        this.loadCategories();
        this.render();
    }

    togglePause() {
        this.isPaused = !this.isPaused;
        this.render();
    }

    private loadCategories() {
        this.dataService.getCategories().then(categories => {
            for(let i = 0; i < categories.length; i++) {
                let cat = categories[i];
                console.log(cat.id);
            }
        });
    }

    private prepareScene() {
        this.scene = new THREE.Scene();

        // fog
        this.scene.fog = new THREE.Fog(0x9999ff, 400, 1000);

        // renderer
        this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        document.body.appendChild(this.renderer.domElement);

        // camera
        this.camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);
        this.camera.position.set(0, 150, 400);
        this.camera.lookAt(this.scene.position);

        // ambient light
        this.ambientLight = new THREE.AmbientLight(0x404040);
        this.scene.add(this.ambientLight);

        // directional light
        let directionalLight = new THREE.DirectionalLight(0xffffff, 0.9);
        directionalLight.position.set(-1, 1, 1);
        directionalLight.castShadow = true;
        this.scene.add(directionalLight);

        // cube
        let textureLoader = new THREE.TextureLoader();
        textureLoader.load('/images/2013/alyssas_first_snowman/xs/DSC_9960.jpg', texture => {
            let geometry = new THREE.BoxGeometry(50, 50, 50);
            let material = new THREE.MeshPhongMaterial({ color: 0xffffff, map: texture });
            this.cube = new THREE.Mesh(geometry, material);
            this.cube.position.set(0, 70, 180);
            this.scene.add(this.cube);
        });

        // floor
        textureLoader.load('/img/photos3d/floor.jpg', texture => {
            let floorPlane = new THREE.PlaneGeometry(1000, 1000);
            texture.wrapS = THREE.RepeatWrapping;
            texture.wrapT = THREE.RepeatWrapping;
            texture.repeat.set(9, 9);
            let floorMaterial = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide }); //
            let floor = new THREE.Mesh(floorPlane, floorMaterial);
            floor.position.y = -30;
            floor.rotation.x = (Math.PI / 2) - .3;
            this.scene.add(floor);
        });

        this.render();
    }

    private render() {
        if(this.isPaused) {
            return;
        }

        requestAnimationFrame(() => this.render());

        if(this.cube != null) {
            this.cube.rotateX(.01);
            this.cube.rotateY(.01);
        }

        this.renderer.render(this.scene, this.camera);
    }
}
