const StatsWriterPlugin = require("webpack-stats-plugin").StatsWriterPlugin;

module.exports = {
    mode: 'production',
    entry: './text.ts',
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
            {
                test: /\.ts$/,
                use: 'ts-loader',
                exclude: /node_modules/
            },
            {
                test: /\.json$/,
                type: 'javascript/auto',
                use: [{
                    loader: 'file-loader',
                    options: {
                        name: '[name].[hash].[ext]',
                        publicPath: '/js/webgl_text/'
                    }
                }]
            },
            {
                test: /\.(png|jpg|gif)$/,
                use: [{
                    loader: 'file-loader',
                    options: {
                        name: '[name].[hash].[ext]',
                        publicPath: '/js/webgl_text/'
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
