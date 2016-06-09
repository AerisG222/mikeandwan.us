import {
    beforeEachProviders,
    it,
    describe,
    expect,
    inject
} from '@angular/core/testing';
import { BreadcrumbService } from './breadcrumb.service';

describe('Breadcrumb Service', () => {
    beforeEachProviders(() => [BreadcrumbService]);

    it('should ...',
        inject([BreadcrumbService], (service: BreadcrumbService) => {
            expect(service).toBeTruthy();
        }));
});
