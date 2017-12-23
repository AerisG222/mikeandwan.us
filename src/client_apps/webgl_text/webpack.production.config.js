const webpack = require('webpack');

module.exports = {
    entry: './text.ts',
    devtool: 'source-map',
    output: {
        filename: 'dist/main.[hash].bundle.js',
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
    },
    plugins: [
        new webpack.LoaderOptionsPlugin({
            minimize: true,
            debug: false
        }),
        new webpack.optimize.UglifyJsPlugin({
            mangle: { screw_ie8 : true },
            compress: { screw_ie8: true },
            output: { comments: false },
            sourceMap: true
        })
    ]
}
