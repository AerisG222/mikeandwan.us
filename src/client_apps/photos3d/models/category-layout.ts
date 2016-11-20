import { Hexagon } from './hexagon';
import { LayoutPosition } from './layout-position';

export class CategoryLayout {
    constructor(public hexagon: Hexagon,
                public positions: LayoutPosition[][]) {

    }
}
