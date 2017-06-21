import {animation, style, animate} from '@angular/animations';

// https://www.yearofmoo.com/2017/06/new-wave-of-animation-features.html

export let fadeAnimation = animation([
        style({ opacity: '{{ from }}' }),
        animate('{{ time }}', style({ opacity: '{{ to }}' }))
    ],
    { params: { time: '1s' } });
