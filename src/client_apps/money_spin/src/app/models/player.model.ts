import { Character } from './character.model';

export class Player {
    private playerScore = 0;

    constructor(public character: Character) {

    }

    get score(): number {
        return this.playerScore;
    }

    resetScore(): void {
        this.playerScore = 0;
    }

    addDollarAmount(value: number): void {
        this.playerScore += value;
    }
}
