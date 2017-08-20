export enum RouteMode {
    None,  // this was added and magically provides a workaround for https://github.com/angular/angular/issues/18170
    Category,
    Comment,
    Rating,
    Random
}
