const webpack = require('webpack');

module.exports = {
    mode: 'production',
    entry: './stats.ts',
    devtool: 'source-map',
    output: {
        filename: 'main.[hash].bundle.js',
        library: 'Photos'
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
        new webpack.LoaderOptionsPlugin({
            minimize: true,
            debug: true,
            minimize: true
        }),
        new webpack.optimize.UglifyJsPlugin({
            mangle: { screw_ie8 : true },
            compress: { screw_ie8: true },
            output: { comments: false },
            sourceMap: true,
            sourceMap: true
        })
    ]
}
