import { Scene, PerspectiveCamera, Renderer, Mesh, AmbientLight, WebGLRenderer, DirectionalLight, AxesHelper } from 'three';
import * as GLTFLoader from 'three-gltf-loader';
import * as Stats from 'stats.js';

const model = require('./bs.gltf');
const bin = require('./bs2.bin');

export class BlenderModelDemo {
    scene: Scene;
    camera: PerspectiveCamera;
    renderer: Renderer;
    model: Mesh;
    ambientLight: AmbientLight;
    stats: Stats;
    loader: GLTFLoader;

    run() {
        this.prepareScene();
        this.render();
    }

    private prepareScene() {
        // this.scene
        this.scene = new Scene();

        // renderer
        this.renderer = new WebGLRenderer({ antialias: true, alpha: true });
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        document.body.appendChild(this.renderer.domElement);

        // stats
        this.stats = new Stats();
        document.body.appendChild(this.stats.dom);

        // camera
        this.camera = new PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);
        this.camera.position.set(0, 10, 20);
        this.camera.lookAt(this.scene.position);

        // ambient light
        this.ambientLight = new AmbientLight(0x404040);
        this.scene.add(this.ambientLight);

        // directional light
        let directionalLight = new DirectionalLight(0xffffff, 0.9);
        directionalLight.position.set(-1, 1, 1);
        directionalLight.castShadow = true;
        this.scene.add(directionalLight);

        // axis helper
        let axisHelper = new AxesHelper(10);
        this.scene.add(axisHelper);

        // model loader
        this.loader = new GLTFLoader();

        this.loader.load(model, gltf => {
            this.scene.add( gltf.scene );
        });
    }

    private render() {
        requestAnimationFrame(() => this.render());

        // this.cube.rotation.x += 0.01;
        // this.cube.rotation.y += 0.01;

        this.renderer.render(this.scene, this.camera);

        this.stats.update();
    }
}
