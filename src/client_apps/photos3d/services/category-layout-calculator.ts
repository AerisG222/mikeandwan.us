import * as THREE from 'three';

import { CategoryLayout } from '../models/category-layout';
import { Hexagon } from '../models/hexagon';
import { LayoutPosition } from '../models/layout-position';

export class CategoryLayoutCalculator {
    private static readonly MAX_EDGE_LENGTH = 80;
    private static readonly MIN_EDGE_LENGTH = 2;
    private static readonly BORDER_WIDTH = 4;
    private static readonly FRUSTRUM_MARGIN_SIDES = 100;
    private static readonly FRUSTRUM_MARGIN_TOP_BOTTOM = 50;
    private static readonly CATEGORY_PADDING = 6;
    private static readonly TOTAL_PAD = CategoryLayoutCalculator.BORDER_WIDTH + CategoryLayoutCalculator.CATEGORY_PADDING;

    constructor(private frustrumHeight: number, private frustrumWidth: number) {

    }

    calculate(categoryCount: number): CategoryLayout {
        let layout: LayoutPosition[][] = [];
        let maxHex = this.getMaxHexagon(categoryCount);
        let itemWidth = (2 * maxHex.centerToVertexLength) + (2 * CategoryLayoutCalculator.CATEGORY_PADDING);
        let horizontalCenterToCenterDistance = itemWidth + maxHex.centerToVertexLength;

        let top = this.frustrumHeight - CategoryLayoutCalculator.FRUSTRUM_MARGIN_TOP_BOTTOM - maxHex.centerToMidEdgeLength;
        let left = -(this.frustrumWidth - CategoryLayoutCalculator.FRUSTRUM_MARGIN_SIDES - maxHex.centerToVertexLength) / 2;

        let currIdx = 0;

        let maxRows = (this.frustrumHeight - (2 * CategoryLayoutCalculator.FRUSTRUM_MARGIN_TOP_BOTTOM))
                    / (maxHex.centerToMidEdgeLength + CategoryLayoutCalculator.TOTAL_PAD);
        let maxCols = (this.frustrumWidth - (2 * CategoryLayoutCalculator.FRUSTRUM_MARGIN_SIDES)) / horizontalCenterToCenterDistance;

        // TODO: make layout appealing - not top left to bottom right
        for (let i = 0; i < maxRows; i++) {
            layout[i] = [];
            let even = i % 2;

            let x = left + (even * (1.5 * maxHex.centerToVertexLength + CategoryLayoutCalculator.CATEGORY_PADDING));
            let y = top - (i * CategoryLayoutCalculator.BORDER_WIDTH) - (i * maxHex.centerToMidEdgeLength);

            for (let j = 0; j < maxCols; j++) {
                let pos = new LayoutPosition();
                pos.center = new THREE.Vector2(x, y);
                pos.index = currIdx++;

                x += horizontalCenterToCenterDistance;

                layout[i][j] = pos;
            }
        }

        return new CategoryLayout(maxHex, layout);
    }

    private getMaxHexagon(categoryCount: number) {
        let height = this.frustrumHeight - (2 * CategoryLayoutCalculator.FRUSTRUM_MARGIN_SIDES);
        let width = this.frustrumWidth - (2 * CategoryLayoutCalculator.FRUSTRUM_MARGIN_TOP_BOTTOM);
        let usableArea = height * width;

        for (let edge = CategoryLayoutCalculator.MAX_EDGE_LENGTH; edge >= 10; edge -= 10 ) {
            let hex = new Hexagon(edge);
            let totalArea = hex.area() * categoryCount;

            if (totalArea < usableArea) {
                return hex;
            }
        }

        return new Hexagon(CategoryLayoutCalculator.MIN_EDGE_LENGTH);
    }
}
