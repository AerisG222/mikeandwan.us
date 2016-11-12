import { CategoryLayout } from './category-layout';
import { Hexagon } from './hexagon';
import { LayoutPosition } from './layout-position';

export class CategoryLayoutCalculator {
    private static readonly MAX_EDGE_LENGTH = 80;
    private static readonly MIN_EDGE_LENGTH = 2;
    private static readonly BORDER_WIDTH = 4;
    private static readonly FRUSTRUM_MARGIN_SIDES = 100;
    private static readonly FRUSTRUM_MARGIN_TOP_BOTTOM = 50;
    private static readonly CATEGORY_PADDING = 6;
    private static readonly TOTAL_PAD = CategoryLayoutCalculator.BORDER_WIDTH + CategoryLayoutCalculator.CATEGORY_PADDING;

    private heightRatio: number;
    private widthRatio: number;

    constructor(private frustrumHeight: number, private frustrumWidth: number) {
        
    }

    calculate(categoryCount: number): CategoryLayout {
        let maxHex = this.getMaxHexagon(categoryCount);
        let layout: LayoutPosition[][] = [];
        let top = this.frustrumHeight - CategoryLayoutCalculator.FRUSTRUM_MARGIN_TOP_BOTTOM - maxHex.centerToMidEdgeLength;
        let left = -(this.frustrumWidth - CategoryLayoutCalculator.FRUSTRUM_MARGIN_SIDES - maxHex.centerToVertexLength) / 2;
        let maxRows = this.frustrumHeight / ((2 * maxHex.centerToMidEdgeLength) + CategoryLayoutCalculator.TOTAL_PAD);
        let maxCols = this.frustrumWidth / ((2 * maxHex.centerToMidEdgeLength) + CategoryLayoutCalculator.TOTAL_PAD);
        let totalSlots = maxRows * maxCols;
        let currIdx = 0;

        let itemWidth = 2 * maxHex.centerToVertexLength + CategoryLayoutCalculator.CATEGORY_PADDING;
        let itemHeight = 2 * maxHex.centerToMidEdgeLength + CategoryLayoutCalculator.CATEGORY_PADDING;

        // TODO: make layout appealing - not top left to bottom right
        for(let i = 0; i < maxRows; i++) {
            layout[i] = [];
            let even = i % 2;

            for(let j = 0; j < maxCols; j++) {
                let pos = new LayoutPosition();
                let x = left + (j * itemWidth) + (even * maxHex.centerToVertexLength);
                let y = top - (i * itemWidth) + (even * maxHex.centerToMidEdgeLength);
                pos.center = new THREE.Vector2(x, y);
                pos.index = currIdx++;

                layout[i][j] = pos;
            }
        }

        return new CategoryLayout(maxHex, layout);
    }

    private getMaxHexagon(categoryCount: number) {
        let height = this.frustrumHeight - (2 * CategoryLayoutCalculator.FRUSTRUM_MARGIN_SIDES);
        let width = this.frustrumWidth - (2 * CategoryLayoutCalculator.FRUSTRUM_MARGIN_TOP_BOTTOM);
        let usableArea = height * width;
        let tolerance = 0.10 * usableArea;

        for(let edge = CategoryLayoutCalculator.MAX_EDGE_LENGTH; edge >= 10; edge -= 10 ) {
            let effectiveEdge = edge 
                              + CategoryLayoutCalculator.BORDER_WIDTH
                              + CategoryLayoutCalculator.CATEGORY_PADDING ;

            let hex = new Hexagon(edge);
            let totalArea = hex.area() * categoryCount;

            if(totalArea < usableArea) {
                return hex;
            }
        }

        return new Hexagon(CategoryLayoutCalculator.MIN_EDGE_LENGTH);
    }
}
