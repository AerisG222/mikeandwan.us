import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { NgClass, NgFor, NgIf } from '@angular/common';

@Component({
  moduleId: module.id,
  selector: 'app-rating',
  directives: [ NgClass, NgIf, NgFor ],
  templateUrl: 'rating.component.html',
  styleUrls: ['rating.component.css']
})
export class RatingComponent implements OnInit {
    private _rating : number = null;
    @Input() editable : boolean = false;
    @Input() maxRating : number = 5;
    @Output() rate : EventEmitter<number> = new EventEmitter<number>();
    hover : number = -1;
    ratingRange : Array<number> = [];
    ratingIcons : Array<string> = [];
    
    ngOnInit() : void {
        let range : Array<number> = [];
        let icons : Array<string> = [];
        
        for(let i = 0; i < this.maxRating; i++) {
            range.push(i + 1);
            icons.push(this.getIcon(i + 1));
        }
        
        this.ratingRange = range;
        this.ratingIcons = icons;
    }
    
    @Input() set rating(value : number) {
        this._rating = value;
        this.updateDisplay();
    }
    
    get rating() : number {
        return this._rating;
    }
    
    getIcon(value : number) : string {
        let compareVal : number;
        
        if(this.hover === -1) {
            compareVal = this.rating;
        }
        else {
            compareVal = this.hover;
        }
        
        if(value <= compareVal) {
            return '&#xf005;';
        }
        else {
            return '&#xf006;';
        }
    }
    
    // so why do we have 2 different arrays?  well, it is all because of the mouse events.  when using the ngrepeat with an array that spit
    // out the icons, this worked great to update the display of the icons, but angular was then updating the model/html everytime, which caused
    // the mouseenter event to fire many times on mouseover (i believe due to the fact the array bound to the repeater was updated everytime). to
    // address this, we have the icon array which will change its values, but the repeater will be tied to a fixed array that won't update other than
    // when displaying the first time.
    updateDisplay() : void {
        for(let i = 0; i < this.ratingRange.length; i++) {
            this.ratingIcons[i] = this.getIcon(i + 1);
        }
    }
    
    setHover(value : number) : void {
        if(this.editable) {
            this.hover = value;
            this.updateDisplay();
        }
    }
    
    onRate(rating : number) : void {
        if(this.editable) {
            this.rating = rating;
            this.rate.next(this.rating);
        }
    }
}
