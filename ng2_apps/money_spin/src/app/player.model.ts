import { Character } from './character.model';

export class Player {
    private _score = 0;

    constructor(public character: Character) {

    }

    get score(): number {
        return this._score;
    }

    resetScore(): void {
        this._score = 0;
    }

    addDollarAmount(value: number): void {
        this._score += value;
    }
}
