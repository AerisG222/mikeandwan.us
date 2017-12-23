import { Scene, PerspectiveCamera, Renderer, Mesh, AmbientLight, Fog, WebGLRenderer, DirectionalLight,
         AxisHelper, TextureLoader, BoxGeometry, MeshBasicMaterial, MeshPhongMaterial, PlaneGeometry,
         DoubleSide, RepeatWrapping
       } from 'three';

import * as Stats from 'stats.js';

export class CubeDemo {
    scene: Scene;
    camera: PerspectiveCamera;
    renderer: Renderer;
    cube: Mesh;
    ambientLight: AmbientLight;
    stats: Stats;
    loadCounter = 0;

    run() {
        this.prepareScene();
        this.render();
    }

    private prepareScene() {
        // this.scene
        this.scene = new Scene();

        // fog
        this.scene.fog = new Fog(0x9999ff, 400, 1000);

        // renderer
        this.renderer = new WebGLRenderer({ antialias: true, alpha: true });
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        document.body.appendChild(this.renderer.domElement);

        // stats
        this.stats = new Stats();
        document.body.appendChild(this.stats.dom);

        // camera
        this.camera = new PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);
        this.camera.position.set(0, 150, 400);
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
        let axisHelper = new AxisHelper(100);
        this.scene.add(axisHelper);

        // cube
        let textureLoader = new TextureLoader();
        textureLoader.load('/images/2013/alyssas_first_snowman/xs/DSC_9960.jpg', texture => {
            let geometry = new BoxGeometry(50, 50, 50);
            let material = new MeshPhongMaterial({ color: 0xffffff, map: texture });
            this.cube = new Mesh(geometry, material);
            this.cube.position.set(0, 70, 180);
            this.scene.add(this.cube);

            this.loadCounter++;

            this.render();
        });

        // floor
        textureLoader.load('/img/webgl/floor_texture.jpg', texture => {
            let floorPlane = new PlaneGeometry(1000, 1000);
            texture.wrapS = RepeatWrapping;
            texture.wrapT = RepeatWrapping;
            texture.repeat.set(9, 9);
            let floorMaterial = new MeshBasicMaterial({ map: texture, side: DoubleSide }); //
            let floor = new Mesh(floorPlane, floorMaterial);
            floor.position.y = -30;
            floor.rotation.x = (Math.PI / 2) - .3;
            this.scene.add(floor);

            this.loadCounter++;

            this.render();
        });
    }

    private render() {
        if (this.loadCounter < 2) {
            return;
        }

        requestAnimationFrame(() => this.render());

        this.cube.rotation.x += 0.01;
        this.cube.rotation.y += 0.01;

        this.renderer.render(this.scene, this.camera);

        this.stats.update();
    }
}
