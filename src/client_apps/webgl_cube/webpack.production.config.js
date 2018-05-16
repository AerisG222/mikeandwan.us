const webpack = require('webpack');
const StatsWriterPlugin = require("webpack-stats-plugin").StatsWriterPlugin;

module.exports = {
    mode: 'production',
    entry: './cube.ts',
    devtool: 'source-map',
    output: {
        filename: 'main.[hash].bundle.js',
        library: 'WebGLDemo'
    },
    resolve: {
        extensions: ['.webpack.js', '.web.js', '.ts', '.js']
    },
    module: {
        rules: [
            { test: /\.ts$/, use: 'ts-loader' }
        ]
    },
    plugins: [
        new StatsWriterPlugin({
            filename: "stats.json",
            fields: null
        })
    ]
}
