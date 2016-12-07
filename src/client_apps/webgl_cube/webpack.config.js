module.exports = {
    entry: './cube.ts',
    devtool: 'source-map',
    output: {
        filename: 'dist/main.bundle.js'
    },
    resolve: {
        extensions: ['.webpack.js', '.web.js', '.ts', '.js']
    },
    module: {
        loaders: [
            { 
                test: /\.ts$/, 
                loader: 'awesome-typescript-loader' 
            }
        ]
    }
}