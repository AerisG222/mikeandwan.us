import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { MemoryService } from '../services/memory.service';

@Injectable()
export class CanPlayGuard implements CanActivate {
    constructor(private router: Router,
                private memoryService: MemoryService) {

    }

    canActivate() {
        if (this.memoryService.isReadyToPlay()) {
            return true;
        }

        this.router.navigateByUrl('/');

        return false;
    }
}
