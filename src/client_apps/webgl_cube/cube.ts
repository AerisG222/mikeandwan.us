import { DoubleSide, RepeatWrapping } from 'three/src/constants';

import { Fog } from 'three/src/scenes/Fog';
import { TextureLoader } from 'three/src/loaders/TextureLoader';
import { BoxGeometry } from 'three/src/geometries/BoxGeometry';
import { MeshBasicMaterial } from 'three/src/materials/MeshBasicMaterial';
import { PlaneGeometry } from 'three/src/geometries/PlaneGeometry';
import { Scene } from 'three/src/scenes/Scene';
import { PerspectiveCamera } from 'three/src/cameras/PerspectiveCamera';
import { Mesh } from 'three/src/objects/Mesh';
import { MeshPhongMaterial } from 'three/src/materials/MeshPhongMaterial';
import { AmbientLight } from 'three/src/lights/AmbientLight';
import { DirectionalLight } from 'three/src/lights/DirectionalLight';
import { WebGLRenderer } from 'three/src/renderers/WebGLRenderer';
import { AxesHelper } from 'three/src/helpers/AxesHelper';
import * as Stats from 'stats.js';

import * as floorTexture from './floor_texture.jpg';
import * as cubeTexture from './DSC_8562.jpg';

export class CubeDemo {
    scene: Scene;
    camera: PerspectiveCamera;
    renderer: WebGLRenderer;
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
        const directionalLight = new DirectionalLight(0xffffff, 0.9);
        directionalLight.position.set(-1, 1, 1);
        directionalLight.castShadow = true;
        this.scene.add(directionalLight);

        // axis helper
        const axisHelper = new AxesHelper(100);
        this.scene.add(axisHelper);

        // cube
        const textureLoader = new TextureLoader();
        textureLoader.load(cubeTexture, texture => {
            const geometry = new BoxGeometry(50, 50, 50);
            const material = new MeshPhongMaterial({ color: 0xffffff, map: texture });
            this.cube = new Mesh(geometry, material);
            this.cube.position.set(0, 70, 180);
            this.scene.add(this.cube);

            this.loadCounter++;

            this.render();
        });

        // floor
        textureLoader.load(floorTexture, texture => {
            const floorPlane = new PlaneGeometry(1000, 1000);
            texture.wrapS = RepeatWrapping;
            texture.wrapT = RepeatWrapping;
            texture.repeat.set(9, 9);
            const floorMaterial = new MeshBasicMaterial({ map: texture, side: DoubleSide }); //
            const floor = new Mesh(floorPlane, floorMaterial);
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
