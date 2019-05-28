var path = require("path");
var WebpackNotifierPlugin = require("webpack-notifier");
var BrowserSyncPlugin = require("browser-sync-webpack-plugin");


module.exports = [
    {
        entry: './Scripts/Vue/index.js',
        output: {
            path: path.resolve(__dirname, "./Scripts/dist"),
            filename: 'bundle.js'
        },
        devtool: 'inline-source-map',
        module: {
            rules: [
                {
                    test: /\.js$/,
                    exclude: /node_modules/,
                    loader: "babel-loader"
                }
            ]
        },
        plugins: [new WebpackNotifierPlugin()]
    },
];