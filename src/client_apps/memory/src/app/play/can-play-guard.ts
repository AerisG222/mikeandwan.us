import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { MemoryService } from '../services/memory.service';

@Injectable()
export class CanPlayGuard implements CanActivate {
    constructor(private _router: Router,
                private _memoryService: MemoryService) {

    }

    canActivate() {
        if (this._memoryService.isReadyToPlay()) {
            return true;
        }

        this._router.navigateByUrl('/');

        return false;
    }
}
