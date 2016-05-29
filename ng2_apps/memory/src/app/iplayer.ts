import { ICharacter } from './icharacter';

export interface IPlayer {
    character : ICharacter;
    isPlayersTurn? : boolean;
    score? : number;
}
