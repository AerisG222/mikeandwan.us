import { Component } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';

import { Result, SizeInfo } from './';

@Component({
  moduleId: module.id,
  selector: 'bandwidth-app',
  directives: [ NgIf, NgFor ],
  templateUrl: 'bandwidth.component.html',
  styleUrls: ['bandwidth.component.css']
})
export class BandwidthAppComponent {
    results : Result[] = null;
    sizeArray = [ new SizeInfo('OC192', '9,400 Mbps',      9400000000 ),
                  new SizeInfo('OC48', '2,400 Mbps',       2400000000 ),
                  new SizeInfo('GigE', '1000 Mbps',        1000000000 ),
                  new SizeInfo('OC12', '622 Mbps',          622000000 ),
                  new SizeInfo('OC3', '155 Mbps',           155000000 ),
                  new SizeInfo('Fast Ethernet', '100 Mbps', 100000000 ),
                  new SizeInfo('T3', '45 Mbps',              45000000 ),
                  new SizeInfo('T1', '1.54 Mbps',             1540000 ),
                  new SizeInfo('Cable/DSL', '512 Kbps',        512000 ),
                  new SizeInfo('Cable/DSL', '256 Kbps',        256000 ),
                  new SizeInfo('Cable/DSL', '128 Kbps',        128000 ),
                  new SizeInfo('ISDN', '64 Kbps',               64000 ),
                  new SizeInfo('Modem', '56.6 Kbps',            56600 ),
                  new SizeInfo('Modem', '33.6 Kbps',            33600 ),
                  new SizeInfo('Modem', '28.8 Kbps',            28800 ),
                  new SizeInfo('Modem', '14.4 Kbps',            14400 ) ];
    
    calculate(size : number, sizeScale : string, timeScale : string) : void {
        let timeInSeconds : number = 0;
        let sizeInBytes : number = 0;
        let calcResults : Result[] = [];
        
        // determine the divisor based on the time interval
        switch(timeScale) {
            case "m":
                timeInSeconds = 60;
                break;
            case "h":
                timeInSeconds = 60 * 60;
                break;
            case "s":
                timeInSeconds = 1;
                break;
            case "d":
                timeInSeconds = 60 * 60 * 24;
                break;
            default:
                throw "Invalid time interval specified!";
        }
        
        // now determine the full size of the file, in bits, based
        // on the file size scale
        switch(sizeScale) {
            case "b":
                sizeInBytes = size * 8;
                break;
            case "k":
                sizeInBytes = size * 1024 * 8;
                break;
            case "m":
                sizeInBytes = size * 1024 * 1024 * 8;
                break;
            case "g":
                sizeInBytes = size * 1024 * 1024 * 1024 * 8;
                break;
            default:
                throw "Invalid file size scale specified!";
        }   
        
        for(let i = 0; i < this.sizeArray.length; i++) {
            calcResults.push(new Result(this.sizeArray[i].name, this.sizeArray[i].speed, (sizeInBytes / this.sizeArray[i].bps) / timeInSeconds));
        }
        
        this.results = calcResults;
    };
}
