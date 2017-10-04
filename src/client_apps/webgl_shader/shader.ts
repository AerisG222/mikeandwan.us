import * as THREE from 'three';
import * as Stats from 'stats.js';

export class ShaderDemo {
    readonly HEIGHT = 480;
    readonly WIDTH = 640;

    scene: THREE.Scene;
    camera: THREE.PerspectiveCamera;
    renderer: THREE.Renderer;
    stats: Stats;

    run() {
        this.prepareScene();
        this.render();
    }

    private prepareScene() {
        const container = document.getElementById('container');

        this.scene = new THREE.Scene();

        this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
        this.renderer.setSize(this.WIDTH, this.HEIGHT);
        container.appendChild(this.renderer.domElement);

        this.stats = new Stats();
        container.appendChild(this.stats.dom);

        this.camera = new THREE.PerspectiveCamera(45, this.WIDTH / this.HEIGHT, 0.1, 10000);
        this.camera.position.set(0, 0, 300);
        this.camera.lookAt(this.scene.position);

        let sphere = new THREE.Mesh(
            new THREE.SphereGeometry(100, 32, 32),
            new THREE.ShaderMaterial({
                vertexShader: document.getElementById('vertexshader').innerText,
                fragmentShader: document.getElementById('fragmentshader').innerText
            })
        );

        this.scene.add(sphere);
    }

    render() {
        // requestAnimationFrame(() => this.render());

        this.renderer.render(this.scene, this.camera);

        this.stats.update();
    }
}
