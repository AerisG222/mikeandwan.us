import { Texture, TextureLoader as _TextureLoader } from 'three';

export class TextureLoader {
    private loader = new _TextureLoader();

    loadTexture(url: string): Promise<Texture> {
        return new Promise((resolve, reject) => {
            this.loader.load(url,
                texture => {
                    texture.name = url;
                    resolve(texture);
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

    loadTextures(urls: Array<string>): Array<Promise<Texture>> {
        let list: Array<Promise<Texture>> = [];

        for (let i = 0; i < urls.length; i++) {
            list.push(this.loadTexture(urls[i]));
        }

        return list;
    }
}
