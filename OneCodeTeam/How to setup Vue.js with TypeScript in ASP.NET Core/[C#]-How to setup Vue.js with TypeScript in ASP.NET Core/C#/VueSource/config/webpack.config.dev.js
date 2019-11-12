const helpers = require("./helpers"),
  webpackConfig = require("./webpack.config.base"),
  DefinePlugin = require('webpack/lib/DefinePlugin'),
  env = require('../environment/dev.env');

webpackConfig.devServer = {
  port: 8080,
  host: "localhost",
  historyApiFallback: true,
  watchOptions: {aggregateTimeout: 300, poll: 1000},
  contentBase: './src',
  open: true
};

webpackConfig.plugins = [...webpackConfig.plugins,
  new DefinePlugin({
    'process.env': env
  })
]

module.exports = webpackConfig;
