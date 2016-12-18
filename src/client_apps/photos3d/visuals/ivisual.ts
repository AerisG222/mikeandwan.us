export interface IVisual {
    init(): void;
    dispose(): void;
    render(clockDelta: number, elapsed: number): void;
}
