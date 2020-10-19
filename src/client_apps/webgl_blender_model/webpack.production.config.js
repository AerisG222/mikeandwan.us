const StatsWriterPlugin = require("webpack-stats-plugin").StatsWriterPlugin;

module.exports = {
    mode: 'production',
    entry: './blender_model.ts',
    devtool: 'source-map',
    output: {
        filename: 'main.[contenthash].bundle.js',
        library: 'WebGLDemo'
    },
    resolve: {
        extensions: ['.webpack.js', '.web.js', '.ts', '.js']
    },
    module: {
        rules: [
            {
                test: /\.ts$/,
                use: 'ts-loader',
                exclude: /node_modules/
            },
            {
                test: /\.gltf$/,
                use: [{
                    loader: 'file-loader',
                    options: {
                        name: '[name].[contenthash].[ext]',
                        publicPath: '/js/webgl_blender_model/'
                    }
                }]
            },
            {
                test: /\.bin$/,
                use: [{
                    loader: 'file-loader',
                    options: {
                        name: '[name].[ext]',
                        publicPath: '/js/webgl_blender_model/'
                    }
                }]
            }
        ]
    },
    plugins: [
        new StatsWriterPlugin({
            filename: "stats.json",
            fields: null
        })
    ]
}
