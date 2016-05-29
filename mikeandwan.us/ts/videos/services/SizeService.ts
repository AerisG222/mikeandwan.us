export class SizeService {
	MAX_THUMB_WIDTH = 160;
	MAX_THUMB_HEIGHT = 120;
	RATIO = this.MAX_THUMB_WIDTH / this.MAX_THUMB_HEIGHT;
	
	getThumbHeight(width : number, height : number) : number {
		let thumbRatio = width / height;
		
		if(thumbRatio <= this.RATIO) {
			return this.MAX_THUMB_HEIGHT;
		}
		
		return Math.floor(this.MAX_THUMB_WIDTH / thumbRatio);
	}
	
	getThumbWidth(width : number, height : number) : number {
		let thumbRatio = width / height;
		
		if(thumbRatio >= this.RATIO) {
			return this.MAX_THUMB_WIDTH;
		}
		
		return Math.floor(this.MAX_THUMB_HEIGHT * thumbRatio);
	}
}
