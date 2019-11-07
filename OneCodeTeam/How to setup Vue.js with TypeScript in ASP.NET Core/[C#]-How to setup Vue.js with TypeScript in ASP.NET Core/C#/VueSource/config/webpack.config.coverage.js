const webpackConfig = require("./webpack.config.base")
  DefinePlugin = require('webpack/lib/DefinePlugin'),
  env = require('../environment/dev.env');

webpackConfig.module.rules[1].query = {
  compilerOptions: {
    inlineSourceMap: true,
    sourceMap: false
  }
};

webpackConfig.module.rules = [...webpackConfig.module.rules,
  {
    test: /\.ts$/,
    enforce: "post",
    loader: "istanbul-instrumenter-loader",
    exclude: [
      "node_modules",
      /\.spec\.ts$/
    ],
    query: {
      esModules: true
    }
  }
];

webpackConfig.devtool = "inline-source-map";

webpackConfig.plugins = [...webpackConfig.plugins,
  new DefinePlugin({
    'process.env': env
  })
];

module.exports = webpackConfig;
