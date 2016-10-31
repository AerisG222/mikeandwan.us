import { DataService } from './data-service';
import { Background } from './background';

export class Photos3D {
    private camera: THREE.PerspectiveCamera;
    private renderer: THREE.WebGLRenderer;
    private ambientLight: THREE.AmbientLight;
    private directionalLight: THREE.DirectionalLight;
    private background: Background;
    private height: number;
    private width: number;
    private sizeCode: string;

    private clock = new THREE.Clock();
    private dataService = new DataService();
    private scene = new THREE.Scene();
    private isPaused = false;

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
        this.camera = new THREE.PerspectiveCamera(45, this.width / this.height, 0.1, 2000);
        this.camera.position.set(0, 200, 1000);
        this.camera.lookAt(new THREE.Vector3(0, 200, 0));

        // ambient light
        this.ambientLight = new THREE.AmbientLight(0x404040);
        this.scene.add(this.ambientLight);

        // directional light
        this.directionalLight = new THREE.DirectionalLight(0xffffff, 0.9);
        this.directionalLight.position.set(-1, 1, 1);
        this.directionalLight.castShadow = true;
        this.scene.add(this.directionalLight);

        // background
        this.background = new Background(this.renderer, 
                                         this.scene, 
                                         this.camera,
                                         this.directionalLight,
                                         this.sizeCode);
        this.background.init();

        this.scene.add(new THREE.AxisHelper(500));

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
