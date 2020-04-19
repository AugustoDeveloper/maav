import { animation, trigger,transition, animate, style, state } from '@angular/animations';

export const transAnimation = animation([
    trigger('fadeInOut', [
        state('void', style({
            opacity: 0
        })),
        transition('void <=> *', animate(1000)),
        ]),
    trigger('balloonEffect', [
        state('initial', style({
            backgroundColor: 'green',
            transform: 'scale(1)'
        })),
        state('final', style({
            backgroundColor: 'red',
            transform: 'scale(1.5)'
        })),
        transition('final=>initial', animate('1000ms')),
        transition('initial=>final', animate('1500ms'))
        ])
]);