const webpack = require('webpack');

module.exports = {
    mode: 'development',
    entry: './photos3d.ts',
    devtool: 'source-map',
    output: {
        filename: 'main.bundle.js',
        library: 'Photos'
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
