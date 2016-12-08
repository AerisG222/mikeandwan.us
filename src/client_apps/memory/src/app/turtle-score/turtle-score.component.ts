import { Component, Input } from '@angular/core';

import { IPlayer } from '../iplayer.model';

@Component({
    selector: 'app-turtle-score',
    templateUrl: './turtle-score.component.html',
    styleUrls: [ './turtle-score.component.css' ]
})
export class TurtleScoreComponent {
    @Input() player: IPlayer;
}
