import { Component, Input } from '@angular/core';
import { NgFor } from '@angular/common';
import { PhotoDataService, IComment, IPhoto } from '../shared';

@Component({
  moduleId: module.id,
  selector: 'app-comment-view',
  directives: [ NgFor ],
  templateUrl: 'comment-view.component.html',
  styleUrls: ['comment-view.component.css']
})
export class CommentViewComponent {
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
