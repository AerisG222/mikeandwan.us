export class Config {
    static DISPLAY_MODE_INLINE = 1;
    static DISPLAY_MODE_POPUP = 2;

    displayMode: number = Config.DISPLAY_MODE_INLINE;
    rowsPerPage = 3;
    slideshowIntervalSeconds = 3;
}
