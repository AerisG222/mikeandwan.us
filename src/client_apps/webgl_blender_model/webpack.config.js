const StatsWriterPlugin = require("webpack-stats-plugin").StatsWriterPlugin;

module.exports = {
    mode: 'development',
    entry: './blender_model.ts',
    devtool: 'source-map',
    output: {
        filename: 'main.bundle.js',
        library: 'WebGLDemo',
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
                type: 'asset/resource',
                generator: {
                    filename: '[name].[hash][ext]',
                    publicPath: '/js/webgl_blender_model/'
                }
            },
            {
                test: /\.bin$/,
                type: 'asset/resource',
                generator: {
                    filename: '[name][ext]',
                    publicPath: '/js/webgl_blender_model/'
                }
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
