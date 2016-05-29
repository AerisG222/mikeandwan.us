import { ICharacter } from './ICharacter';

export interface IPlayer {
    character : ICharacter;
    isPlayersTurn? : boolean;
    score? : number;
}
