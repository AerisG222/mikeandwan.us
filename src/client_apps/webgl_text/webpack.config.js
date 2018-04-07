module.exports = {
    mode: 'development',
    entry: './text.ts',
    devtool: 'source-map',
    output: {
        filename: 'main.bundle.js',
        library: 'WebGLDemo'
    },
    resolve: {
        extensions: ['.webpack.js', '.web.js', '.ts', '.js']
    },
    module: {
        rules: [
            { test: /\.ts$/, use: 'ts-loader' }
        ]
    }
}
