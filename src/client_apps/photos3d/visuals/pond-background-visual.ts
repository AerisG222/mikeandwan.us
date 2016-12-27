import { ArgumentNullError } from '../models/argument-null-error';
import { DisposalService } from '../services/disposal-service';
import { IDisposable } from '../models/idisposable';
import { IVisual } from './ivisual';
import { TextureLoader } from '../services/texture-loader';
import { VisualContext } from '../models/visual-context';

export class PondBackgroundVisual extends THREE.Object3D implements IDisposable, IVisual {
    private static readonly TEXTURE_WATER = 'water.jpg';
    private static readonly TEXTURE_TREES = 'trees_blur20.jpg';
    private static readonly TEXTURE_NOISE = 'noise.png';

    private _horizontalRepeat = 2;
    private _isDisposed = false;
    private _textureLoader = new TextureLoader();
    private _treeMesh: THREE.Mesh;
    private _waterMesh: THREE.Mesh;
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
        return this._textureLoader.loadTextures([
            `/img/photos3d/${this._ctx.size}/${PondBackgroundVisual.TEXTURE_NOISE}`,
            `/img/photos3d/${this._ctx.size}/${PondBackgroundVisual.TEXTURE_TREES}`,
            `/img/photos3d/${this._ctx.size}/${PondBackgroundVisual.TEXTURE_WATER}`
        ]);
    }

    private updateTextures(texturePromises: Array<Promise<THREE.Texture>>) {
        Promise.all(texturePromises).then(textures => {
            let waterTexture: THREE.Texture = null;
            let noiseTexture: THREE.Texture = null;

            for (let texture of textures) {
                if (texture.name.indexOf(PondBackgroundVisual.TEXTURE_NOISE) > 0) {
                    noiseTexture = texture;
                } else if (texture.name.indexOf(PondBackgroundVisual.TEXTURE_TREES) > 0) {
                    this.updateTrees(texture);
                } else if (texture.name.indexOf(PondBackgroundVisual.TEXTURE_WATER) > 0) {
                    waterTexture = texture;
                }
            }

            this.updateWater(waterTexture, noiseTexture);
        }).catch(error => {
            console.error(`error getting textures: ${error}`);
        });
    }

    private updateTrees(texture: THREE.Texture) {
        if (this._treeMesh != null) {
            this.remove(this._treeMesh);
        }

        let treeGeometry = new THREE.PlaneGeometry(texture.image.width * this._horizontalRepeat, texture.image.height);
        this._treeMesh = new THREE.Mesh(treeGeometry);
        this._treeMesh.position.y = texture.image.height / 2;

        texture.minFilter = THREE.LinearFilter;
        texture.repeat.set(this._horizontalRepeat, 1);
        texture.wrapS = THREE.MirroredRepeatWrapping;

        this._treeMesh.material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });

        this.add(this._treeMesh);
    }

    private updateWater(waterTexture: THREE.Texture, noiseTexture: THREE.Texture) {
        if (this._waterMesh != null) {
            this.remove(this._waterMesh);
        }

        let waterGeometry = new THREE.PlaneGeometry(waterTexture.image.width * this._horizontalRepeat, waterTexture.image.height);
        this._waterMesh = new THREE.Mesh(waterGeometry);
        this._waterMesh.scale.y = -1;
        this._waterMesh.rotation.x = (Math.PI / 2);
        this._waterMesh.position.z = waterTexture.image.height / 2;

        waterTexture.minFilter = THREE.LinearFilter;
        waterTexture.repeat.set(this._horizontalRepeat, 1);
        waterTexture.wrapT = THREE.MirroredRepeatWrapping;
        waterTexture.wrapS = THREE.MirroredRepeatWrapping;

        noiseTexture.minFilter = THREE.LinearFilter;
        noiseTexture.repeat.set(2, 1);
        noiseTexture.wrapT = THREE.MirroredRepeatWrapping;
        noiseTexture.wrapS = THREE.MirroredRepeatWrapping;

        this._waterUniform = {
            baseTexture: 	{ type: 't', value: waterTexture },
            baseSpeed: 		{ type: 'f', value: 0.005 },
            noiseTexture: 	{ type: 't', value: noiseTexture },
            noiseScale:		{ type: 'f', value: 0.1 },
            alpha: 			{ type: 'f', value: 0.8 },
            time: 			{ type: 'f', value: 1.0 }
        };

        let waterMaterial = new THREE.ShaderMaterial({
            uniforms: this._waterUniform,
            vertexShader:   document.getElementById( 'waterVertexShader'   ).textContent,
            fragmentShader: document.getElementById( 'waterFragmentShader' ).textContent
        });

        waterMaterial.side = THREE.DoubleSide;

        this._waterMesh.material = waterMaterial;

        this.add(this._waterMesh);
    }
}
