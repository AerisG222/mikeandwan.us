var webpack = require('webpack');

module.exports = {
    mode: 'development',
    entry: './stats.ts',
    devtool: 'source-map',
    output: {
        filename: 'main.bundle.js',
        library: 'Photos'
    },
    resolve: {
        extensions: ['.webpack.js', '.web.js', '.ts', '.js']
    },
    module: {
        rules: [ {
            test: /\.ts$/,
            use: 'ts-loader' }
        ]
    },
    plugins: [
        new webpack.DefinePlugin({
            __IS_PRODUCTION__: JSON.stringify(false),
            __API_URL__: JSON.stringify('https://localhost:5011'),
            __AUTH_CONFIG__: JSON.stringify({
                authUrl: 'https://localhost:5001',
                wwwUrl: 'https://localhost:5021'
            })
        })
    ]
}