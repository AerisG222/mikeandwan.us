export interface IVisual {
    init(): void;
    render(clockDelta: number): void;
}
