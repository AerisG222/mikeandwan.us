import { Observable } from 'rxjs/Observable';
import { Subscription } from 'rxjs/Subscription';
import 'rxjs/add/observable/fromEvent';
import 'rxjs/add/operator/throttleTime';

import { DataService } from './data-service';
import { StateService } from './state-service';
import { Background } from './background';
import { CategoryListView } from './category-list-view';
import { CategoryObject3D } from './category-object3d';
import { StatusBar } from './status-bar';

export class Photos3D {
    private camera: THREE.PerspectiveCamera;
    private renderer: THREE.WebGLRenderer;
    private ambientLight: THREE.AmbientLight;
    private directionalLight: THREE.DirectionalLight;
    private axisHelper: THREE.AxisHelper;
    private background: Background;
    private nav: StatusBar;
    private categoryListView: CategoryListView;
    private height: number;
    private width: number;
    private sizeCode: string;
    private mouseoverSubscription: Subscription;
    private stateService;

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

        this.mouseoverSubscription = Observable
            .fromEvent<MouseEvent>(document, 'mousemove')
            .throttleTime(10)
            .subscribe(evt => this.onMouseMove(evt));

        this.animate();
    }

    strafeLeft() {
        this.camera.position.x -= 25;
    }

    strafeRight() {
        this.camera.position.x += 25;
    }

    stepForward() {
        this.camera.position.z -= 25;
    }

    stepBackward() {
        this.camera.position.z += 25;
    }

    togglePause() {
        this.isPaused = !this.isPaused;
        this.animate();
    }

    toggleBackground() { 
        if(this.background.isShown) {
            this.background.hide();
        }
        else {
            this.background.show();
        }
    }

    toggleAxisHelper() {
        if(this.axisHelper == null) {
            this.axisHelper = new THREE.AxisHelper(500);
            this.scene.add(this.axisHelper);
        }
        else {
            this.scene.remove(this.axisHelper);
            this.axisHelper = null;
        }
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
            this.camera.updateProjectionMatrix();
        }
    }

    // http://stemkoski.github.io/Three.js/Mouse-Over.html
    private onMouseMove(event: MouseEvent) {
        let x = ( event.clientX / window.innerWidth ) * 2 - 1;
	    let y = - ( event.clientY / window.innerHeight ) * 2 + 1;
        let vector = new THREE.Vector3(x, y, 0.5);
        vector.unproject(this.camera);

        let ray = new THREE.Raycaster(this.camera.position, vector.sub(this.camera.position).normalize());

        // create an array containing all objects in the scene with which the ray intersects, though
        // filter the list to just objects that care about this to optimize perf
        // TODO: get rid of magic number in filter below
	    let intersects = ray
            .intersectObjects(this.scene.children, true)
            .filter(x => x.distance < 800 && x.object.parent instanceof CategoryObject3D);

        this.stateService.updateMouseover(intersects);
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

        this.stateService = new StateService(this.scene, this.camera);

        // status / nav bar
        this.nav = new StatusBar(this.stateService);
        this.nav.init();

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

        this.categoryListView = new CategoryListView(this.scene,
                                                     this.camera,
                                                     this.width,
                                                     this.height,
                                                     this.dataService,
                                                     this.stateService,
                                                     500,
                                                     2000);
        this.categoryListView.init();

        this.toggleAxisHelper();
        
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
        this.categoryListView.render(delta);
    }
}
