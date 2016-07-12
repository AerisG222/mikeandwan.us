/// <reference path="./typings/browser.d.ts" />
jQuery(function () {
    var demo = new CubeDemo();
    demo.run();
});
var CubeDemo = (function () {
    function CubeDemo() {
    }
    CubeDemo.prototype.run = function () {
        this.prepareScene();
        this.render();
    };
    CubeDemo.prototype.prepareScene = function () {
        // this.scene
        this.scene = new THREE.Scene();
        // fog
        this.scene.fog = new THREE.Fog(0x9999ff, 400, 1000);
        // renderer
        this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
        this.renderer.setSize(window.innerWidth, window.innerHeight);
        document.body.appendChild(this.renderer.domElement);
        // stats
        this.stats = new Stats();
        this.stats.domElement.style.position = 'absolute';
        this.stats.domElement.style.top = '0px';
        document.body.appendChild(this.stats.domElement);
        // camera
        this.camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);
        this.camera.position.set(0, 150, 400);
        this.camera.lookAt(this.scene.position);
        // ambient light
        this.ambientLight = new THREE.AmbientLight(0x404040);
        this.scene.add(this.ambientLight);
        // directional light
        var directionalLight = new THREE.DirectionalLight(0xffffff, 0.9);
        directionalLight.position.set(-1, 1, 1);
        directionalLight.castShadow = true;
        this.scene.add(directionalLight);
        // axis helper
        var axisHelper = new THREE.AxisHelper(100);
        this.scene.add(axisHelper);
        // cube
        var texture = THREE.ImageUtils.loadTexture('/images/2013/alyssas_first_snowman/thumbnails/DSC_9960.jpg');
        var geometry = new THREE.BoxGeometry(50, 50, 50);
        var material = new THREE.MeshPhongMaterial({ color: 0xffffff, map: texture });
        this.cube = new THREE.Mesh(geometry, material);
        this.cube.position.set(0, 70, 180);
        this.scene.add(this.cube);
        // floor
        var floorPlane = new THREE.PlaneGeometry(1000, 1000);
        var floorTexture = THREE.ImageUtils.loadTexture('/img/photos3d/floor.jpg');
        floorTexture.wrapS = THREE.RepeatWrapping;
        floorTexture.wrapT = THREE.RepeatWrapping;
        floorTexture.repeat.set(9, 9);
        var floorMaterial = new THREE.MeshBasicMaterial({ map: floorTexture, side: THREE.DoubleSide }); //
        var floor = new THREE.Mesh(floorPlane, floorMaterial);
        floor.position.y = -30;
        floor.rotation.x = (Math.PI / 2) - .3;
        this.scene.add(floor);
    };
    CubeDemo.prototype.render = function () {
        var _this = this;
        requestAnimationFrame(function () { return _this.render(); });
        this.cube.rotation.x += 0.01;
        this.cube.rotation.y += 0.01;
        this.renderer.render(this.scene, this.camera);
        this.stats.update();
    };
    return CubeDemo;
}());
//# sourceMappingURL=/cube.js.map