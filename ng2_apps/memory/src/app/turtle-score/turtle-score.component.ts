import { Component, Input } from '@angular/core';
import { NgClass } from '@angular/common';
import { IPlayer } from '../iplayer';

@Component({
  moduleId: module.id,
  selector: 'app-turtle-score',
  directives: [ NgClass ],
  templateUrl: 'turtle-score.component.html',
  styleUrls: ['turtle-score.component.css']
})
export class TurtleScoreComponent {
    @Input() player : IPlayer;
}
