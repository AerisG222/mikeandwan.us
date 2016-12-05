import { Component, Input } from '@angular/core';

import { PhotoDataService } from '../shared/photo-data.service';
import { IComment } from '../shared/icomment.model';
import { IPhoto } from '../shared/iphoto.model';

@Component({
    selector: 'app-comment-view',
    templateUrl: 'comment-view.component.html',
    styleUrls: [ 'comment-view.component.css' ]
})
export class CommentViewComponent {
    private _photo: IPhoto = null;
    newComment = '';
    comments: Array<IComment> = [];

    constructor(private _dataService: PhotoDataService) {

    }

    @Input() set photo(val: IPhoto) {
        this._photo = val;
        this.getComments();
    }

    get photo(): IPhoto {
        return this._photo;
    }

    getComments(): void {
        this._dataService
            .getCommentsForPhoto(this._photo.id)
            .subscribe(comments => this.comments = comments);
    }

    hasComments(): boolean {
        return this.comments.length > 0;
    }

    addComment(): void {
        let photo = this._photo;

        if (photo !== null) {
            this._dataService.addCommentForPhoto(photo.id, this.newComment)
                .subscribe((x: any) => {
                    this.getComments();
                    this.newComment = '';
                });
        }
    }
}
