import { Photos3D } from './photos3d';

let app = new Photos3D();
app.run();

Mousetrap.bind('space', e => { app.togglePause(); });
Mousetrap.bind('b', e => { app.toggleBackground(); });
Mousetrap.bind('x', e => { app.toggleAxisHelper(); });
Mousetrap.bind('s', e => { app.stepBackward(); });
Mousetrap.bind('w', e => { app.stepForward(); });
Mousetrap.bind('a', e => { app.strafeLeft(); });
Mousetrap.bind('d', e => { app.strafeRight(); });
