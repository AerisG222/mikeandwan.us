export class ArgumentNullError extends Error {
    private _argName: string;

    constructor(argumentName: string) {
        super(`${argumentName} should not be null.`);

        this._argName = argumentName;
    }

    get argumentName(): string {
        return this._argName;
    }
}
