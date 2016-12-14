export interface IVisual {
    init(): void;
    render(clockDelta: number, elapsed: number): void;
}
