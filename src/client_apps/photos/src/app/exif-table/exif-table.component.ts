import { Component, Input } from '@angular/core';

import { ExifDetail } from '../exif-view/exif-detail.model';

@Component({
    selector: 'exif-table',
    templateUrl: 'exif-table.component.html',
    styleUrls: [ 'exif-table.component.css' ]
})
export class ExifTableComponent {
    private _data: Array<ExifDetail>;

    exif: Array<Array<ExifDetail>> = [];

    @Input() set data(values: Array<ExifDetail>) {
        if(values == null) {
            return;
        }

        this.exif = [];
        this._data = values;

        for (let i = 0; i < values.length; i++) {
          let leftDetail = values[i];
          let rightDetail = new ExifDetail('', '');

          i++;

          if (i < values.length) {
              rightDetail = values[i];
          }

          this.exif.push([leftDetail, rightDetail]);
      }
    }

    get data(): Array<ExifDetail> {
        return this._data;
    }
}
