import { Component } from '@angular/core';

@Component({
    selector: 'bytecounter',	
    templateUrl: '/js/tools/byteCounter/ByteCounter.html'
})
export class ByteCounter {
	bCount : number = 0;
	kbCount : number = 0;
	mbCount : number = 0;

	calculate(value : string) : void {
		this.bCount = value.length;
		this.kbCount = this.bCount / 1024;
		this.mbCount = this.bCount / (1024 * 1024);
	};
}
