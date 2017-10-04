module.exports = {
    entry: './shader.ts',
    devtool: 'source-map',
    output: {
        filename: 'dist/main.bundle.js',
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
                loader: 'awesome-typescript-loader' 
            }
        ]
    }
}