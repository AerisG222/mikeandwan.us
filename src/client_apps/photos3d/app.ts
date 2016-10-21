import { Photos3D } from './photos3d';

let app = new Photos3D();
app.run();

Mousetrap.bind("space", e => { app.togglePause() });
