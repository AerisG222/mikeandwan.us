export interface IController {
    readonly areVisualsEnabled: boolean;
    init(): void;
    render(delta: number, elapsed: number): void;
    enableVisuals(areEnabled: boolean): void;
}
