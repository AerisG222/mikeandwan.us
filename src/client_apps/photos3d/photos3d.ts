import { DataService } from './data-service';
import { Background } from './background';

export class Photos3D {
    camera: THREE.PerspectiveCamera;
    renderer: THREE.WebGLRenderer;
    ambientLight: THREE.AmbientLight;
    background: Background;
    height: number;
    width: number;
    sizeCode: string;

    clock = new THREE.Clock();
    dataService = new DataService();
    scene = new THREE.Scene();
    isPaused = false;

    run() {
        // ensure scrollbars do not appear
        document.getElementsByTagName('body')[0].style.overflow = "hidden";
        window.addEventListener("resize", () => this.onResize(), false);

        this.onResize();
        this.prepareScene();
        this.loadCategories();
        this.animate();
    }

    togglePause() {
        this.isPaused = !this.isPaused;
        this.animate();
    }

    private onResize() {
        this.width = window.innerWidth;
        this.height = window.innerHeight;

        if(this.width < 2200) {
            this.sizeCode = 'md';
        }
        else {
            this.sizeCode = 'lg';
        }

        // adjust view
        if(this.renderer != null) {
            this.renderer.setSize(this.width, this.height);
            this.camera.aspect = this.width / this.height;
        }
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
        // renderer
        this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
        this.renderer.setSize(this.width, this.height);
        document.body.appendChild(this.renderer.domElement);

        // camera
        this.camera = new THREE.PerspectiveCamera(75, this.width / this.height, 0.1, 1600);
        this.camera.position.set(0, 50, 800);
        this.camera.lookAt(new THREE.Vector3(0, 50, 0));

        // background
        this.background = new Background(this.scene, this.sizeCode);
        this.background.init();

        // ambient light
        this.ambientLight = new THREE.AmbientLight(0x404040);
        this.scene.add(this.ambientLight);

        // directional light
        let directionalLight = new THREE.DirectionalLight(0xffffff, 0.9);
        directionalLight.position.set(-1, 1, 1);
        directionalLight.castShadow = true;
        this.scene.add(directionalLight);

        this.animate();
    }

    private animate() {
        if(this.isPaused) {
            return;
        }

        requestAnimationFrame(() => this.animate());

        this.render();

        this.renderer.render(this.scene, this.camera);
    }

    private render() {
        let delta = this.clock.getDelta();

        this.background.render(delta);
    }
}
