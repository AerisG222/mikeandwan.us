import {
    describe,
    ddescribe,
    expect,
    iit,
    it
} from '@angular/core/testing';
import {FilterSettings} from './filter-settings.model';

describe('FilterSettings', () => {
    it('should create an instance', () => {
        expect(new FilterSettings()).toBeTruthy();
    });
});
