export interface IController {
    readonly areVisualsEnabled: boolean;
    enableVisuals(areEnabled: boolean): void;
    init(): void;
    render(clockDelta: number, elapsed: number): void;
}
