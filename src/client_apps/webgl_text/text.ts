import { Scene, PerspectiveCamera, Renderer, Mesh, Object3D, AmbientLight, SpotLight, Font, Fog, WebGLRenderer,
         AxesHelper, DirectionalLight, TextureLoader, PlaneGeometry, RepeatWrapping, DoubleSide,
         MeshPhongMaterial, MeshBasicMaterial
       } from 'three';

import { FontLoader } from 'three/examples/jsm/loaders/FontLoader.js'
import { TextGeometry } from 'three/examples/jsm/geometries/TextGeometry.js'

import * as Stats from 'stats.js';

import * as floorTexture from './floor_texture.jpg';
import * as fontFile from './open_sans_bold.json';

export const _script_root = document.currentScript;

export class TextDemo {
    scene: Scene;
    camera: PerspectiveCamera;
    renderer: Renderer;
    textMesh: Mesh;
    textPivot: Object3D;
    ambientLight: AmbientLight;
    spotLight: SpotLight;
    stats: Stats;
    font: Font;
    rotationAngle: number;
    animate = false;
    loaderCount = 0;

    run() {
        this.prepareScene();
        this.render();
    }

    render() {
        requestAnimationFrame(() => this.render());

        if (this.loaderCount < 2) {
            return;
        }

        if (this.animate) {
            if (this.textPivot.rotation.y >= Math.PI / 3) {
                this.rotationAngle = -0.015;
            }

            if (this.textPivot.rotation.y <= -(Math.PI / 3)) {
                this.rotationAngle = 0.015;
            }

            this.textPivot.rotation.y += this.rotationAngle;
            this.textPivot.rotation.x += this.rotationAngle / 8;
            this.textPivot.rotation.z += this.rotationAngle / 8;

            this.renderer.render(this.scene, this.camera);

            this.stats.update();
        }
    }

    private prepareScene() {
        // scene
        this.scene = new Scene();
        this.scene.fog = new Fog(0x449999, 400, 1000);

        // renderer
        this.renderer = new WebGLRenderer({ antialias: true, alpha: true });
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        document.body.appendChild(this.renderer.domElement);

        // stats
        this.stats = new Stats();
        document.body.appendChild(this.stats.dom);

        // axis helper
        let axisHelper = new AxesHelper(100);
        this.scene.add(axisHelper);

        // camera
        this.camera = new PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);
        this.camera.position.set(0, 140, 400);
        this.camera.lookAt(this.scene.position);

        // ambient light
        this.ambientLight = new AmbientLight(0x404040);
        this.scene.add(this.ambientLight);

        let directionalLight = new DirectionalLight(0xffffff, 0.5);
        directionalLight.position.set(0, 1, 0);
        this.scene.add(directionalLight);

        // spot light
        this.spotLight = new SpotLight(0xffffff);
        this.spotLight.position.set(-60, 200, 100);
        this.spotLight.castShadow = true;
        this.scene.add(this.spotLight);

        // floor
        let textureLoader = new TextureLoader();
        textureLoader.load(floorTexture, texture => {
            let floorPlane = new PlaneGeometry(1000, 1000);
            texture.wrapS = RepeatWrapping;
            texture.wrapT = RepeatWrapping;
            texture.repeat.set(24, 24);
            let floorMaterial = new MeshBasicMaterial({ map: texture, side: DoubleSide });
            let floor = new Mesh(floorPlane, floorMaterial);
            floor.position.y = -30;
            floor.rotation.x = (Math.PI / 2) - 0.2;
            this.scene.add(floor);

            this.loaderCount++;
            this.render();
        });

        // setup initial rotation
        this.rotationAngle = 0.015;

        // prepare textMesh
        this.prepareText();
    }

    private prepareText() {
        let loader = new FontLoader();

        loader.load(fontFile, response => {
            let textGeometry = new TextGeometry('WebGL', {
                font: response,
                size: 60,
                curveSegments: 48,
                height: 24,
                bevelEnabled: false,
                bevelThickness: 0,
                bevelSize: 0
            });

            let textMaterial = new MeshPhongMaterial({
                color: 0x006051,
                specular: 0xffffff,
                shininess: 50
            });

            textGeometry.computeBoundingBox();
            let textWidth = textGeometry.boundingBox.max.x - textGeometry.boundingBox.min.x;

            this.textMesh = new Mesh(textGeometry, textMaterial);
            this.textMesh.position.set(-0.5 * textWidth, 30, 10);
            this.scene.add(this.textMesh);

            // pivot
            this.textPivot = new Object3D();
            this.textPivot.add(this.textMesh);
            this.scene.add(this.textPivot);

            this.animate = true;

            this.loaderCount++;
            this.render();
        });
    }
}
