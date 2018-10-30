import { Component, ChangeDetectorRef, AfterViewInit, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { state, style, transition, trigger, useAnimation } from '@angular/animations';

import { fadeIn, fadeOut } from 'maw-common';
import { IVideo } from '../models/ivideo.model';
import { IVideoInfo } from '../models/ivideo-info.model';
import { VideoDataService } from '../services/video-data.service';
import { VideoStateService } from '../services/video-state.service';
import { Observable } from 'rxjs';
import { VideoNavigationService } from '../services/video-navigation.service';

@Component({
    selector: 'app-video-list',
    templateUrl: './video-list.component.html',
    styleUrls: [ './video-list.component.css' ],
    animations: [
        trigger('fadeInOut', [
            state('in', style({opacity: 1})),
            transition(':enter', useAnimation(fadeIn)),
            transition(':leave', useAnimation(fadeOut))
        ])
    ]
})
export class VideoListComponent implements OnInit, AfterViewInit {
    cardsPerPage = 24;
    page = 1;
    categoryId: number = null;
    year: number = null;
    currentVideo: IVideo = null;
    displayedVideo: IVideoInfo = null;
    videoList$: Observable<IVideo[]>;

    constructor(private _videoDataService: VideoDataService,
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

            this._changeDetectionRef.detectChanges();

            // for some reason this component is not raising the event bound in the
            // constructor, and am thinking it might be due to our use of activated route.
            // as such, try to call this manually to see if the header updates the navigation
            // state when starting from video list page
            this._videoNavigationService.onRouterEvent(null);

            this.videoList$ = this._videoDataService
                .getVideosForCategory(this.categoryId);
        });
    }

    onVideoSelected(video: IVideo): void {
        if (video !== null) {
            this.currentVideo = video;
            this.updateVideo();
        }
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
}
