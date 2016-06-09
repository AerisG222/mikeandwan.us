export class FilterSettings {
    grayscale = 0;
    sepia = 0;
    blur = 0;
    saturate = 100;
    brightness = 100;
    contrast = 100;
    hue = 0;
    invert = 0;

    get styleValue(): string {
        let style: Array<string> = [];

        if (this.grayscale > 0) {
            style.push('grayscale(' + this.grayscale + '%)');
        }
        if (this.sepia > 0) {
            style.push('sepia(' + this.sepia + '%)');
        }
        if (this.blur > 0) {
            style.push('blur(' + this.blur + 'px)');
        }
        if (this.saturate !== 100) {
            style.push('saturate(' + this.saturate + '%)');
        }
        if (this.brightness !== 100) {
            style.push('brightness(' + this.brightness + '%)');
        }
        if (this.contrast !== 100) {
            style.push('contrast(' + this.contrast + '%)');
        }
        if (this.hue > 0) {
            style.push('hue-rotate(' + this.hue + 'deg)');
        }
        if (this.invert > 0) {
            style.push('invert(' + this.invert + '%)');
        }

        if (style.length === 0) {
            return '';
        }

        return style.join(' ');
    }
}
