import { Component, ViewChild, ChangeDetectorRef, AfterViewInit, OnInit } from '@angular/core';
import { NgIf, NgStyle } from '@angular/common';
import { RouteParams } from '@angular/router-deprecated';

import { PagerComponent } from '../../../../ng_maw/src/app/pager';
import { ThumbnailListComponent, SelectedThumbnail } from '../../../../ng_maw/src/app/thumbnail-list';

import { IVideo, IVideoInfo, SizeService, VideoDataService, VideoStateService, VideoThumbnailInfo } from '../shared';

@Component({
    moduleId: module.id,
    selector: 'app-video-list',
    directives: [ NgIf, NgStyle, PagerComponent, ThumbnailListComponent ],
    templateUrl: 'video-list.component.html',
    styleUrls: ['video-list.component.css']
})
export class VideoListComponent implements OnInit {
    @ViewChild(PagerComponent) pager : PagerComponent;
    @ViewChild(ThumbnailListComponent) thumbnailList : ThumbnailListComponent;
    categoryId : number = null;
    year : number = null;
	currentVideo : IVideo = null;
    displayedVideo : IVideoInfo = null;
    videoList : Array<IVideo> = [];

    constructor(private _sizeService : SizeService, 
                private _videoDataService : VideoDataService,
                private _videoStateService : VideoStateService,
                private _changeDetectionRef : ChangeDetectorRef,
                params : RouteParams) {
        this.year = parseInt(params.get('year'), 10);
        this.categoryId = parseInt(params.get('category'), 10);
    }
    
    ngOnInit() : void {
        this._videoStateService.configUpdatedEventEmitter.subscribe(    
            (data : string) => this.updateVideo()
        );
    }
    
    ngAfterViewInit() : void {
        this.thumbnailList.setRowCountPerPage(2);
        
        this.thumbnailList.itemsPerPageUpdated.subscribe((x : any) => {
            this.updatePager();
        });
        
        this._changeDetectionRef.detectChanges();
        
        this._videoDataService.getVideosForCategory(this.categoryId)
            .subscribe(
                (data : Array<IVideo>) => this.setVideos(data),
                (err : any) => console.error("there was an error: " + err)
            );
    }

    onChangePage(pageIndex : number) : void {
        this.thumbnailList.setPageDisplayedIndex(pageIndex);
    }

    onThumbnailSelected(item : SelectedThumbnail) : void {
        if(item.index >= 0 && this.videoList.length > item.index) {
            let vid : IVideo = (<VideoThumbnailInfo>item.thumbnail).video;
            this.currentVideo = vid;
            this.updateVideo();
        }
    }

    setVideos(videos : Array<IVideo>) : void {
        this.videoList = videos;
        
        let thumbnails : Array<VideoThumbnailInfo> = videos.map((vid : IVideo) => 
            new VideoThumbnailInfo(vid.thumbnailVideo.path,
                this._sizeService.getThumbHeight(vid.thumbnailVideo.width, vid.thumbnailVideo.height),
                this._sizeService.getThumbWidth(vid.thumbnailVideo.width, vid.thumbnailVideo.height),
                vid
            )
        );
        
        this.thumbnailList.setItemList(thumbnails);
        this.pager.setPageCount(Math.ceil(thumbnails.length / this.thumbnailList.itemsPerPage));
    }
    
    updateVideo() : void {
        if(this._videoStateService.config.preferFullSize) {
            this.displayedVideo = this.currentVideo.fullsizeVideo;
        }
        else {
            this.displayedVideo = this.currentVideo.scaledVideo;
        }

        setTimeout(function() {
            let el = (<HTMLMediaElement>document.querySelector('video'));
            el.load();
            el.play();
        }, 0, false);
    }
    
    private updatePager() {
        this.pager.setPageCount(this.pager.calcPageCount(this.thumbnailList.itemList.length, this.thumbnailList.itemsPerPage));
        this.pager.setActivePage(this.thumbnailList.pageDisplayedIndex);
    }
}
