import { Component, Input } from '@angular/core';
import { NgFor } from '@angular/common';
import { PhotoDataService } from '../services/PhotoDataService';
import { IComment } from '../interfaces/IComment';
import { IPhoto } from '../interfaces/IPhoto';

@Component({
    selector: 'comments',
    directives: [ NgFor ],
    templateUrl: '/js/photos/components/CommentView.html'
})
export class CommentView {
    private _photo : IPhoto = null;
    comments: Array<IComment> = [];

    constructor(private _dataService: PhotoDataService) {

    }

    @Input() set photo(val : IPhoto) {
        this._photo = val;
        this.getComments();
    }
    
    get photo() : IPhoto {
        return this._photo;
    }
    
    getComments() : void {
        this._dataService
            .getCommentsForPhoto(this._photo.id)
            .subscribe(comments => this.comments = comments);
    }

    hasComments(): boolean {
        return this.comments.length > 0;
    }

    addComment(comment : string): void {
        let photo = this._photo;
        
        if(photo !== null) {
            this._dataService.addCommentForPhoto(photo.id, comment)
                .subscribe( (x:any) => this.getComments() );
        }
    }
}
