module.exports = {
    entry: './cube.ts',
    devtool: 'source-map',
    output: {
        filename: 'dist/main.bundle.js',
        library: 'WebGLDemo',
        libraryTarget: 'var'
    },
    resolve: {
        extensions: ['.webpack.js', '.web.js', '.ts', '.js']
    },
    module: {
        loaders: [
            {
                test: /\.ts$/,
                loader: 'ts-loader'
            }
        ]
    }
}