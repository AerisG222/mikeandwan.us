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

    setupSphere() {
        const geometry = new THREE.SphereBufferGeometry(100, 32, 32);

        const verts = geometry.getAttribute('position');
        const displacement = new Float32Array(verts.count);
        const colors = new Float32Array(verts.count * 3);
        const color = new THREE.Color();

        for (let v = 0, c = 0; v < verts.count; v++, c += 3) {
            displacement[v] = Math.random() * 20;

            color.setHSL(v / verts.count, 1.0, 0.5);

            colors[c + 0] = color.r;
            colors[c + 1] = color.g;
            colors[c + 2] = color.b;
        }

        geometry.setAttribute('displacement', new THREE.BufferAttribute(displacement, 1));
        geometry.setAttribute('colors', new THREE.BufferAttribute(colors, 3));

        const sphere = new THREE.Mesh(
            geometry,
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

        this.setupSphere();
    }
}
