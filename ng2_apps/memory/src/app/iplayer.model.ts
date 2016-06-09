import { ICharacter } from './';

export interface IPlayer {
    character: ICharacter;
    isPlayersTurn?: boolean;
    score?: number;
}
