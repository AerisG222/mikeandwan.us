import { Component, Input, ElementRef, OnInit } from '@angular/core';
import { NgStyle } from '@angular/common';

@Component({
    moduleId: module.id,
    selector: 'app-three-dlink',
    directives: [ NgStyle ],
    templateUrl: 'three-dlink.component.html',
    styleUrls: ['three-dlink.component.css']
})
export class ThreeDLinkComponent implements OnInit {
    private _el : HTMLElement = null;
    private _counter : number = 0;
    @Input() url : string = null;
    @Input() width : number = null;
    @Input() height : number = null;
    showLink = Modernizr.webgl;
    scene : THREE.Scene = null;
    camera : THREE.PerspectiveCamera = null;
    renderer : THREE.WebGLRenderer = null;
    directionalLight1 : THREE.DirectionalLight = null;
    directionalLight2 : THREE.DirectionalLight = null;
    threeFrame : THREE.Object3D = null;
    threeUnitVector : THREE.Vector3 = null;
    dashFrame : THREE.Object3D = null;
    dashUnitVector : THREE.Vector3 = null;
    dFrame : THREE.Object3D = null;
    dUnitVector : THREE.Vector3 = null;
    
    constructor(el : ElementRef) {
        this._el = el.nativeElement;
    }

    ngOnInit() : void {
        if(this.showLink) {
            this.scene = new THREE.Scene();
            this.camera = new THREE.PerspectiveCamera(75, this.width / this.height, 0.1, 50);
            this.renderer = new THREE.WebGLRenderer({ antialias: true, alpha: true });
    
            this.renderer.setSize(this.width, this.height);
            this.renderer.setClearColor(0x000000, 0);
    
            this._el.appendChild(this.renderer.domElement);
    
            this.prepareScene();
            this.render();
        }
    }
    
    prepareScene() : void {
        let threeGeometry = new THREE.TextGeometry('3', { 
            font: 'open sans', 
            size: 24,
            curveSegments: 24, 
            height: 8,
            weight: 'bold', 
            style: 'normal', 
            bevelEnabled: false
        });
        
        let dashGeometry = new THREE.TextGeometry('-', { 
            font: 'open sans', 
            size: 24,
            curveSegments: 24, 
            height: 4,
            weight: 'bold', 
            style: 'normal', 
            bevelEnabled: false
        });
        
        let dGeometry = new THREE.TextGeometry('D', { 
            font: 'open sans', 
            size: 24,
            curveSegments: 24, 
            height: 8,
            weight: 'bold', 
            style: 'normal', 
            bevelEnabled: false
        });
        
        let material = new THREE.MeshPhongMaterial( { 
            color: 0xcccccc, 
            specular: 0xffffff, 
            shininess: 40, 
            shading: 
            THREE.FlatShading 
        });

        let threeText = new THREE.Mesh(threeGeometry, material);
        let dashText = new THREE.Mesh(dashGeometry, material);
        let dText = new THREE.Mesh(dGeometry, material);

        // position the characters so the middle is at the origin
        threeText.position.set(-12, -12, -4);
        dashText.position.set(-12, -9, -2);
        dText.position.set(-12, -12, -4);

        this.threeFrame = new THREE.Object3D();
        this.dashFrame = new THREE.Object3D();
        this.dFrame = new THREE.Object3D();

        this.threeFrame.add(threeText);
        this.dashFrame.add(dashText);
        this.dFrame.add(dText);

        this.threeFrame.position.set(-16, 0, -4);
        this.dashFrame.position.set(8, 0, -2);
        this.dFrame.position.set(20, 0, -4);

        this.directionalLight1 = new THREE.DirectionalLight( 0xcccccc, 0.4 ); 
        this.directionalLight1.position.set(8, 10, 18); 

        this.directionalLight2 = new THREE.DirectionalLight( 0xcccccc, 0.4 ); 
        this.directionalLight2.position.set(-16, 16, 30);

        this.threeUnitVector = new THREE.Vector3(1, 0, 0);
        this.dashUnitVector = new THREE.Vector3(1, 0, 0);
        this.dUnitVector = new THREE.Vector3(0, 1, 0);

        this.scene.add(this.directionalLight1);
        this.scene.add(this.directionalLight2);
        this.scene.add(this.threeFrame);
        this.scene.add(this.dashFrame);
        this.scene.add(this.dFrame);

        this.camera.position.z = 28;
    }

    render() : void { 
        if(this._counter > 25) {
            // if a user doesn't move away from the current view, stop the animation so we don't annoy them
            return;
        }

        requestAnimationFrame(() => this.render);

        this._counter += 0.05;

        let angle = Math.sin(this._counter) / 2.0;

        let threeQuaternion = new THREE.Quaternion().setFromAxisAngle(this.threeUnitVector, angle);
        let dashQuaternion = new THREE.Quaternion().setFromAxisAngle(this.dashUnitVector, this._counter);
        let dQuaternion = new THREE.Quaternion().setFromAxisAngle(this.dUnitVector, angle);

        this.threeFrame.rotation.setFromQuaternion(threeQuaternion, 'XYZ');
        this.dashFrame.rotation.setFromQuaternion(dashQuaternion, 'XYZ');
        this.dFrame.rotation.setFromQuaternion(dQuaternion, 'XYZ');

        this.renderer.render(this.scene, this.camera); 
    }

    onMouseenter() : void {
        this.directionalLight1.color.setHex(0x00ffff);
        this.directionalLight2.color.setHex(0x00ffff);

        if(this._counter >= 25) {
            this._counter = 0;
        }

        this.render();
    }

    onMouseout() : void {
        this.directionalLight1.color.setHex(0xcccccc);
        this.directionalLight2.color.setHex(0xcccccc);

        this._counter = 24.999;
        this.render();
    }
}
