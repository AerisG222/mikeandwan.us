import { Component, Input, ElementRef, OnInit } from '@angular/core';

import { Scene, PerspectiveCamera, WebGLRenderer, DirectionalLight, Object3D, FlatShading,
         Vector3, FontLoader, TextGeometry, Mesh, MeshPhongMaterial, Quaternion
       } from 'three';

@Component({
    selector: 'app-three-dlink',
    templateUrl: './three-dlink.component.html',
    styleUrls: [ './three-dlink.component.css' ]
})
export class ThreeDLinkComponent implements OnInit {
    private _el: HTMLElement = null;
    private _counter = 0;
    @Input() url: string = null;
    @Input() width: number = null;
    @Input() height: number = null;
    showLink = Modernizr.webgl;
    scene: Scene = null;
    camera: PerspectiveCamera = null;
    renderer: WebGLRenderer = null;
    directionalLight1: DirectionalLight = null;
    directionalLight2: DirectionalLight = null;
    threeFrame: Object3D = null;
    threeUnitVector: Vector3 = null;
    dashFrame: Object3D = null;
    dashUnitVector: Vector3 = null;
    dFrame: Object3D = null;
    dUnitVector: Vector3 = null;

    constructor(el: ElementRef) {
        this._el = el.nativeElement;
    }

    ngOnInit(): void {
        if (this.showLink) {
            this.scene = new Scene();
            this.camera = new PerspectiveCamera(75, this.width / this.height, 0.1, 50);
            this.renderer = new WebGLRenderer({ antialias: true, alpha: true });

            this.renderer.setSize(this.width, this.height);
            this.renderer.setClearColor(0x000000, 0);

            this._el.appendChild(this.renderer.domElement);

            this.prepareScene();
            this.render();
        }
    }

    prepareScene(): void {
        const loader = new FontLoader();

        loader.load('/js/libs/fonts/open_sans_bold.typeface.js', response => {
            const threeGeometry = new TextGeometry('3', {
                font: response,
                size: 60,
                curveSegments: 48,
                height: 24,
                bevelEnabled: false,
                bevelThickness: 0,
                bevelSize: 0
            });

            // threeGeometry.weight: 'bold',
            // threeGeometry.style: 'normal',

            const dashGeometry = new TextGeometry('-', {
                font: response,
                size: 60,
                curveSegments: 48,
                height: 24,
                bevelEnabled: false,
                bevelThickness: 0,
                bevelSize: 0
            });

            // dashGeometry.weight: 'bold',
            // dashGeometry.style: 'normal',

            const dGeometry = new TextGeometry('D', {
                font: response,
                size: 60,
                curveSegments: 48,
                height: 24,
                bevelEnabled: false,
                bevelThickness: 0,
                bevelSize: 0
            });

            // dGeometry.weight: 'bold',
            // dGeometry.style: 'normal',

            const material = new MeshPhongMaterial({
                color: 0xcccccc,
                specular: 0xffffff,
                shininess: 40,
                shading:
                FlatShading
            });

            const threeText = new Mesh(threeGeometry, material);
            const dashText = new Mesh(dashGeometry, material);
            const dText = new Mesh(dGeometry, material);

            // position the characters so the middle is at the origin
            threeText.position.set(-12, -12, -4);
            dashText.position.set(-12, -9, -2);
            dText.position.set(-12, -12, -4);

            this.threeFrame = new Object3D();
            this.dashFrame = new Object3D();
            this.dFrame = new Object3D();

            this.threeFrame.add(threeText);
            this.dashFrame.add(dashText);
            this.dFrame.add(dText);

            this.threeFrame.position.set(-16, 0, -4);
            this.dashFrame.position.set(8, 0, -2);
            this.dFrame.position.set(20, 0, -4);

            this.directionalLight1 = new DirectionalLight(0xcccccc, 0.4);
            this.directionalLight1.position.set(8, 10, 18);

            this.directionalLight2 = new DirectionalLight(0xcccccc, 0.4);
            this.directionalLight2.position.set(-16, 16, 30);

            this.threeUnitVector = new Vector3(1, 0, 0);
            this.dashUnitVector = new Vector3(1, 0, 0);
            this.dUnitVector = new Vector3(0, 1, 0);

            this.scene.add(this.directionalLight1);
            this.scene.add(this.directionalLight2);
            this.scene.add(this.threeFrame);
            this.scene.add(this.dashFrame);
            this.scene.add(this.dFrame);

            this.camera.position.z = 28;
        });
    }

    render(): void {
        if (this._counter > 25) {
            // if a user doesn't move away from the current view, stop the animation so we don't annoy them
            return;
        }

        requestAnimationFrame(() => this.render);

        this._counter += 0.05;

        const angle = Math.sin(this._counter) / 2.0;

        const threeQuaternion = new Quaternion().setFromAxisAngle(this.threeUnitVector, angle);
        const dashQuaternion = new Quaternion().setFromAxisAngle(this.dashUnitVector, this._counter);
        const dQuaternion = new Quaternion().setFromAxisAngle(this.dUnitVector, angle);

        this.threeFrame.rotation.setFromQuaternion(threeQuaternion, 'XYZ');
        this.dashFrame.rotation.setFromQuaternion(dashQuaternion, 'XYZ');
        this.dFrame.rotation.setFromQuaternion(dQuaternion, 'XYZ');

        this.renderer.render(this.scene, this.camera);
    }

    onMouseenter(): void {
        this.directionalLight1.color.setHex(0x00ffff);
        this.directionalLight2.color.setHex(0x00ffff);

        if (this._counter >= 25) {
            this._counter = 0;
        }

        this.render();
    }

    onMouseout(): void {
        this.directionalLight1.color.setHex(0xcccccc);
        this.directionalLight2.color.setHex(0xcccccc);

        this._counter = 24.999;
        this.render();
    }
}
