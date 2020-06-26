import { Component, Input } from '@angular/core';

import { IPlayer } from '../models/iplayer.model';

@Component({
    selector: 'app-turtle-score',
    templateUrl: './turtle-score.component.html',
    styleUrls: [ './turtle-score.component.scss' ]
})
export class TurtleScoreComponent {
    @Input() player?: IPlayer;
}
