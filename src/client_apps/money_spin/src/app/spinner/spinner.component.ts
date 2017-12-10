import { Component, ElementRef, EventEmitter, Output, OnInit, OnDestroy } from '@angular/core';

import { Scene, OrthographicCamera, Renderer, Group, WebGLRenderer, TextureLoader, Texture,
         PlaneGeometry, Mesh, MeshBasicMaterial, DoubleSide, Math as _Math
       } from 'three';

import { BoardSector } from '../board-sector.model';

@Component({
    selector: 'app-spinner',
    templateUrl: './spinner.component.html',
    styleUrls: [ './spinner.component.css' ]
})
export class SpinnerComponent implements OnInit, OnDestroy {
    private static SECTORS = [
        new BoardSector(37, 8),
        new BoardSector(97, 6),
        new BoardSector(128, 4),
        new BoardSector(157, 1),
        new BoardSector(178, 9),
        new BoardSector(219, 7),
        new BoardSector(265, 5),
        new BoardSector(286, 3),
        new BoardSector(313, 2),
        new BoardSector(362, 10), // last degree inflated to catch any possible overflow condition
    ];
    private static MIN_TOP_SPEED = 0.2;
    private static MAX_ADDITIONAL_TOP_SPEED = 0.4;
    private static MIN_DROPOFF = .002;
    private static MAX_ADDITIONAL_DROPOFF = .003;
    private static MIN_TOP_SPEED_TIME_MS = 511;
    private static MAX_ADDITIONAL_TOP_SPEED_TIME_MS = 1511;
    _el: HTMLDivElement;
    _scene: Scene;
    _camera: OrthographicCamera;
    _renderer: Renderer;
    _board: Group;
    _arrow: Group;
    _arrowSpeed = 0;
    _speedDropoff = 0;
    _slowDown = true;
    _animationId: number;
    @Output() spinCompleted = new EventEmitter<number>();

    constructor(private _elRef: ElementRef) {

    }

    ngOnInit(): void {
        this._el = this._elRef.nativeElement.querySelector('div');
        this.prepareScene();
    }

    ngOnDestroy(): void {
        // Stop the animation / cleanup
        cancelAnimationFrame(this._animationId);
        this._camera = null;
        this._scene = null;

        // kill the rendering node
        while (this._el.hasChildNodes()) {
            this._el.removeChild(this._el.firstChild);
        }
    }

    prepareScene(): void {
        // scene
        this._scene = new Scene();

        // renderer
        this._renderer = new WebGLRenderer({ antialias: true, alpha: true });
        this._renderer.setSize(640, 498);
        this._el.appendChild(this._renderer.domElement);

        // camera
        this._camera = new OrthographicCamera(320, -320, 250, -250, 0.1, 1000);
        this._camera.position.set(0, 0, -10);
        this._camera.lookAt(this._scene.position);
        this._scene.add(this._camera);

        this._board = new Group();
        this._scene.add(this._board);

        this._arrow = new Group();
        this._scene.add(this._arrow);

        const loader = new TextureLoader();
        loader.load('/img/games/money_spin/board.png', (texture: Texture) => {
            const geometry = new PlaneGeometry(640, 498);
            const material = new MeshBasicMaterial({ map: texture, side: DoubleSide });
            const mesh = new Mesh(geometry, material);
            this._board.add(mesh);
        });
        loader.load('/img/games/money_spin/arrow.png', (texture: Texture) => {
            const geometry = new PlaneGeometry(240, 200);
            const material = new MeshBasicMaterial({ map: texture, side: DoubleSide, transparent: true });
            const mesh = new Mesh(geometry, material);
            this._arrow.add(mesh);
            this._arrow.position.z = -1;
        });

        this.render();
    }

    spin(): void {
        if (this._arrowSpeed > 0) {
            return;
        }

        // randomize full speed, duration, and dropoff speed each spin
        this._slowDown = false;
        this._arrowSpeed = SpinnerComponent.MIN_TOP_SPEED + (Math.random() * SpinnerComponent.MAX_ADDITIONAL_TOP_SPEED);
        this._speedDropoff = SpinnerComponent.MIN_DROPOFF + (Math.random() * SpinnerComponent.MAX_ADDITIONAL_DROPOFF);
        const fullSpeedTime = SpinnerComponent.MIN_TOP_SPEED_TIME_MS + (Math.random() * SpinnerComponent.MAX_ADDITIONAL_TOP_SPEED_TIME_MS);

        setTimeout(() => {
            this._slowDown = true;
        }, fullSpeedTime);
    }


    private render(): void {
        this._animationId = requestAnimationFrame(() => { this.render(); });

        if (this._slowDown) {
            if (this._arrowSpeed !== 0) {
                this._arrowSpeed -= this._speedDropoff;

                if (this._arrowSpeed <= 0) {
                    this._arrowSpeed = 0;
                    const deg = _Math.radToDeg(this._arrow.rotation.z);
                    this.spinCompleted.next(this.getScore(deg));
                }
            }
        }

        this._arrow.rotateZ(this._arrowSpeed);

        this._renderer.render(this._scene, this._camera);
    }

    private getScore(deg: number): number {
        if (deg < 0) {
            deg = 360 + deg;
        }

        for (let i = 0; i < SpinnerComponent.SECTORS.length; i++) {
            const score = SpinnerComponent.SECTORS[i];

            if (deg <= score.minDegree) {
                return score.value;
            }
        }

        throw new Error('Unexpected result from spin!');
    }
}
