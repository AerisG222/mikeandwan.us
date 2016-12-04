export class FrustrumCalculator {
    calculateBounds(camera: THREE.PerspectiveCamera, z: number): THREE.Vector2 {
        // http://gamedev.stackexchange.com/questions/96317/determining-view-boundaries-based-on-z-position-when-using-a-perspective-project
        let height = 2 * Math.tan(camera.fov * 0.5 * (Math.PI / 180)) * (camera.position.z - z);
        let width = height * camera.aspect;

        return new THREE.Vector2(width, height);
    }

    calculateZForFullFrame(camera: THREE.PerspectiveCamera): number {
        let angleDeg = 90 - (camera.fov / 2);
        let angleRad = angleDeg * Math.PI / 180;
        let len = Math.tan(angleRad) * camera.position.y;

        return camera.position.z - len;
    }
}
