import { Scene, PerspectiveCamera, Renderer, Mesh, MeshPhongMaterial, AmbientLight,
         WebGLRenderer, DirectionalLight, AxesHelper } from 'three';
import * as GLTFLoader from 'three-gltf-loader';
import * as Stats from 'stats.js';

const model = require('./bs.gltf');
const bin = require('./bs2.bin');

export class BlenderModelDemo {
    private _scene: Scene;
    private _camera: PerspectiveCamera;
    private _renderer: Renderer;
    private _ambientLight: AmbientLight;
    private _stats: Stats;
    private _loader: GLTFLoader;
    private _smStar: Mesh;
    private _mdStar: Mesh;
    private _lgStar: Mesh;
    private _frameCounter = 0;
    private _rotateMultiplier = 1.0;

    run() {
        this.prepareScene();
        this.render();
    }

    private prepareScene() {
        // this.scene
        this._scene = new Scene();

        // renderer
        this._renderer = new WebGLRenderer({ antialias: true, alpha: true });
        this._renderer.setSize(window.innerWidth, window.innerHeight);
        document.body.appendChild(this._renderer.domElement);

        // stats
        this._stats = new Stats();
        document.body.appendChild(this._stats.dom);

        // camera
        this._camera = new PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);
        this._camera.position.set(0, 10, 20);
        this._camera.lookAt(this._scene.position);

        // ambient light
        this._ambientLight = new AmbientLight(0x404040);
        this._scene.add(this._ambientLight);

        // directional light
        let directionalLight = new DirectionalLight(0xffffff, 0.9);
        directionalLight.position.set(-1, 1, 1);
        directionalLight.castShadow = true;
        this._scene.add(directionalLight);

        // axis helper
        let axisHelper = new AxesHelper(10);
        this._scene.add(axisHelper);

        // model loader
        this._loader = new GLTFLoader();

        this._loader.load(model, gltf => {
            this.setupModel(gltf);
        });
    }

    private setupModel(gltf) {
        this._smStar = this.prepareMesh(gltf, 'Small_Star',  '#000b8c');
        this._mdStar = this.prepareMesh(gltf, 'Medium_Star', '#0047ba');
        this._lgStar = this.prepareMesh(gltf, 'Large_Star',  '#acc5e9');
    }

    private prepareMesh(gltf, name: string, color: string): Mesh {
        const mesh = gltf.scene.children.find(x => x.name === name  && x.type === 'Mesh');
        const mat = new MeshPhongMaterial({
            color: color,
            specular: 0xffffff,
            shininess: 20
        });

        mesh.material = mat;

        this._scene.add(mesh);

        return mesh;
    }

    private render() {
        requestAnimationFrame(() => this.render());

        if (this._smStar !== undefined &&
            this._mdStar !== undefined &&
            this._lgStar !== undefined) {
            this._smStar.rotation.x += 0.01 * this._rotateMultiplier;
            this._mdStar.rotation.x += 0.01 * this._rotateMultiplier;
            this._lgStar.rotation.x += 0.01 * this._rotateMultiplier;

            this._frameCounter++;

            if(this._frameCounter > 40) {
                this._frameCounter = 0;
                this._rotateMultiplier = this._rotateMultiplier * -1.0;
            }
        }

        this._renderer.render(this._scene, this._camera);

        this._stats.update();
    }
}
