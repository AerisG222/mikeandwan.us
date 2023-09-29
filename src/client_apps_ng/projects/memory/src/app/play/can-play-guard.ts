import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { MemoryService } from '../services/memory.service';

@Injectable()
export class CanPlayGuard  {
    constructor(private router: Router,
                private memoryService: MemoryService) {

    }

    canActivate(): boolean {
        if (this.memoryService.isReadyToPlay()) {
            return true;
        }

        this.router.navigateByUrl('/');

        return false;
    }
}
