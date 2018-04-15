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
    },
    plugins: [
        new webpack.DefinePlugin({
            __IS_PRODUCTION__: JSON.stringify(false),
            __API_URL__: JSON.stringify('https://apidev.mikeandwan.us:5011'),
            __AUTH_CONFIG__: JSON.stringify({
                authUrl: 'https://authdev.mikeandwan.us:5001',
                wwwUrl: 'https://wwwdev.mikeandwan.us:5021'
            })
        })
    ]
}