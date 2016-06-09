import {
    beforeEachProviders,
    it,
    describe,
    expect,
    inject
} from '@angular/core/testing';
import { MemoryService } from './memory.service';

describe('Memory Service', () => {
    beforeEachProviders(() => [MemoryService]);

    it('should ...',
        inject([MemoryService], (service: MemoryService) => {
            expect(service).toBeTruthy();
        }));
});
