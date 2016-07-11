export interface PixasticStatic {
	process(img : HTMLElement, operation : string, options : any) : any;
	revert(img : HTMLElement) : any;
}

export var Pixastic: PixasticStatic;