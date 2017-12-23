import { animate, animation, style, useAnimation } from '@angular/animations';

// https://www.yearofmoo.com/2017/06/new-wave-of-animation-features.html

export let fadeAnimation = animation([
        style({ opacity: '{{ from }}' }),
        animate('{{ time }}', style({ opacity: '{{ to }}' }))
    ],
    { params: { time: '1s' } });

export let fadeIn = useAnimation(fadeAnimation, { params: { from: 0, to: 1, time: '420ms' }});
export let fadeOut = useAnimation(fadeAnimation, { params: { from: 1, to: 0, time: '420ms' }});
