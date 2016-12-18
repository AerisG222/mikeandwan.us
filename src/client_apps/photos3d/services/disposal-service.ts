// http://stackoverflow.com/questions/22565737/cleanup-threejs-scene-leak
export class DisposalService {
    public dispose(obj: any) {
        if (obj !== null) {
            for (let i = 0; i < obj.children.length; i++) {
                this.dispose(obj.children[i]);
            }

            if (obj.geometry) {
                obj.geometry.dispose();
                obj.geometry = undefined;
            }

            if (obj.material) {
                if (obj.material.map) {
                    obj.material.map.dispose();
                    obj.material.map = undefined;
                }

                obj.material.dispose();
                obj.material = undefined;
            }
        }

        obj = undefined;
    }
}
