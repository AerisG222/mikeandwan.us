import { Component, OnInit, OnDestroy } from '@angular/core';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: [ './app.component.css' ]
})
export class AppComponent implements OnInit, OnDestroy {
    intervalId: number = null;
    value = '';

    ngOnInit(): void {
        this.updateMessage();
        this.intervalId = setInterval(() => { this.updateMessage(); }, <any>300);
    }

    ngOnDestroy(): void {
        clearInterval(this.intervalId);
    }

    isWeekend(currDate: Date): boolean {
        const day: number = currDate.getDay();

        if (day === 0 || day === 6) {
            return true;
        } else if (day === 5) {
            if (currDate.getHours() >= 17) {
                return true;
            }
        }

        return false;
    }

    updateMessage(): void {
        const theDate: Date = new Date();

        if (this.isWeekend(theDate)) {
            this.value = 'Sweet! It\'s the weekend!';
        } else {
            let secDelta = 0;
            let minDelta = 0;
            let hourDelta = 0;

            // now determine the difference to our target time for each part
            let dayDelta = 5 - theDate.getDay();

            if (theDate.getHours() < 17) {
                hourDelta = 17 - theDate.getHours();
            } else if (theDate.getHours() >= 17) {
                // subtract one day and add the day to our hours
                dayDelta -= 1;
                hourDelta = 24 + 17 - theDate.getHours();
            }

            if (theDate.getMinutes() !== 0) {
                // subtract one from the hours
                hourDelta -= 1;
                minDelta = 60 - theDate.getMinutes();
            }

            if (theDate.getSeconds() !== 0) {
                // subtract one from the minutes
                minDelta -= 1;
                secDelta = 60 - theDate.getSeconds();
            }

            this.value = `${dayDelta}d ${hourDelta}h ${minDelta}m ${secDelta}s`;
        }
    }
}
