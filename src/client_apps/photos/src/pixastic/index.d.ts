interface PixasticStatic {
    process(img: HTMLElement, operation: string, options: any): any;
    revert(img: HTMLElement): any;
}

declare var Pixastic: PixasticStatic;

declare module "pixastic" {
    export = Pixastic;
}
