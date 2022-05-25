const StatsWriterPlugin = require("webpack-stats-plugin").StatsWriterPlugin;

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
            {
                test: /\.ts$/,
                use: 'ts-loader',
                exclude: /node_modules/
            },
            {
                test: /\.json$/,
                type: 'asset/resource',
                generator: {
                    filename: '[name].[hash][ext]',
                    publicPath: '/js/webgl_text/'
                }
            },
            {
                test: /\.(png|jpg|gif)$/,
                type: 'asset/resource',
                generator: {
                    filename: '[name].[hash][ext]',
                    publicPath: '/js/webgl_text/'
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
