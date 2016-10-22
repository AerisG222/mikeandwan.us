import { DataService } from './data-service';

export class Photos3D {
    scene: THREE.Scene;
    camera: THREE.PerspectiveCamera;
    renderer: THREE.Renderer;
    ambientLight: THREE.AmbientLight;
    dataService = new DataService();
    isPaused = false;

    run() {
        // ensure scrollbars do not appear
        document.getElementsByTagName('body')[0].style.overflow = "hidden";

        this.prepareScene();
        this.loadCategories();
        this.render();
    }

    togglePause() {
        this.isPaused = !this.isPaused;
        this.render();
    }

    private loadCategories() {
        this.dataService
            .getCategories()
            .then(categories => {
                for(let i = 0; i < categories.length; i++) {
                    let cat = categories[i];
                }
            });
    }

    private prepareScene() {
        this.scene = new THREE.Scene();

        // fog
        //this.scene.fog = new THREE.Fog(0x9999ff, 400, 1000);

        // renderer
        this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        document.body.appendChild(this.renderer.domElement);

        // camera
        this.camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);
        this.camera.position.set(0, 100, 600);
        this.camera.lookAt(this.scene.position);

        // ambient light
        this.ambientLight = new THREE.AmbientLight(0x404040);
        this.scene.add(this.ambientLight);

        // directional light
        let directionalLight = new THREE.DirectionalLight(0xffffff, 0.9);
        directionalLight.position.set(-1, 1, 1);
        directionalLight.castShadow = true;
        this.scene.add(directionalLight);

        // water
        let textureLoader = new THREE.TextureLoader();
        textureLoader.load('/img/photos3d/md/water.jpg', texture => {
            let waterPlane = new THREE.PlaneGeometry(2000, 621);
            let waterMaterial = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });
            let water = new THREE.Mesh(waterPlane, waterMaterial);
            water.scale.y = -1;
            water.position.y = -60;
            water.rotation.x = (Math.PI / 2) + .1;
            this.scene.add(water);

            this.render();
        });

        // trees
        textureLoader.load('/img/photos3d/md/trees.jpg', texture => {
            let treePlane = new THREE.PlaneGeometry(2000, 506);
            let treeMaterial = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });
            let trees = new THREE.Mesh(treePlane, treeMaterial);
            //trees.scale.y = -1;
            trees.position.z = -280;
            trees.position.y = 220;
            this.scene.add(trees);

            this.render();
        });

        this.render();
    }

    private render() {
        if(this.isPaused) {
            return;
        }

        //requestAnimationFrame(() => this.render());

        // animate

        this.renderer.render(this.scene, this.camera);
    }
}
