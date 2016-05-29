import { bootstrap } from '@angular/platform-browser-dynamic';
import { ByteCounter } from './ByteCounter';

bootstrap(ByteCounter)
    .catch(err => console.error(err));
