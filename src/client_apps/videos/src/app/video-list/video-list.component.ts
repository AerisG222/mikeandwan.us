import { Component, ViewChild, ChangeDetectorRef, AfterViewInit, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { trigger, state, style, animate, transition } from '@angular/animations';

import { PagerComponent } from '../../ng_maw/pager/pager.component';
import { ThumbnailListComponent } from '../../ng_maw/thumbnail-list/thumbnail-list.component';
import { SelectedThumbnail } from '../../ng_maw/thumbnail-list/selected-thumbnail.model';

import { IVideo } from '../shared/ivideo.model';
import { IVideoInfo } from '../shared/ivideo-info.model';
import { SizeService } from '../shared/size.service';
import { VideoDataService } from '../shared/video-data.service';
import { VideoNavigationService } from '../shared/video-navigation.service';
import { VideoStateService } from '../shared/video-state.service';
import { VideoThumbnailInfo } from '../shared/video-thumbnail-info.model';

@Component({
    selector: 'app-video-list',
    templateUrl: './video-list.component.html',
    styleUrls: [ './video-list.component.css' ],
    animations: [
        trigger('fadeInOut', [
            state('in', style({opacity: 1})),
            transition('void => *', [
                style({opacity: 0}),
                animate(320)
            ]),
            transition('* => void', [
                animate(320, style({opacity: 1}))
            ])
        ])
    ]
})
export class VideoListComponent implements OnInit, AfterViewInit {
    @ViewChild(PagerComponent) pager: PagerComponent;
    @ViewChild(ThumbnailListComponent) thumbnailList: ThumbnailListComponent;
    categoryId: number = null;
    year: number = null;
    currentVideo: IVideo = null;
    displayedVideo: IVideoInfo = null;
    videoList: Array<IVideo> = [];

    constructor(private _sizeService: SizeService,
                private _videoDataService: VideoDataService,
                private _videoStateService: VideoStateService,
                private _videoNavigationService: VideoNavigationService,
                private _changeDetectionRef: ChangeDetectorRef,
                private _activatedRoute: ActivatedRoute) {

    }

    ngOnInit(): void {
        this._videoStateService.configUpdatedEventEmitter.subscribe(
            (data: string) => this.updateVideo()
        );
    }

    ngAfterViewInit(): void {
        this._activatedRoute.params.subscribe(params => {
            this.year = parseInt(params['year'], 10);
            this.categoryId = parseInt(params['category'], 10);

            this.thumbnailList.setRowCountPerPage(2);

            this.thumbnailList.itemsPerPageUpdated.subscribe((x: any) => {
                this.updatePager();
            });

            this._changeDetectionRef.detectChanges();

            // for some reason this component is not raising the event bound in the
            // constructor, and am thinking it might be due to our use of activated route.
            // as such, try to call this manually to see if the header updates the navigation
            // state when starting from video list page
            this._videoNavigationService.onRouterEvent(null);

            this._videoDataService.getVideosForCategory(this.categoryId)
                .subscribe(
                    (data: Array<IVideo>) => this.setVideos(data),
                    (err: any) => console.error('there was an error: ' + err)
                );
        });
    }

    onChangePage(pageIndex: number): void {
        this.thumbnailList.setPageDisplayedIndex(pageIndex);
    }

    onThumbnailSelected(item: SelectedThumbnail): void {
        if (item.index >= 0 && this.videoList.length > item.index) {
            const vid: IVideo = (<VideoThumbnailInfo>item.thumbnail).video;
            this.currentVideo = vid;
            this.updateVideo();
        }
    }

    setVideos(videos: Array<IVideo>): void {
        this.videoList = videos;

        const thumbnails: Array<VideoThumbnailInfo> = videos.map((vid: IVideo) =>
            new VideoThumbnailInfo(vid.thumbnailVideo.path,
                this._sizeService.getThumbHeight(vid.thumbnailVideo.width, vid.thumbnailVideo.height),
                this._sizeService.getThumbWidth(vid.thumbnailVideo.width, vid.thumbnailVideo.height),
                vid
            )
        );

        this.thumbnailList.setItemList(thumbnails);
        this.pager.setPageCount(Math.ceil(thumbnails.length / this.thumbnailList.itemsPerPage));
    }

    updateVideo(): void {
        if (this._videoStateService.config.preferFullSize) {
            this.displayedVideo = this.currentVideo.fullsizeVideo;
        } else {
            this.displayedVideo = this.currentVideo.scaledVideo;
        }

        setTimeout(function () {
            const el = (<HTMLMediaElement>document.querySelector('video'));
            el.load();
            el.play();
        }, 0, false);
    }

    private updatePager() {
        this.pager.setPageCount(this.pager.calcPageCount(this.thumbnailList.itemList.length, this.thumbnailList.itemsPerPage));
        this.pager.setActivePage(this.thumbnailList.pageDisplayedIndex);
    }
}
