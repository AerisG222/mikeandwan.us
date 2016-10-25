import { TextureLoader } from './texture-loader';
import { ITextureInfo } from './itexture-info';

export class Background {
    private static readonly TEXTURE_WATER = 'water.jpg';
    private static readonly TEXTURE_TREES = 'trees.jpg';
    private static readonly TEXTURE_NOISE = 'noise.png';

    private scene: THREE.Scene;
    private size: string;
    private waterMesh: THREE.Mesh;
    private treeMesh: THREE.Mesh;
    private textureLoader = new TextureLoader();

    constructor(scene: THREE.Scene, size: string) {
        if(scene == null) {
            throw new Error('scene should not be null');
        }

        if(size == null) {
            throw new Error('size should not be null');
        }

        this.scene = scene;
        this.size = size;
    }

    init() {
        var texturePromise = this.loadTextures();

        let waterPlane = new THREE.PlaneGeometry(6000, 1242);
        this.waterMesh = new THREE.Mesh(waterPlane);
        this.waterMesh.scale.y = -1;
        this.waterMesh.position.y = -60;
        this.waterMesh.rotation.x = (Math.PI / 2) + .1;
        this.scene.add(this.waterMesh);
        
        let treePlane = new THREE.PlaneGeometry(6000, 1012);
        this.treeMesh = new THREE.Mesh(treePlane);
        this.treeMesh.position.z = -280;
        this.treeMesh.position.y = 500;
        this.scene.add(this.treeMesh);

        this.updateTextures(texturePromise);
    }
    
    setSize(size: string) {
        if(size == this.size) {
            return;
        }

        this.size = size;
        this.updateTextures(this.loadTextures());
    }

    private loadTextures() {
        return this.textureLoader.loadTextures([
            `/img/photos3d/${this.size}/${Background.TEXTURE_NOISE}`,
            `/img/photos3d/${this.size}/${Background.TEXTURE_TREES}`,
            `/img/photos3d/${this.size}/${Background.TEXTURE_WATER}`
        ]);
    }

    private updateTextures(texturePromises: Array<Promise<ITextureInfo>>)
    {
        Promise.all(texturePromises).then(textures => {
            let waterTexture: THREE.Texture = null;
            let noiseTexture: THREE.Texture = null;

            for(let i = 0; i < textures.length; i++) {
                let ti = textures[i];

                if(ti.url.indexOf(Background.TEXTURE_NOISE) > 0) {
                    noiseTexture = ti.texture;
                }
                else if(ti.url.indexOf(Background.TEXTURE_TREES) > 0) {
                    this.updateTrees(ti.texture);
                }
                else if(ti.url.indexOf(Background.TEXTURE_WATER) > 0) {
                    waterTexture = ti.texture;
                }
            }

            this.updateWater(waterTexture, noiseTexture);
        }).catch(error => {
            console.error(`error getting textures: ${error}`);
        });
    }

    private updateTrees(texture: THREE.Texture) {
        texture.wrapT = THREE.MirroredRepeatWrapping;
        texture.wrapS = THREE.MirroredRepeatWrapping;
        texture.repeat.set(2, 1);
        this.treeMesh.material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });
    }

    private updateWater(waterTexture: THREE.Texture, noiseTexture: THREE.Texture) {
        waterTexture.wrapT = THREE.MirroredRepeatWrapping;
        waterTexture.wrapS = THREE.MirroredRepeatWrapping;
        waterTexture.repeat.set(2, 1);

        noiseTexture.wrapT = THREE.MirroredRepeatWrapping;
        noiseTexture.wrapS = THREE.MirroredRepeatWrapping;
        noiseTexture.repeat.set(2, 1);

        let waterUniform = {
            baseTexture: 	{ type: "t", value: waterTexture },
            baseSpeed: 		{ type: "f", value: 1.15 },
            noiseTexture: 	{ type: "t", value: noiseTexture },
            noiseScale:		{ type: "f", value: 0.2 },
            alpha: 			{ type: "f", value: 0.8 },
            time: 			{ type: "f", value: 1.0 }
        };

        let waterMaterial = new THREE.ShaderMaterial({
            uniforms: waterUniform,
            vertexShader:   document.getElementById( 'vertexShader'   ).textContent,
            fragmentShader: document.getElementById( 'fragmentShader' ).textContent
        });

        waterMaterial.side = THREE.DoubleSide;

        this.waterMesh.material = waterMaterial;
    }
}
