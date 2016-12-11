export class TextureLoader {
    // TODO: typings do not recognize the other loading functions (progress+error), so we
    //       mark the loader as any to avoid associated errors today.  once the typings
    //       support this, remove the any typing.
    private loader: any = new THREE.TextureLoader();

    loadTexture(url: string): Promise<THREE.Texture> {
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

    loadTextures(urls: Array<string>): Array<Promise<THREE.Texture>> {
        let list: Array<Promise<THREE.Texture>> = [];

        for (let i = 0; i < urls.length; i++) {
            list.push(this.loadTexture(urls[i]));
        }

        return list;
    }
}