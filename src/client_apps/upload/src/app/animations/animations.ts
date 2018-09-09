import { trigger, transition, animate, keyframes, style } from '@angular/animations';

export const listItemAnimation = trigger('listItemAnimation', [
    transition(':enter', [
        animate('500ms', keyframes([
            style({
                'background-color': '*',
            }),
            style({
                'background-color': '#78cc78',
            }),
            style({
                'background-color': '*',
            })
        ]))
    ]),
    transition(':leave', [
        animate('500ms', keyframes([
            style({
                'background-color': '*',
            }),
            style({
                'background-color': '#d71c16',
                opacity: 0.5
            }),
            style({
                'background-color': '*',
                opacity: 0
            })
        ]))
    ])
]);
