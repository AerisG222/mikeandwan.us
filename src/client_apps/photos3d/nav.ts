export class Nav {
    navDiv: HTMLDivElement;
    
    constructor() {

    }

    init() {
        this.navDiv = document.createElement('div');
        this.navDiv.id = 'nav';
        this.navDiv.style.position = 'absolute';
        this.navDiv.style.bottom = '0';
        this.navDiv.style.opacity = '0.7';
        this.navDiv.style.backgroundColor = '#333';
        this.navDiv.style.height = '24px';
        this.navDiv.style.width = '100%';

        document.body.appendChild(this.navDiv);
    }
}
