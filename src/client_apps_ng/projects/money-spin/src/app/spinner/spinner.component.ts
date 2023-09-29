import { Component, ElementRef, EventEmitter, Output, OnInit, OnDestroy } from '@angular/core';

import { Scene, OrthographicCamera, Renderer, Group, WebGLRenderer, TextureLoader, Texture,
         PlaneGeometry, Mesh, MeshBasicMaterial, DoubleSide, MathUtils
       } from 'three';

import { BoardSector } from '../models/board-sector.model';

@Component({
    selector: 'app-spinner',
    templateUrl: './spinner.component.html',
    styleUrls: [ './spinner.component.scss' ]
})
export class SpinnerComponent implements OnInit, OnDestroy {
    private static readonly sectors = [
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
    private static readonly minTopSpeed = 0.2;
    private static readonly maxAdditionalTopSpeed = 0.4;
    private static readonly minDropoff = .002;
    private static readonly maxAdditionalDropoff = .003;
    private static readonly minTopSpeedTimeMs = 511;
    private static readonly maxAdditionalTopSpeedTimeMs = 1511;

    @Output() spinCompleted = new EventEmitter<number>();

    el?: HTMLDivElement;
    scene?: Scene;
    camera?: OrthographicCamera;
    renderer?: Renderer;
    board = new Group();
    arrow = new Group();
    arrowSpeed = 0;
    speedDropoff = 0;
    slowDown = true;
    animationId?: number;

    constructor(private elRef: ElementRef) {

    }

    ngOnInit(): void {
        this.el = this.elRef.nativeElement.querySelector('div');
        this.prepareScene();
    }

    ngOnDestroy(): void {
        if (this.animationId) {
            cancelAnimationFrame(this.animationId);
        }

        this.camera = undefined;
        this.scene = undefined;

        // kill the rendering node
        while (!!this.el && this.el.hasChildNodes()) {
            const child = this.el.firstChild;

            if (child) {
                this.el.removeChild(child);
            }
        }
    }

    prepareScene(): void {
        if (!this.el) {
            throw new Error('Unable to prepare scene as root element is not defined');
        }

        // scene
        this.scene = new Scene();

        // renderer
        this.renderer = new WebGLRenderer({ antialias: true, alpha: true });
        this.renderer.setSize(640, 498);
        this.el.appendChild(this.renderer.domElement);

        // camera
        this.camera = new OrthographicCamera(320, -320, 250, -250, 0.1, 1000);
        this.camera.position.set(0, 0, -10);
        this.camera.lookAt(this.scene.position);
        this.scene.add(this.camera);

        this.board = new Group();
        this.scene.add(this.board);

        this.arrow = new Group();
        this.scene.add(this.arrow);

        const loader = new TextureLoader();
        loader.load('/js/money-spin/assets/board.png', (texture: Texture) => {
            const geometry = new PlaneGeometry(640, 498);
            const material = new MeshBasicMaterial({ map: texture, side: DoubleSide });
            const mesh = new Mesh(geometry, material);
            this.board.add(mesh);
        });
        loader.load('/js/money-spin/assets/arrow.png', (texture: Texture) => {
            const geometry = new PlaneGeometry(240, 200);
            const material = new MeshBasicMaterial({ map: texture, side: DoubleSide, transparent: true });
            const mesh = new Mesh(geometry, material);
            this.arrow.add(mesh);
            this.arrow.position.z = -1;
        });

        this.render();
    }

    spin(): void {
        if (this.arrowSpeed > 0) {
            return;
        }

        // randomize full speed, duration, and dropoff speed each spin
        this.slowDown = false;
        this.arrowSpeed = SpinnerComponent.minTopSpeed + (Math.random() * SpinnerComponent.maxAdditionalTopSpeed);
        this.speedDropoff = SpinnerComponent.minDropoff + (Math.random() * SpinnerComponent.maxAdditionalDropoff);
        const fullSpeedTime = SpinnerComponent.minTopSpeedTimeMs + (Math.random() * SpinnerComponent.maxAdditionalTopSpeedTimeMs);

        setTimeout(() => {
            this.slowDown = true;
        }, fullSpeedTime);
    }


    private render(): void {
        this.animationId = requestAnimationFrame(() => { this.render(); });

        if (this.slowDown) {
            if (this.arrowSpeed !== 0) {
                this.arrowSpeed -= this.speedDropoff;

                if (this.arrowSpeed <= 0) {
                    this.arrowSpeed = 0;
                    const deg = MathUtils.radToDeg(this.arrow.rotation.z);
                    this.spinCompleted.next(this.getScore(deg));
                }
            }
        }

        this.arrow.rotateZ(this.arrowSpeed);

        if (!!this.renderer && !!this.scene && !!this.camera) {
            this.renderer.render(this.scene, this.camera);
        }
    }

    private getScore(deg: number): number {
        if (deg < 0) {
            deg = 360 + deg;
        }

        for (const score of SpinnerComponent.sectors) {
            if (deg <= score.minDegree) {
                return score.value;
            }
        }

        throw new Error('Unexpected result from spin!');
    }
}
