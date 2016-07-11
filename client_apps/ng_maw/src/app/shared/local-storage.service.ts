import { Injectable } from '@angular/core';

@Injectable()
export class LocalStorageService {
    get(name: string, defaultValue: any): any {
        let val = window.localStorage.getItem(name);

        if (val == null) {
            return defaultValue;
        }

        try {
            return JSON.parse(val);
        }
        catch (x) {
            return defaultValue;
        }
    }

    set(name: string, value: any): void {
        window.localStorage.setItem(name, JSON.stringify(value));
    }
}
