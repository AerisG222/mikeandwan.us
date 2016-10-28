import { TextureLoader } from './texture-loader';

export class Background {
    private static readonly TEXTURE_WATER = 'water.jpg';
    private static readonly TEXTURE_TREES = 'trees.jpg';
    private static readonly TEXTURE_NOISE = 'noise.png';

    private horizontalRepeat = 2;
    private scene: THREE.Scene;
    private size: string;
    private waterMesh: THREE.Mesh;
    private waterUniform: any = null;
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
        
        this.updateTextures(texturePromise);
    }
    
    setSize(size: string) {
        if(size == this.size) {
            return;
        }

        this.size = size;
        this.updateTextures(this.loadTextures());
    }

    render(clockDelta: number) {
        if(this.waterUniform != null) {
            this.waterUniform.time.value += clockDelta;
        }
    }

    private loadTextures() {
        return this.textureLoader.loadTextures([
            `/img/photos3d/${this.size}/${Background.TEXTURE_NOISE}`,
            `/img/photos3d/${this.size}/${Background.TEXTURE_TREES}`,
            `/img/photos3d/${this.size}/${Background.TEXTURE_WATER}`
        ]);
    }

    private updateTextures(texturePromises: Array<Promise<THREE.Texture>>)
    {
        Promise.all(texturePromises).then(textures => {
            let waterTexture: THREE.Texture = null;
            let noiseTexture: THREE.Texture = null;

            for(let texture of textures) {
                if(texture.name.indexOf(Background.TEXTURE_NOISE) > 0) {
                    noiseTexture = texture;
                }
                else if(texture.name.indexOf(Background.TEXTURE_TREES) > 0) {
                    this.updateTrees(texture);
                }
                else if(texture.name.indexOf(Background.TEXTURE_WATER) > 0) {
                    waterTexture = texture;
                }
            }

            this.updateWater(waterTexture, noiseTexture);
        }).catch(error => {
            console.error(`error getting textures: ${error}`);
        });
    }

    private updateTrees(texture: THREE.Texture) {
        if(this.treeMesh != null) {
            this.scene.remove(this.treeMesh);
        }

        let treeGeometry = new THREE.PlaneGeometry(texture.image.width * this.horizontalRepeat, texture.image.height);
        this.treeMesh = new THREE.Mesh(treeGeometry);
        this.treeMesh.position.y = texture.image.height / 2;

        texture.repeat.set(this.horizontalRepeat, 1);
        texture.wrapS = THREE.MirroredRepeatWrapping;

        this.treeMesh.material = new THREE.MeshBasicMaterial({ map: texture, side: THREE.DoubleSide });

        this.scene.add(this.treeMesh);
    }

    private updateWater(waterTexture: THREE.Texture, noiseTexture: THREE.Texture) {
        if(this.waterMesh != null) {
            this.scene.remove(this.waterMesh);
        }

        let waterGeometry = new THREE.PlaneGeometry(waterTexture.image.width * this.horizontalRepeat, waterTexture.image.height);
        this.waterMesh = new THREE.Mesh(waterGeometry);
        this.waterMesh.scale.y = -1;
        this.waterMesh.rotation.x = (Math.PI / 2);
        this.waterMesh.position.z = waterTexture.image.height / 2;

        waterTexture.repeat.set(this.horizontalRepeat, 1);
        waterTexture.wrapT = THREE.MirroredRepeatWrapping;
        waterTexture.wrapS = THREE.MirroredRepeatWrapping;
        
        noiseTexture.repeat.set(2, 1);
        noiseTexture.wrapT = THREE.MirroredRepeatWrapping;
        noiseTexture.wrapS = THREE.MirroredRepeatWrapping;

        this.waterUniform = {
            baseTexture: 	{ type: "t", value: waterTexture },
            baseSpeed: 		{ type: "f", value: 0.005 },
            noiseTexture: 	{ type: "t", value: noiseTexture },
            noiseScale:		{ type: "f", value: 0.1 },
            alpha: 			{ type: "f", value: 0.8 },
            time: 			{ type: "f", value: 1.0 }
        };

        let waterMaterial = new THREE.ShaderMaterial({
            uniforms: this.waterUniform,
            vertexShader:   document.getElementById( 'waterVertexShader'   ).textContent,
            fragmentShader: document.getElementById( 'waterFragmentShader' ).textContent
        });

        waterMaterial.side = THREE.DoubleSide;
        
        this.waterMesh.material = waterMaterial;

        this.scene.add(this.waterMesh);
    }
}
