import { Vector2 } from 'three';

export class ScaleCalculator {
    scale(maxWidth: number, maxHeight: number, actualWidth: number, actualHeight: number): Vector2 {
        let imgAspect = actualWidth / actualHeight;
        let viewAspect = maxWidth / maxHeight;

        if (viewAspect > imgAspect) {
            let width = actualWidth * (maxHeight / actualHeight);
            let height = maxHeight;

            return new Vector2(width, height);
        } else {
            let width = maxWidth;
            let height = actualHeight * (maxWidth / actualWidth);

            return new Vector2(width, height);
        }
    }
}
