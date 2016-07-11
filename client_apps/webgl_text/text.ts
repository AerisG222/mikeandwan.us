/// <reference path="../../typings/browser.d.ts" />

jQuery(function() {
	let demo = new TextDemo();
	demo.run();
});

class TextDemo {
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

	run() {
		this.prepareScene();
		this.render();
	}

	prepareScene() {
		// scene
		this.scene = new THREE.Scene();
		this.scene.fog = new THREE.Fog(0x449999, 400, 1000);

		// renderer
		this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
		this.renderer.setSize(window.innerWidth, window.innerHeight);
		document.body.appendChild(this.renderer.domElement);

		// stats
		this.stats = new Stats();
		this.stats.domElement.style.position = 'absolute';
		this.stats.domElement.style.top = '0px';
		document.body.appendChild(this.stats.domElement);

		// axis helper
		let axisHelper = new THREE.AxisHelper(100);
		this.scene.add(axisHelper);

		// camera
		this.camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);
		this.camera.position.set(0, 140, 400);
		this.camera.lookAt(this.scene.position);

		// ambient light
		this.ambientLight = new THREE.AmbientLight(0x404040);
		this.scene.add(this.ambientLight);

		let directionalLight = new THREE.DirectionalLight(0xffffff, 0.5);
		directionalLight.position.set(0, 1, 0);
		this.scene.add(directionalLight);

		// spot light
		this.spotLight = new THREE.SpotLight(0xffffff);
		this.spotLight.position.set(-60, 200, 100);
		this.spotLight.castShadow = true;
		this.scene.add(this.spotLight);

		// floor
		let floorPlane = new THREE.PlaneGeometry(1000, 1000);
		let floorTexture = THREE.ImageUtils.loadTexture('/img/webgl/floor_texture.jpg');
		floorTexture.wrapS = THREE.RepeatWrapping;
		floorTexture.wrapT = THREE.RepeatWrapping;
		floorTexture.repeat.set(24, 24);
		let floorMaterial = new THREE.MeshBasicMaterial({ map: floorTexture, side: THREE.DoubleSide });
		let floor = new THREE.Mesh(floorPlane, floorMaterial);
		floor.position.y = -30;
		floor.rotation.x = (Math.PI / 2) - 0.2;
		this.scene.add(floor);

		// setup initial rotation
		this.rotationAngle = 0.015;

        // prepare textMesh
        this.prepareText();
	}

    prepareText() {
        var loader = new THREE.FontLoader();

        loader.load( '/js/libs/fonts/open_sans_bold.typeface.js', response => {
            // text
            let textGeometry = new THREE.TextGeometry('WebGL', {
                font: response,
                size: 60,
                curveSegments: 48,
                height: 24,
                weight: 'bold',
                style: 'normal',
                bevelEnabled: false
            });

            let textMaterial = new THREE.MeshPhongMaterial({
                color: 0x006051,
                specular: 0xffffff,
                shininess: 50
            });

            textGeometry.computeBoundingBox();
            let textWidth = textGeometry.boundingBox.max.x - textGeometry.boundingBox.min.x;

            this.textMesh = new THREE.Mesh(textGeometry, textMaterial);
            this.textMesh.position.set(-0.5 * textWidth, 30, 10);
            this.scene.add(this.textMesh);

            // pivot
            this.textPivot = new THREE.Object3D();
            this.textPivot.add(this.textMesh);
            this.scene.add(this.textPivot);

            this.animate = true;
        });
    }

	render() {
		requestAnimationFrame(() => this.render());

        if(this.animate) {
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
}
