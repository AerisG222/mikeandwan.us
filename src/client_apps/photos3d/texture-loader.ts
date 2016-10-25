import { ITextureInfo } from './itexture-info';

export class TextureLoader {
    private loader = new THREE.TextureLoader();

    loadTexture(url: string): Promise<ITextureInfo> {
        return new Promise((resolve, reject) => {
            this.loader.load(url, 
                texture => {
                    resolve({ url: url, texture: texture });
                },
                progress => { 
                    console.log(`downloading ${url}: ${progress.loaded}`);
                },
                error => {
                    reject(error);
                }
            );
        });
    }

    loadTextures(urls: Array<string>): Array<Promise<ITextureInfo>> {
        let list: Array<Promise<ITextureInfo>> = [];

        for(var i = 0; i < urls.length; i++) {
            list.push(this.loadTexture(urls[i]));
        }

        return list;
    }
}
