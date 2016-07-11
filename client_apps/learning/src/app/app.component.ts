import { Component, ElementRef, ViewChild } from '@angular/core';
import { NgFor } from '@angular/common';

import { AudioSource } from './audio-source.model';

@Component({
    moduleId: module.id,
    selector: 'learning-app',
    directives: [ NgFor ],
    templateUrl: 'app.component.html',
    styleUrls: [ 'app.component.css' ]
})
export class LearningAppComponent {
    letters: string[] = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];
    numbers: string[] = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '10'];
    speakers: string[] = ['Mommy', 'Daddy', 'No Sound'];
    lessons: string[] = ['Letters', 'Numbers', 'Random'];
    currentSpeaker: string = 'Mommy';
    currentLesson: string = 'Letters';
    currentChar: string = '';
    currentCharAudio: string = '';
    doRun: boolean = false;
    runButtonText: string = 'Start';
    intervalId: number = null;
    @ViewChild('audio') audioElement: ElementRef;

    toggleRunning(): void {
        this.doRun = !this.doRun;

        if (this.doRun) {
            this.run();  // immediately start first character
            this.intervalId = setInterval(() => this.run(), 3000);  // schedule additional letters
        }
        else {
            clearInterval(this.intervalId);
        }

        this.runButtonText = this.getButtonText();
    };

    run(): void {
        this.currentChar = this.getNextChar();

        if (this.currentSpeaker !== "No Sound") {
            let srcs = this.getCurrentAudioSources();

            // originally tried to use the source elements under the audio, but could not get this to
            // work cross browser (worked fine in ff, but in chrome only played the audio after moving
            // to the next letter.  found the following which is working well:
            // http://stackoverflow.com/questions/5235145/changing-source-on-html5-video-tag
            if (Modernizr.audio && Modernizr.audio.ogg) {
                this.currentCharAudio = srcs.ogg;
            }
            else {
                this.currentCharAudio = srcs.mp3;
            }

            (<HTMLMediaElement>this.audioElement.nativeElement).load();
        }
    }

    getCurrentAudioSources(): AudioSource {
        let prefix = `/audio/learning/${this.currentSpeaker.toLowerCase()}/${this.currentChar.toLowerCase()}`;

        return new AudioSource(`${prefix}.mp3`, `${prefix}.ogg`);
    }

    getButtonText(): string {
        return this.doRun ? 'Stop' : 'Start';
    }

    getNextChar(): string {
        if (this.currentLesson === 'Letters') {
            return this.getNextInArray(this.currentChar, this.letters);
        }
        else if (this.currentLesson === 'Numbers') {
            return this.getNextInArray(this.currentChar, this.numbers);
        }
        else {
            let arr = this.letters.concat(this.numbers);
            return arr[Math.floor(Math.random() * arr.length)];
        }
    }

    getNextInArray(currItem: string, arr: string[]): string {
        let i = arr.indexOf(currItem) + 1;

        if (i < 0 || i >= arr.length) {
            i = 0;
        }

        return arr[i];
    }
}
