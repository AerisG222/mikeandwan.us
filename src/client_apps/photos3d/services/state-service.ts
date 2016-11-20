import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';
import { ActiveStatus } from '../models/active-status';

export class StateService {
    private scene: THREE.Scene;
    private camera: THREE.PerspectiveCamera;
    
    private activeNavSubject = new Subject<ActiveStatus>();
    private temporalNavSubject = new Subject<string>();
    private mouseoverSubject = new Subject<Array<THREE.Intersection>>();

    constructor(scene: THREE.Scene,
                camera: THREE.PerspectiveCamera) {
        if(scene == null) {
            throw new Error('scene must be defined');
        }

        if(camera == null) {
            throw new Error('camera must be defined');
        }

        this.scene = scene;
        this.camera = camera;
    }

    get Scene() {
        return this.scene;
    }

    get Camera() {
        return this.camera;
    }

    get ActiveNavObservable() {
        return this.activeNavSubject.asObservable();
    }

    get TemporalNavObservable() {
        return this.temporalNavSubject.asObservable();
    }

    get MouseoverObservable() {
        return this.mouseoverSubject.asObservable();
    }

    updateTemporalNav(category: string) {
        this.temporalNavSubject.next(category);
    }

    updateActiveNav(year: number, category?: string) {
        this.activeNavSubject.next(new ActiveStatus(year, category));
    }

    updateMouseover(intersections: Array<THREE.Intersection>) {
        this.updateTemporalNav(null);
        this.mouseoverSubject.next(intersections);
    }
}
