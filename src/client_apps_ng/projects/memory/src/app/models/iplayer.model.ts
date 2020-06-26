import { ICharacter } from './icharacter.model';

export interface IPlayer {
    character: ICharacter;
    isPlayersTurn: boolean;
    score: number;
}
