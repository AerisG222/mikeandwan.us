import { IVideoInfo } from './ivideo-info.model';

export interface ICategory {
    id: number;
    name: string;
    year: number;
    teaserThumbnail: IVideoInfo;
}
