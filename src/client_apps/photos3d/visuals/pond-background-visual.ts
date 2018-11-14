import { Group, Mesh, Texture, PlaneGeometry, LinearFilter, MirroredRepeatWrapping, MeshBasicMaterial,
         DoubleSide, ShaderMaterial
       } from 'three';

import { ArgumentNullError } from '../models/argument-null-error';
import { DisposalService } from '../services/disposal-service';
import { IDisposable } from '../models/idisposable';
import { IVisual } from './ivisual';
import { TextureLoader } from '../services/texture-loader';
import { VisualContext } from '../models/visual-context';

const textureNoiseMd = require('../img/md/noise.png');
const textureTreeseMd = require('../img/md/trees_blur20.jpg');
const textureWaterMd = require('../img/md/water.jpg');

const textureNoiseLg = require('../img/lg/noise.png');
const textureTreeseLg = require('../img/lg/trees.jpg');
const textureWaterLg = require('../img/lg/water.jpg');

export class PondBackgroundVisual extends Group implements IDisposable, IVisual {
    private _horizontalRepeat = 2;
    private _isDisposed = false;
    private _textureLoader = new TextureLoader();
    private _treeMesh: Mesh;
    private _waterMesh: Mesh;
    private _waterUniform: any = null;

    constructor(private _disposalService: DisposalService,
                private _ctx: VisualContext) {
        super();

        if (_disposalService == null) {
            throw new ArgumentNullError('_disposalService');
        }

        if (_ctx == null) {
            throw new ArgumentNullError('_ctx');
        }
    }

    init() {
        let texturePromise = this.loadTextures();

        this.updateTextures(texturePromise);
    }

    show() {
        if (this._treeMesh != null) {
            this.add(this._treeMesh);
        }

        if (this._waterMesh != null) {
            this.add(this._waterMesh);
        }
    }

    hide() {
        if (this._treeMesh != null) {
            this.remove(this._treeMesh);
        }

        if (this._waterMesh != null) {
            this.remove(this._waterMesh);
        }
    }

    render(clockDelta: number, elapsed: number) {
        if (this._isDisposed) {
            return;
        }

        if (this._waterUniform != null) {
            this._waterUniform.time.value += clockDelta;
        }
    }

    dispose(): void {
        if (this._isDisposed) {
            return;
        }

        this._isDisposed = true;

        this.hide();

        this._disposalService.dispose(this._treeMesh);
        this._treeMesh = null;

        this._disposalService.dispose(this._waterMesh);
        this._waterMesh = null;
    }

    private loadTextures() {
        if (this._ctx.size === 'md') {
            return this._textureLoader.loadTextures([
                textureNoiseMd,
                textureTreeseMd,
                textureWaterMd
            ]);
        } else {
            return this._textureLoader.loadTextures([
                textureNoiseLg,
                textureTreeseLg,
                textureWaterLg
            ]);
        }
    }

    private updateTextures(texturePromises: Array<Promise<Texture>>) {
        Promise.all(texturePromises).then(textures => {
            let waterTexture: Texture = null;
            let noiseTexture: Texture = null;

            for (let texture of textures) {
                if (texture.name.indexOf('noise') > 0) {
                    noiseTexture = texture;
                } else if (texture.name.indexOf('tree') > 0) {
                    this.updateTrees(texture);
                } else if (texture.name.indexOf('water') > 0) {
                    waterTexture = texture;
                }
            }

            this.updateWater(waterTexture, noiseTexture);
        }).catch(error => {
            console.error(`error getting textures: ${error}`);
        });
    }

    private updateTrees(texture: Texture) {
        if (this._treeMesh != null) {
            this.remove(this._treeMesh);
        }

        let treeGeometry = new PlaneGeometry(texture.image.width * this._horizontalRepeat, texture.image.height);
        this._treeMesh = new Mesh(treeGeometry);
        this._treeMesh.position.y = texture.image.height / 2;

        texture.minFilter = LinearFilter;
        texture.repeat.set(this._horizontalRepeat, 1);
        texture.wrapS = MirroredRepeatWrapping;

        this._treeMesh.material = new MeshBasicMaterial({ map: texture, side: DoubleSide });

        this.add(this._treeMesh);
    }

    private updateWater(waterTexture: Texture, noiseTexture: Texture) {
        if (this._waterMesh != null) {
            this.remove(this._waterMesh);
        }

        let waterGeometry = new PlaneGeometry(waterTexture.image.width * this._horizontalRepeat, waterTexture.image.height);
        this._waterMesh = new Mesh(waterGeometry);
        this._waterMesh.scale.y = -1;
        this._waterMesh.rotation.x = (Math.PI / 2);
        this._waterMesh.position.z = waterTexture.image.height / 2;

        waterTexture.minFilter = LinearFilter;
        waterTexture.repeat.set(this._horizontalRepeat, 1);
        waterTexture.wrapT = MirroredRepeatWrapping;
        waterTexture.wrapS = MirroredRepeatWrapping;

        noiseTexture.minFilter = LinearFilter;
        noiseTexture.repeat.set(2, 1);
        noiseTexture.wrapT = MirroredRepeatWrapping;
        noiseTexture.wrapS = MirroredRepeatWrapping;

        this._waterUniform = {
            baseTexture: 	{ type: 't', value: waterTexture },
            baseSpeed: 		{ type: 'f', value: 0.005 },
            noiseTexture: 	{ type: 't', value: noiseTexture },
            noiseScale:		{ type: 'f', value: 0.1 },
            alpha: 			{ type: 'f', value: 0.8 },
            time: 			{ type: 'f', value: 1.0 }
        };

        let waterMaterial = new ShaderMaterial({
            uniforms: this._waterUniform,
            vertexShader:   document.getElementById( 'waterVertexShader'   ).textContent,
            fragmentShader: document.getElementById( 'waterFragmentShader' ).textContent
        });

        waterMaterial.side = DoubleSide;

        this._waterMesh.material = waterMaterial;

        this.add(this._waterMesh);
    }
}
