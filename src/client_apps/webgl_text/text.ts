import * as THREE from 'three';
import * as Stats from 'stats.js';

import { FontLoader } from 'three/examples/jsm/loaders/FontLoader.js'
import { TextGeometry } from 'three/examples/jsm/geometries/TextGeometry.js'

import * as floorTexture from './floor_texture.jpg';
import * as fontFile from './open_sans_bold.json';

export class TextDemo {
    scene: THREE.Scene;
    camera: THREE.PerspectiveCamera;
    renderer: THREE.Renderer;
    textMesh: THREE.Mesh;
    textPivot: THREE.Object3D;
    ambientLight: THREE.AmbientLight;
    spotLight: THREE.SpotLight;
    stats: Stats;
    font: THREE.Font;
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
        this.scene = new THREE.Scene();
        this.scene.fog = new THREE.Fog(0x449999, 400, 1000);

        // renderer
        this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        document.body.appendChild(this.renderer.domElement);

        // stats
        this.stats = new Stats();
        document.body.appendChild(this.stats.dom);

        // axis helper
        const axisHelper = new THREE.AxesHelper(100);
        this.scene.add(axisHelper);

        // camera
        this.camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);
        this.camera.position.set(0, 140, 400);
        this.camera.lookAt(this.scene.position);

        // ambient light
        this.ambientLight = new THREE.AmbientLight(0x909090);
        this.scene.add(this.ambientLight);

        const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
        directionalLight.position.set(0, 1, 0);
        this.scene.add(directionalLight);

        // spot light
        this.spotLight = new THREE.SpotLight(0xffffff);
        this.spotLight.position.set(-50, 180, 80);
        this.spotLight.castShadow = true;
        this.scene.add(this.spotLight);

        // floor
        const textureLoader = new THREE.TextureLoader();
        textureLoader.load(floorTexture, texture => {
            texture.colorSpace = THREE.SRGBColorSpace;
            const floorPlane = new THREE.PlaneGeometry(1000, 1000);
            texture.wrapS = THREE.RepeatWrapping;
            texture.wrapT = THREE.RepeatWrapping;
            texture.repeat.set(24, 24);
            const floorMaterial = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });
            const floor = new THREE.Mesh(floorPlane, floorMaterial);
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
        const loader = new FontLoader();

        loader.load(fontFile, response => {
            const textGeometry = new TextGeometry('WebGL', {
                font: response,
                size: 60,
                curveSegments: 48,
                height: 24,
                bevelEnabled: false,
                bevelThickness: 0,
                bevelSize: 0
            });

            const textMaterial = new THREE.MeshPhongMaterial({
                color: 0x006051,
                specular: 0xffffff,
                shininess: 50
            });

            textGeometry.computeBoundingBox();
            const textWidth = textGeometry.boundingBox.max.x - textGeometry.boundingBox.min.x;

            this.textMesh = new THREE.Mesh(textGeometry, textMaterial);
            this.textMesh.position.set(-0.5 * textWidth, 30, 10);
            this.scene.add(this.textMesh);

            // pivot
            this.textPivot = new THREE.Object3D();
            this.textPivot.add(this.textMesh);
            this.scene.add(this.textPivot);

            this.animate = true;

            this.loaderCount++;
            this.render();
        });
    }
}
